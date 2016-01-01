// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    internal static class FixAllContextHelper
    {
        private static readonly ImmutableDictionary<string, ImmutableArray<Type>> DiagnosticAnalyzers = GetAllAnalyzers();

        private static readonly Func<CompilationWithAnalyzers, SyntaxTree, CancellationToken, Task<ImmutableArray<Diagnostic>>> GetAnalyzerSyntaxDiagnosticsAsync;
        private static readonly Func<CompilationWithAnalyzers, SemanticModel, TextSpan?, CancellationToken, Task<ImmutableArray<Diagnostic>>> GetAnalyzerSemanticDiagnosticsAsync;
        private static readonly Func<CompilationWithAnalyzers, ImmutableArray<DiagnosticAnalyzer>, CancellationToken, Task<ImmutableArray<Diagnostic>>> GetAnalyzerCompilationDiagnosticsAsync;

        static FixAllContextHelper()
        {
            Version roslynVersion = typeof(AdditionalText).GetTypeInfo().Assembly.GetName().Version;
            bool avoidGetAnalyzerDiagnosticsAsync = roslynVersion >= new Version(1, 1, 0, 0) && roslynVersion < new Version(1, 2, 0, 0);
            if (avoidGetAnalyzerDiagnosticsAsync)
            {
                var methodInfo = typeof(CompilationWithAnalyzers).GetRuntimeMethod(nameof(GetAnalyzerSyntaxDiagnosticsAsync), new[] { typeof(SyntaxTree), typeof(CancellationToken) });
                GetAnalyzerSyntaxDiagnosticsAsync = (Func<CompilationWithAnalyzers, SyntaxTree, CancellationToken, Task<ImmutableArray<Diagnostic>>>)methodInfo?.CreateDelegate(typeof(Func<CompilationWithAnalyzers, SyntaxTree, CancellationToken, Task<ImmutableArray<Diagnostic>>>));

                methodInfo = typeof(CompilationWithAnalyzers).GetRuntimeMethod(nameof(GetAnalyzerSemanticDiagnosticsAsync), new[] { typeof(SemanticModel), typeof(TextSpan?), typeof(CancellationToken) });
                GetAnalyzerSemanticDiagnosticsAsync = (Func<CompilationWithAnalyzers, SemanticModel, TextSpan?, CancellationToken, Task<ImmutableArray<Diagnostic>>>)methodInfo?.CreateDelegate(typeof(Func<CompilationWithAnalyzers, SemanticModel, TextSpan?, CancellationToken, Task<ImmutableArray<Diagnostic>>>));

                methodInfo = typeof(CompilationWithAnalyzers).GetRuntimeMethod(nameof(GetAnalyzerCompilationDiagnosticsAsync), new[] { typeof(ImmutableArray<DiagnosticAnalyzer>), typeof(CancellationToken) });
                GetAnalyzerCompilationDiagnosticsAsync = (Func<CompilationWithAnalyzers, ImmutableArray<DiagnosticAnalyzer>, CancellationToken, Task<ImmutableArray<Diagnostic>>>)methodInfo?.CreateDelegate(typeof(Func<CompilationWithAnalyzers, ImmutableArray<DiagnosticAnalyzer>, CancellationToken, Task<ImmutableArray<Diagnostic>>>));
            }
        }

        public static async Task<ImmutableDictionary<Document, ImmutableArray<Diagnostic>>> GetDocumentDiagnosticsToFixAsync(FixAllContext fixAllContext)
        {
            var allDiagnostics = ImmutableArray<Diagnostic>.Empty;
            var projectsToFix = ImmutableArray<Project>.Empty;

            var document = fixAllContext.Document;
            var project = fixAllContext.Project;

            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                if (document != null)
                {
                    var documentDiagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                    return ImmutableDictionary<Document, ImmutableArray<Diagnostic>>.Empty.SetItem(document, documentDiagnostics);
                }

                break;

            case FixAllScope.Project:
                projectsToFix = ImmutableArray.Create(project);
                allDiagnostics = await GetAllDiagnosticsAsync(fixAllContext, project).ConfigureAwait(false);
                break;

            case FixAllScope.Solution:
                projectsToFix = project.Solution.Projects
                    .Where(p => p.Language == project.Language)
                    .ToImmutableArray();

                var diagnostics = new ConcurrentDictionary<ProjectId, ImmutableArray<Diagnostic>>();
                var tasks = new Task[projectsToFix.Length];
                for (int i = 0; i < projectsToFix.Length; i++)
                {
                    fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                    var projectToFix = projectsToFix[i];
                    tasks[i] = Task.Run(
                        async () =>
                        {
                            var projectDiagnostics = await GetAllDiagnosticsAsync(fixAllContext, projectToFix).ConfigureAwait(false);
                            diagnostics.TryAdd(projectToFix.Id, projectDiagnostics);
                        }, fixAllContext.CancellationToken);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                allDiagnostics = allDiagnostics.AddRange(diagnostics.SelectMany(i => i.Value.Where(x => fixAllContext.DiagnosticIds.Contains(x.Id))));
                break;
            }

            if (allDiagnostics.IsEmpty)
            {
                return ImmutableDictionary<Document, ImmutableArray<Diagnostic>>.Empty;
            }

            return await GetDocumentDiagnosticsToFixAsync(allDiagnostics, projectsToFix, fixAllContext.CancellationToken).ConfigureAwait(false);
        }

        public static async Task<ImmutableDictionary<Project, ImmutableArray<Diagnostic>>> GetProjectDiagnosticsToFixAsync(FixAllContext fixAllContext)
        {
            var project = fixAllContext.Project;
            if (project != null)
            {
                switch (fixAllContext.Scope)
                {
                case FixAllScope.Project:
                    var diagnostics = await fixAllContext.GetProjectDiagnosticsAsync(project).ConfigureAwait(false);
                    return ImmutableDictionary<Project, ImmutableArray<Diagnostic>>.Empty.SetItem(project, diagnostics);

                case FixAllScope.Solution:
                    var projectsAndDiagnostics = new ConcurrentDictionary<Project, ImmutableArray<Diagnostic>>();
                    var options = new ParallelOptions() { CancellationToken = fixAllContext.CancellationToken };
                    Parallel.ForEach(project.Solution.Projects, options, proj =>
                    {
                        fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                        var projectDiagnosticsTask = fixAllContext.GetProjectDiagnosticsAsync(proj);
                        projectDiagnosticsTask.Wait(fixAllContext.CancellationToken);
                        var projectDiagnostics = projectDiagnosticsTask.Result;
                        if (projectDiagnostics.Any())
                        {
                            projectsAndDiagnostics.TryAdd(proj, projectDiagnostics);
                        }
                    });

                    return projectsAndDiagnostics.ToImmutableDictionary();
                }
            }

            return ImmutableDictionary<Project, ImmutableArray<Diagnostic>>.Empty;
        }

        public static async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync(Compilation compilation, CompilationWithAnalyzers compilationWithAnalyzers, ImmutableArray<DiagnosticAnalyzer> analyzers, IEnumerable<Document> documents, bool includeCompilerDiagnostics, CancellationToken cancellationToken)
        {
            if (GetAnalyzerSyntaxDiagnosticsAsync == null || GetAnalyzerSemanticDiagnosticsAsync == null)
            {
                // In everything except Roslyn 1.1, we use GetAllDiagnosticsAsync and return the result.
                var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync().ConfigureAwait(false);
                return allDiagnostics;
            }

            compilationWithAnalyzers.Compilation.GetDeclarationDiagnostics(cancellationToken);

            // Note that the following loop to obtain syntax and semantic diagnostics for each document cannot operate
            // on parallel due to our use of a single CompilationWithAnalyzers instance.
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            foreach (var document in documents)
            {
                var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                var syntaxDiagnostics = await GetAnalyzerSyntaxDiagnosticsAsync(compilationWithAnalyzers, syntaxTree, cancellationToken).ConfigureAwait(false);
                diagnostics.AddRange(syntaxDiagnostics);

                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                var semanticDiagnostics = await GetAnalyzerSemanticDiagnosticsAsync(compilationWithAnalyzers, semanticModel, default(TextSpan?), cancellationToken).ConfigureAwait(false);
                diagnostics.AddRange(semanticDiagnostics);
            }

            foreach (var analyzer in analyzers)
            {
                diagnostics.AddRange(await GetAnalyzerCompilationDiagnosticsAsync(compilationWithAnalyzers, ImmutableArray.Create(analyzer), cancellationToken).ConfigureAwait(false));
            }

            if (includeCompilerDiagnostics)
            {
                // This is the special handling for cases where code fixes operate on warnings produced by the C#
                // compiler, as opposed to being created by specific analyzers.
                var compilerDiagnostics = compilation.GetDiagnostics(cancellationToken);
                diagnostics.AddRange(compilerDiagnostics);
            }

            return diagnostics.ToImmutable();
        }

        private static ImmutableDictionary<string, ImmutableArray<Type>> GetAllAnalyzers()
        {
            Assembly assembly = typeof(NoCodeFixAttribute).GetTypeInfo().Assembly;

            var diagnosticAnalyzerType = typeof(DiagnosticAnalyzer);

            var analyzers = ImmutableDictionary.CreateBuilder<string, ImmutableArray<Type>>();

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsSubclassOf(diagnosticAnalyzerType) && !type.IsAbstract)
                {
                    Type analyzerType = type.AsType();
                    DiagnosticAnalyzer analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType);
                    foreach (var descriptor in analyzer.SupportedDiagnostics)
                    {
                        ImmutableArray<Type> types;
                        if (analyzers.TryGetValue(descriptor.Id, out types))
                        {
                            types = types.Add(analyzerType);
                        }
                        else
                        {
                            types = ImmutableArray.Create(analyzerType);
                        }

                        analyzers[descriptor.Id] = types;
                    }
                }
            }

            return analyzers.ToImmutable();
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetDiagnosticAnalyzersForContext(FixAllContext fixAllContext)
        {
            return DiagnosticAnalyzers
                .Where(x => fixAllContext.DiagnosticIds.Contains(x.Key))
                .SelectMany(x => x.Value)
                .Distinct()
                .Select(type => (DiagnosticAnalyzer)Activator.CreateInstance(type))
                .ToImmutableArray();
        }

        /// <summary>
        /// Gets all <see cref="Diagnostic"/> instances within a specific <see cref="Project"/> which are relevant to a
        /// <see cref="FixAllContext"/>.
        /// </summary>
        /// <param name="fixAllContext">The context for the Fix All operation.</param>
        /// <param name="project">The project.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. When the task completes
        /// successfully, the <see cref="Task{TResult}.Result"/> will contain the requested diagnostics.</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync(FixAllContext fixAllContext, Project project)
        {
            if (GetAnalyzerSyntaxDiagnosticsAsync == null || GetAnalyzerSemanticDiagnosticsAsync == null)
            {
                return await fixAllContext.GetAllDiagnosticsAsync(project).ConfigureAwait(false);
            }

            /*
             * The rest of this method is workaround code for issues with Roslyn 1.1...
             */

            var analyzers = GetDiagnosticAnalyzersForContext(fixAllContext);

            // Most code fixes in this project operate on diagnostics reported by analyzers in this project. However, a
            // few code fixes also operate on standard warnings produced by the C# compiler. Special handling is
            // required for the latter case since these warnings are not considered "analyzer diagnostics".
            bool includeCompilerDiagnostics = fixAllContext.DiagnosticIds.Any(x => x.StartsWith("CS", StringComparison.Ordinal));

            // Use a single CompilationWithAnalyzers for the entire operation. This allows us to use the
            // GetDeclarationDiagnostics workaround for dotnet/roslyn#7446 a single time, rather than once per document.
            var compilation = await project.GetCompilationAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, project.AnalyzerOptions, fixAllContext.CancellationToken);
            ImmutableArray<Diagnostic> diagnostics = await GetAllDiagnosticsAsync(compilation, compilationWithAnalyzers, analyzers, project.Documents, includeCompilerDiagnostics, fixAllContext.CancellationToken).ConfigureAwait(false);

            // Make sure to filter the results to the set requested for the Fix All operation, since analyzers can
            // report diagnostics with different IDs.
            diagnostics = diagnostics.RemoveAll(x => !fixAllContext.DiagnosticIds.Contains(x.Id));
            return diagnostics;
        }

        private static async Task<ImmutableDictionary<Document, ImmutableArray<Diagnostic>>> GetDocumentDiagnosticsToFixAsync(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Project> projects,
            CancellationToken cancellationToken)
        {
            var treeToDocumentMap = await GetTreeToDocumentMapAsync(projects, cancellationToken).ConfigureAwait(false);

            var builder = ImmutableDictionary.CreateBuilder<Document, ImmutableArray<Diagnostic>>();
            foreach (var documentAndDiagnostics in diagnostics.GroupBy(d => GetReportedDocument(d, treeToDocumentMap)))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var document = documentAndDiagnostics.Key;
                var diagnosticsForDocument = documentAndDiagnostics.ToImmutableArray();
                builder.Add(document, diagnosticsForDocument);
            }

            return builder.ToImmutable();
        }

        private static async Task<ImmutableDictionary<SyntaxTree, Document>> GetTreeToDocumentMapAsync(ImmutableArray<Project> projects, CancellationToken cancellationToken)
        {
            var builder = ImmutableDictionary.CreateBuilder<SyntaxTree, Document>();
            foreach (var project in projects)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var document in project.Documents)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                    builder.Add(tree, document);
                }
            }

            return builder.ToImmutable();
        }

        private static Document GetReportedDocument(Diagnostic diagnostic, ImmutableDictionary<SyntaxTree, Document> treeToDocumentsMap)
        {
            var tree = diagnostic.Location.SourceTree;
            if (tree != null)
            {
                Document document;
                if (treeToDocumentsMap.TryGetValue(tree, out document))
                {
                    return document;
                }
            }

            return null;
        }
    }
}
