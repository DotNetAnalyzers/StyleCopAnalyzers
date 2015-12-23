// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class FixAllContextHelper
    {
        private static readonly ImmutableDictionary<string, ImmutableArray<Type>> DiagnosticAnalyzers = GetAllAnalyzers();

        public static async Task<ImmutableDictionary<Document, ImmutableArray<Diagnostic>>> GetDocumentDiagnosticsToFixAsync(FixAllContext fixAllContext)
        {
            var allDiagnostics = ImmutableArray<Diagnostic>.Empty;
            var projectsToFix = ImmutableArray<Project>.Empty;

            var document = fixAllContext.Document;
            var project = fixAllContext.Project;
            var analyzers = GetDiagnosticAnalyzersForContext(fixAllContext);

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
                allDiagnostics = await GetAllDiagnosticsAsync(fixAllContext, project, analyzers).ConfigureAwait(false);
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
                            var projectDiagnostics = await GetAllDiagnosticsAsync(fixAllContext, projectToFix, analyzers).ConfigureAwait(false);
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

        private static async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync(FixAllContext fixAllContext, Project project, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            bool includeCompilerDiagnostics = fixAllContext.DiagnosticIds.Any(x => x.StartsWith("CS", StringComparison.Ordinal));
            var compilation = await project.GetCompilationAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

            var diagnostics = ImmutableArray<Diagnostic>.Empty;
            if (analyzers.Any())
            {
                var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, project.AnalyzerOptions, fixAllContext.CancellationToken);
                diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
            }

            if (includeCompilerDiagnostics)
            {
                var compilerDiagnostics = compilation.GetDiagnostics(fixAllContext.CancellationToken);
                diagnostics = diagnostics.AddRange(compilerDiagnostics);
            }

            diagnostics = diagnostics.Where(x => fixAllContext.DiagnosticIds.Contains(x.Id)).ToImmutableArray();
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
