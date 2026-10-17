// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Tools.SA1003FixAllBenchmark;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using StyleCop.Analyzers.SpacingRules;

/// <summary>
/// Runs comparative Fix All benchmarks for SA1003.
/// </summary>
internal static class Program
{
    private static async Task Main()
    {
        var fixer = new SA1003CodeFixProvider();
        using var dense = await CreateSolutionAsync(1, 1, 500).ConfigureAwait(false);
        using var manyDocuments = await CreateSolutionAsync(4, 100, 5).ConfigureAwait(false);
        using var sharedBoundary = await CreateSharedBoundarySolutionAsync(100).ConfigureAwait(false);
        using var nearMisses = await CreateNearMissSolutionAsync(100).ConfigureAwait(false);

        await MeasureAsync("Dense document", dense, FixAllScope.Document, fixer, WellKnownFixAllProviders.BatchFixer).ConfigureAwait(false);
        await MeasureAsync("Dense document", dense, FixAllScope.Document, fixer, fixer.GetFixAllProvider()).ConfigureAwait(false);
        await MeasureAsync("Many documents", manyDocuments, FixAllScope.Solution, fixer, WellKnownFixAllProviders.BatchFixer).ConfigureAwait(false);
        await MeasureAsync("Many documents", manyDocuments, FixAllScope.Solution, fixer, fixer.GetFixAllProvider()).ConfigureAwait(false);
        await MeasureAsync("Shared boundaries", sharedBoundary, FixAllScope.Document, fixer, WellKnownFixAllProviders.BatchFixer).ConfigureAwait(false);
        await MeasureAsync("Shared boundaries", sharedBoundary, FixAllScope.Document, fixer, fixer.GetFixAllProvider()).ConfigureAwait(false);
        await MeasureAsync("Near misses", nearMisses, FixAllScope.Document, fixer, WellKnownFixAllProviders.BatchFixer).ConfigureAwait(false);
        await MeasureAsync("Near misses", nearMisses, FixAllScope.Document, fixer, fixer.GetFixAllProvider()).ConfigureAwait(false);
    }

    private static async Task<BenchmarkSolution> CreateSolutionAsync(int projectCount, int documentsPerProject, int diagnosticsPerDocument)
    {
        var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution;
        var diagnostics = new Dictionary<DocumentId, ImmutableArray<Diagnostic>>();
        var expectedTexts = new Dictionary<DocumentId, string>();
        Document? firstDocument = null;
        var descriptor = SA1003SymbolsMustBeSpacedCorrectly.DescriptorNotFollowedByWhitespace;

        for (int projectIndex = 0; projectIndex < projectCount; projectIndex++)
        {
            var projectId = ProjectId.CreateNewId();
            solution = solution.AddProject(projectId, "Project" + projectIndex, "Project" + projectIndex, LanguageNames.CSharp);

            for (int documentIndex = 0; documentIndex < documentsPerProject; documentIndex++)
            {
                var documentId = DocumentId.CreateNewId(projectId);
                var builder = new System.Text.StringBuilder("class C { int M() {");
                for (int diagnosticIndex = 0; diagnosticIndex < diagnosticsPerDocument; diagnosticIndex++)
                {
                    builder.Append(" int v").Append(diagnosticIndex).Append(" = ~ 0;");
                }

                builder.Append(" return 0; } }\r\n");
                solution = solution.AddDocument(documentId, $"Document{documentIndex}.cs", SourceText.From(builder.ToString()));
                var document = solution.GetDocument(documentId)
                    ?? throw new InvalidOperationException("The document was not added to the benchmark solution.");
                firstDocument ??= document;

                var source = await document.GetTextAsync().ConfigureAwait(false);
                var documentTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false)
                    ?? throw new InvalidOperationException("The benchmark document has no syntax tree.");
                var documentDiagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
                var position = 0;
                var sourceText = source.ToString();
                while ((position = sourceText.IndexOf('~', position)) >= 0)
                {
                    documentDiagnostics.Add(
                        CreateDiagnostic(
                            descriptor,
                            Location.Create(documentTree, new TextSpan(position, 1)),
                            SA1003SymbolsMustBeSpacedCorrectly.RemoveAfterTag,
                            "~"));
                    position++;
                }

                diagnostics.Add(documentId, documentDiagnostics.ToImmutable());
                expectedTexts.Add(documentId, sourceText.Replace("~ ", "~"));
            }
        }

        var firstDocumentId = firstDocument?.Id ?? throw new InvalidOperationException("The benchmark must include a document.");
        var firstDocumentInSolution = solution.GetDocument(firstDocumentId)
            ?? throw new InvalidOperationException("The first benchmark document was not added to the solution.");
        return new BenchmarkSolution(workspace, firstDocumentInSolution, diagnostics, expectedTexts);
    }

    private static async Task<BenchmarkSolution> CreateSharedBoundarySolutionAsync(int expressionCount)
    {
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        var builder = new System.Text.StringBuilder("class C { int M() { int x = 0;");
        for (int i = 0; i < expressionCount; i++)
        {
            builder.Append(" var v").Append(i).Append(" = x > 0 ? x++:x++;");
        }

        builder.Append(" return x; } }\r\n");
        var source = builder.ToString();
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "Project", "Project", LanguageNames.CSharp)
            .AddDocument(documentId, "SharedBoundary.cs", SourceText.From(source));
        var document = solution.GetDocument(documentId)
            ?? throw new InvalidOperationException("The shared-boundary document was not added to the solution.");
        var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false)
            ?? throw new InvalidOperationException("The shared-boundary document has no syntax tree.");
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        var position = 0;
        while ((position = source.IndexOf("++:", position, StringComparison.Ordinal)) >= 0)
        {
            var operatorLocation = Location.Create(tree, new TextSpan(position, 2));
            var colonLocation = Location.Create(tree, new TextSpan(position + 2, 1));
            diagnostics.Add(
                CreateDiagnostic(
                    SA1003SymbolsMustBeSpacedCorrectly.DescriptorFollowedByWhitespace,
                    operatorLocation,
                    SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag,
                    "++"));
            diagnostics.Add(
                CreateDiagnostic(
                    SA1003SymbolsMustBeSpacedCorrectly.DescriptorPrecededByWhitespace,
                    colonLocation,
                    SA1003SymbolsMustBeSpacedCorrectly.InsertBeforeTag,
                    ":"));
            diagnostics.Add(
                CreateDiagnostic(
                    SA1003SymbolsMustBeSpacedCorrectly.DescriptorFollowedByWhitespace,
                    colonLocation,
                    SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag,
                    ":"));
            position += 3;
        }

        var expected = source.Replace("++:", "++ :").Replace(":x++", ": x++");
        return new BenchmarkSolution(
            workspace,
            document,
            new Dictionary<DocumentId, ImmutableArray<Diagnostic>> { [documentId] = diagnostics.ToImmutable() },
            new Dictionary<DocumentId, string> { [documentId] = expected });
    }

    private static async Task<BenchmarkSolution> CreateNearMissSolutionAsync(int nearMissCount)
    {
        var workspace = new AdhocWorkspace();
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);
        var builder = new System.Text.StringBuilder("class C { int M() { int x = 0;");
        for (int i = 0; i < nearMissCount; i++)
        {
            builder.Append(" var v").Append(i).Append(" = x > 0 ? x++ : x++;");
        }

        builder.Append(" var target = x > 0 ? x++:x++; return x; } }\r\n");
        var source = builder.ToString();
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "Project", "Project", LanguageNames.CSharp)
            .AddDocument(documentId, "NearMisses.cs", SourceText.From(source));
        var document = solution.GetDocument(documentId)
            ?? throw new InvalidOperationException("The near-miss document was not added to the solution.");
        var tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false)
            ?? throw new InvalidOperationException("The near-miss document has no syntax tree.");
        var diagnosticOffset = source.LastIndexOf("++:", StringComparison.Ordinal);
        var colonPosition = diagnosticOffset + 2;
        var diagnostics = ImmutableArray.Create(
            CreateDiagnostic(
                SA1003SymbolsMustBeSpacedCorrectly.DescriptorFollowedByWhitespace,
                Location.Create(tree, new TextSpan(diagnosticOffset, 2)),
                SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag,
                "++"),
            CreateDiagnostic(
                SA1003SymbolsMustBeSpacedCorrectly.DescriptorPrecededByWhitespace,
                Location.Create(tree, new TextSpan(colonPosition, 1)),
                SA1003SymbolsMustBeSpacedCorrectly.InsertBeforeTag,
                ":"),
            CreateDiagnostic(
                SA1003SymbolsMustBeSpacedCorrectly.DescriptorFollowedByWhitespace,
                Location.Create(tree, new TextSpan(colonPosition, 1)),
                SA1003SymbolsMustBeSpacedCorrectly.InsertAfterTag,
                ":"));
        var expected = source.Replace("++:", "++ :").Replace(":x++", ": x++");

        return new BenchmarkSolution(
            workspace,
            document,
            new Dictionary<DocumentId, ImmutableArray<Diagnostic>> { [documentId] = diagnostics },
            new Dictionary<DocumentId, string> { [documentId] = expected });
    }

    private static Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, Location location, string action, string tokenText)
    {
        return Diagnostic.Create(
            descriptor,
            location,
            ImmutableDictionary<string, string?>.Empty.Add(SA1003SymbolsMustBeSpacedCorrectly.CodeFixAction, action),
            tokenText);
    }

    private static async Task MeasureAsync(string scenario, BenchmarkSolution benchmarkSolution, FixAllScope scope, CodeFixProvider fixer, FixAllProvider fixAllProvider)
    {
        async Task RunAsync()
        {
            var context = new FixAllContext(
                benchmarkSolution.FirstDocument,
                fixer,
                scope,
                nameof(SA1003CodeFixProvider),
                fixer.FixableDiagnosticIds,
                new DiagnosticProvider(benchmarkSolution.Diagnostics),
                CancellationToken.None);
            var action = await fixAllProvider.GetFixAsync(context).ConfigureAwait(false);
            if (action == null)
            {
                await AssertOriginalTextAsync(context.Solution, benchmarkSolution.ExpectedTexts).ConfigureAwait(false);
                return;
            }

            var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
            var applyChanges = operations.OfType<ApplyChangesOperation>().Single();
            await AssertExpectedTextAsync(applyChanges.ChangedSolution, benchmarkSolution.ExpectedTexts).ConfigureAwait(false);
        }

        await RunAsync().ConfigureAwait(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var allocatedBefore = GC.GetTotalAllocatedBytes(true);
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 3; i++)
        {
            await RunAsync().ConfigureAwait(false);
        }

        stopwatch.Stop();
        var allocatedBytes = (GC.GetTotalAllocatedBytes(true) - allocatedBefore) / 3;
        Console.WriteLine(
            $"{scenario} {fixAllProvider.GetType().Name}: {stopwatch.Elapsed.TotalMilliseconds / 3:F2} ms, {allocatedBytes:N0} allocated bytes/run, {GC.GetTotalMemory(true):N0} live bytes after GC");
    }

    private static async Task AssertOriginalTextAsync(Solution solution, Dictionary<DocumentId, string> expectedTexts)
    {
        await AssertExpectedTextAsync(solution, expectedTexts).ConfigureAwait(false);
    }

    private static async Task AssertExpectedTextAsync(Solution solution, Dictionary<DocumentId, string> expectedTexts)
    {
        foreach (var expectedDocument in expectedTexts)
        {
            var document = solution.GetDocument(expectedDocument.Key)
                ?? throw new InvalidOperationException($"Document {expectedDocument.Key} is missing from the result.");
            var actualText = await document.GetTextAsync().ConfigureAwait(false);
            if (!string.Equals(expectedDocument.Value, actualText.ToString(), StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Fixed output mismatch in document {expectedDocument.Key}.");
            }
        }
    }

    private sealed class BenchmarkSolution : IDisposable
    {
        public BenchmarkSolution(AdhocWorkspace workspace, Document firstDocument, Dictionary<DocumentId, ImmutableArray<Diagnostic>> diagnostics, Dictionary<DocumentId, string> expectedTexts)
        {
            this.Workspace = workspace;
            this.FirstDocument = firstDocument;
            this.Diagnostics = diagnostics;
            this.ExpectedTexts = expectedTexts;
        }

        public AdhocWorkspace Workspace { get; }

        public Document FirstDocument { get; }

        public Dictionary<DocumentId, ImmutableArray<Diagnostic>> Diagnostics { get; }

        public Dictionary<DocumentId, string> ExpectedTexts { get; }

        public void Dispose()
        {
            this.Workspace.Dispose();
        }
    }

    private sealed class DiagnosticProvider : FixAllContext.DiagnosticProvider
    {
        private readonly Dictionary<DocumentId, ImmutableArray<Diagnostic>> diagnostics;

        public DiagnosticProvider(Dictionary<DocumentId, ImmutableArray<Diagnostic>> diagnostics)
        {
            this.diagnostics = diagnostics;
        }

        public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(this.diagnostics[document.Id]);

        public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult(Enumerable.Empty<Diagnostic>());

        public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken)
            => Task.FromResult<IEnumerable<Diagnostic>>(project.DocumentIds.SelectMany(id => this.diagnostics[id]));
    }
}
