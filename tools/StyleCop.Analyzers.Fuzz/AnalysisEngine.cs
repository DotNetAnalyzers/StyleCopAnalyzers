// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.Fuzz;

using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

internal sealed class AnalysisEngine
{
    private readonly ImmutableArray<DiagnosticAnalyzer> analyzers;
    private readonly ImmutableArray<CodeFixProvider> fixers;
    private readonly ImmutableArray<MetadataReference> references;

    internal AnalysisEngine()
        : this(
            LoadTypes<DiagnosticAnalyzer>("StyleCop.Analyzers").Concat(LoadTypes<DiagnosticAnalyzer>("StyleCop.Analyzers.CodeFixes")).ToImmutableArray(),
            LoadTypes<CodeFixProvider>("StyleCop.Analyzers.CodeFixes").ToImmutableArray())
    {
    }

    internal AnalysisEngine(ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<CodeFixProvider> fixers)
    {
        this.analyzers = analyzers;
        this.fixers = fixers;
        var platformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")
            ?? throw new InvalidOperationException("Runtime reference assemblies are unavailable.");
        this.references = platformAssemblies.Split(Path.PathSeparator)
            .Where(path => Path.GetFileName(path).StartsWith("System.", StringComparison.Ordinal)
                || Path.GetFileName(path) is "System.dll" or "netstandard.dll" or "mscorlib.dll")
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToImmutableArray<MetadataReference>();
    }

    internal static string Version => Assembly.Load("StyleCop.Analyzers")
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

    internal static void Progress(string stage) => Console.Error.WriteLine(stage);

    internal async Task<Result> RunAsync(Input input)
    {
        var findings = new List<Finding>();
        using var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        var options = this.analyzers.SelectMany(analyzer => analyzer.SupportedDiagnostics)
            .Select(descriptor => descriptor.Id).Distinct()
            .ToImmutableDictionary(id => id, _ => ReportDiagnostic.Warn);
        var solution = workspace.CurrentSolution.AddProject(ProjectInfo.Create(
            projectId,
            VersionStamp.Default,
            "Fuzz",
            "Fuzz",
            LanguageNames.CSharp,
            filePath: "/fuzz/Fuzz.csproj",
            compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, specificDiagnosticOptions: options),
            parseOptions: new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.Diagnose),
            metadataReferences: this.references));
        solution = solution.AddDocument(documentId, input.Name, SourceText.From(input.Source, Encoding.UTF8), filePath: "/fuzz/" + input.Name);
        if (input.Settings != null)
        {
            solution = solution.AddAdditionalDocument(DocumentId.CreateNewId(projectId), "stylecop.json", SourceText.From(input.Settings), filePath: "/fuzz/stylecop.json");
        }

        var document = solution.GetDocument(documentId) ?? throw new InvalidOperationException("Missing input document.");
        Progress("Compiler");
        var compilation = await document.Project.GetCompilationAsync().ConfigureAwait(false)
            ?? throw new InvalidOperationException("Missing compilation.");
        var compiles = !compilation.GetDiagnostics().Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        var analyzerOptions = new CompilationWithAnalyzersOptions(
            document.Project.AnalyzerOptions,
            (exception, analyzer, _) => findings.Add(new Finding("AnalyzerException", analyzer.GetType().FullName!, string.Empty, string.Empty, exception.ToString())),
            concurrentAnalysis: false,
            logAnalyzerExecutionTime: false);
        Progress("Analyzers");
        var diagnostics = await compilation.WithAnalyzers(this.analyzers, analyzerOptions).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
        var originalSnapshot = await SnapshotAsync(solution).ConfigureAwait(false);
        var actionCount = 0;

        foreach (var diagnostic in diagnostics.Where(diagnostic => diagnostic.Id != "AD0001").OrderBy(diagnostic => diagnostic.Id).ThenBy(diagnostic => diagnostic.Location.SourceSpan.Start))
        {
            foreach (var fixer in this.fixers.Where(fixer => fixer.FixableDiagnosticIds.Contains(diagnostic.Id)))
            {
                var component = fixer.GetType().FullName!;
                var actions = new List<CodeAction>();
                Progress(component + " " + diagnostic.Id + " Register");
                try
                {
                    // Location.None and external-file diagnostics cannot use a document-based CodeFixContext.
                    if (!diagnostic.Location.IsInSource)
                    {
                        continue;
                    }

                    await fixer.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None)).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    findings.Add(new Finding("CodeFixException", component, diagnostic.Id, "Register", exception.ToString()));
                    continue;
                }

                foreach (var action in Flatten(actions))
                {
                    actionCount++;
                    Progress(component + " " + diagnostic.Id + " " + action.Title);
                    try
                    {
                        var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
                        var changes = operations.OfType<ApplyChangesOperation>().ToArray();
                        if (changes.Length != 1 || operations.Length != 1)
                        {
                            findings.Add(new Finding("UnsupportedOperation", component, diagnostic.Id, action.Title, string.Join(", ", operations.Select(operation => operation.GetType().FullName))));
                            continue;
                        }

                        var changed = changes[0].ChangedSolution;
                        var fixedDocument = changed.GetDocument(documentId);
                        var fixedSource = fixedDocument == null ? null : (await fixedDocument.GetTextAsync().ConfigureAwait(false)).ToString();
                        if (await SnapshotAsync(changed).ConfigureAwait(false) == originalSnapshot)
                        {
                            findings.Add(new Finding("CodeFixDoesNotChangeSolution", component, diagnostic.Id, action.Title, string.Empty, fixedSource));
                        }

                        if (compiles)
                        {
                            foreach (var project in changed.Projects)
                            {
                                var fixedCompilation = await project.GetCompilationAsync().ConfigureAwait(false)
                                    ?? throw new InvalidOperationException("Missing fixed compilation.");
                                var errors = fixedCompilation.GetDiagnostics().Where(error => error.Severity == DiagnosticSeverity.Error).ToArray();
                                if (errors.Length != 0)
                                {
                                    findings.Add(new Finding("CodeFixBreaksCode", component, diagnostic.Id, action.Title, string.Join("\n", errors.Select(error => error.ToString())), fixedSource));
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        findings.Add(new Finding("CodeFixException", component, diagnostic.Id, action.Title, exception.ToString()));
                    }
                }
            }
        }

        return new Result(compiles, diagnostics.Length, actionCount, findings);
    }

    private static IEnumerable<T> LoadTypes<T>(string name) => Assembly.Load(name).GetTypes()
        .Where(type => !type.IsAbstract && typeof(T).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
        .OrderBy(type => type.FullName, StringComparer.Ordinal)
        .Select(type => (T)Activator.CreateInstance(type)!);

    private static IEnumerable<CodeAction> Flatten(IEnumerable<CodeAction> actions)
    {
        foreach (var action in actions)
        {
            if (action.NestedActions.IsEmpty)
            {
                yield return action;
            }
            else
            {
                foreach (var nested in Flatten(action.NestedActions))
                {
                    yield return nested;
                }
            }
        }
    }

    private static async Task<string> SnapshotAsync(Solution solution)
    {
        var snapshot = new StringBuilder();
        foreach (var project in solution.Projects)
        {
            snapshot.AppendLine(project.Id.ToString()).AppendLine(project.Name).AppendLine(project.AssemblyName).AppendLine(project.FilePath);
            foreach (var reference in project.MetadataReferences)
            {
                snapshot.AppendLine(reference.Display);
            }

            foreach (var document in project.Documents.Cast<TextDocument>().Concat(project.AdditionalDocuments).Concat(project.AnalyzerConfigDocuments))
            {
                snapshot.AppendLine(document.Id.ToString()).AppendLine(document.Name).AppendLine(document.FilePath)
                    .AppendLine(string.Join("/", document.Folders)).AppendLine((await document.GetTextAsync().ConfigureAwait(false)).ToString());
            }
        }

        return snapshot.ToString();
    }
}
