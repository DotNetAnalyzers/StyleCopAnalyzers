// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCopTester
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.MSBuild;

    /// <summary>
    /// StyleCopTester is a tool that will analyze a solution, find diagnostics in it and will print out the number of
    /// diagnostics it could find. This is useful to easily test performance without having the overhead of visual
    /// studio running.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress +=
                (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

            SynchronizationContext previousContext = SynchronizationContext.Current;
            try
            {
                var context = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                DispatcherFrame dispatcherFrame = new DispatcherFrame();
                Task mainTask = MainAsync(args, cts.Token);
                mainTask.ContinueWith(task => dispatcherFrame.Continue = false);

                Dispatcher.PushFrame(dispatcherFrame);
                mainTask.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        private static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            // A valid call must have at least one parameter (a solution file). Optionally it can include /all or /id:SAXXXX.
            if (args.Length < 1)
            {
                PrintHelp();
            }
            else
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var analyzers = GetAllAnalyzers();

                analyzers = FilterAnalyzers(analyzers, args).ToImmutableArray();

                if (analyzers.Length == 0)
                {
                    PrintHelp();
                    return;
                }

                MSBuildWorkspace workspace = MSBuildWorkspace.Create();
                string solutionPath = args.SingleOrDefault(i => !i.StartsWith("/", StringComparison.Ordinal));
                Solution solution = workspace.OpenSolutionAsync(solutionPath, cancellationToken).Result;

                Console.WriteLine($"Loaded solution in {stopwatch.ElapsedMilliseconds}ms");

                if (!args.Contains("/nostats"))
                {
                    List<Project> csharpProjects = solution.Projects.Where(i => i.Language == LanguageNames.CSharp).ToList();

                    Console.WriteLine("Number of projects:\t\t" + csharpProjects.Count);
                    Console.WriteLine("Number of documents:\t\t" + csharpProjects.Sum(x => x.DocumentIds.Count));

                    var statistics = await GetAnalyzerStatisticsAsync(csharpProjects, cancellationToken).ConfigureAwait(true);

                    Console.WriteLine("Number of syntax nodes:\t\t" + statistics.NumberofNodes);
                    Console.WriteLine("Number of syntax tokens:\t" + statistics.NumberOfTokens);
                    Console.WriteLine("Number of syntax trivia:\t" + statistics.NumberOfTrivia);
                }

                stopwatch.Restart();

                var diagnostics = await GetAnalyzerDiagnosticsAsync(solution, solutionPath, analyzers, cancellationToken).ConfigureAwait(true);
                var allDiagnostics = diagnostics.SelectMany(i => i.Value).ToImmutableArray();

                Console.WriteLine($"Found {allDiagnostics.Length} diagnostics in {stopwatch.ElapsedMilliseconds}ms");

                foreach (var group in allDiagnostics.GroupBy(i => i.Id).OrderBy(i => i.Key, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"  {group.Key}: {group.Count()} instances");

                    // Print out analyzer diagnostics like AD0001 for analyzer exceptions
                    if (group.Key.StartsWith("AD", StringComparison.Ordinal))
                    {
                        foreach (var item in group)
                        {
                            Console.WriteLine(item);
                        }
                    }
                }

                if (args.Contains("/codefixes"))
                {
                    await TestCodeFixesAsync(stopwatch, solution, allDiagnostics, cancellationToken).ConfigureAwait(true);
                }

                if (args.Contains("/fixall"))
                {
                    await TestFixAllAsync(stopwatch, solution, diagnostics, cancellationToken).ConfigureAwait(true);
                }
            }
        }

        private static async Task TestFixAllAsync(Stopwatch stopwatch, Solution solution, ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>> diagnostics, CancellationToken cancellationToken)
        {
            Console.WriteLine("Calculating fixes");

            var codeFixers = GetAllCodeFixers().SelectMany(x => x.Value).Distinct();

            var equivalenceGroups = new List<CodeFixEquivalenceGroup>();

            foreach (var codeFixer in codeFixers)
            {
                equivalenceGroups.AddRange(await CodeFixEquivalenceGroup.CreateAsync(codeFixer, diagnostics, solution, cancellationToken).ConfigureAwait(true));
            }

            Console.WriteLine($"Found {equivalenceGroups.Count} equivalence groups.");

            Console.WriteLine("Calculating changes");

            foreach (var fix in equivalenceGroups)
            {
                try
                {
                    stopwatch.Restart();
                    Console.WriteLine($"Calculating fix for {fix.CodeFixEquivalenceKey} using {fix.FixAllProvider} for {fix.NumberOfDiagnostics} instances.");
                    await fix.GetOperationsAsync(cancellationToken).ConfigureAwait(true);
                    WriteLine($"Calculating changes completed in {stopwatch.ElapsedMilliseconds}ms. This is {fix.NumberOfDiagnostics / stopwatch.Elapsed.TotalSeconds:0.000} instances/second.", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    // Report thrown exceptions
                    WriteLine($"The fix '{fix.CodeFixEquivalenceKey}' threw an exception after {stopwatch.ElapsedMilliseconds}ms:", ConsoleColor.Yellow);
                    WriteLine(ex.ToString(), ConsoleColor.Yellow);
                }
            }
        }

        private static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static async Task TestCodeFixesAsync(Stopwatch stopwatch, Solution solution, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            Console.WriteLine("Calculating fixes");

            List<CodeAction> fixes = new List<CodeAction>();

            var codeFixers = GetAllCodeFixers();

            foreach (var item in diagnostics)
            {
                foreach (var codeFixer in codeFixers.GetValueOrDefault(item.Id, ImmutableList.Create<CodeFixProvider>()))
                {
                    fixes.AddRange(await GetFixesAsync(solution, codeFixer, item, cancellationToken).ConfigureAwait(false));
                }
            }

            Console.WriteLine($"Found {fixes.Count} potential code fixes");

            Console.WriteLine("Calculating changes");

            stopwatch.Restart();

            object lockObject = new object();

            Parallel.ForEach(fixes, fix =>
            {
                try
                {
                    fix.GetOperationsAsync(cancellationToken).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Report thrown exceptions
                    lock (lockObject)
                    {
                        WriteLine($"The fix '{fix.Title}' threw an exception:", ConsoleColor.Yellow);
                        WriteLine(ex.ToString(), ConsoleColor.Red);
                    }
                }
            });

            Console.WriteLine($"Calculating changes completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static async Task<IEnumerable<CodeAction>> GetFixesAsync(Solution solution, CodeFixProvider codeFixProvider, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            List<CodeAction> codeActions = new List<CodeAction>();

            await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => codeActions.Add(a), cancellationToken)).ConfigureAwait(false);

            return codeActions;
        }

        private static Task<Statistic> GetAnalyzerStatisticsAsync(IEnumerable<Project> projects, CancellationToken cancellationToken)
        {
            ConcurrentBag<Statistic> sums = new ConcurrentBag<Statistic>();

            Parallel.ForEach(projects.SelectMany(i => i.Documents), document =>
            {
                var documentStatistics = GetAnalyzerStatisticsAsync(document, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                sums.Add(documentStatistics);
            });

            Statistic sum = sums.Aggregate(new Statistic(0, 0, 0), (currentResult, value) => currentResult + value);
            return Task.FromResult(sum);
        }

        private static async Task<Statistic> GetAnalyzerStatisticsAsync(Document document, CancellationToken cancellationToken)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            var tokensAndNodes = root.DescendantNodesAndTokensAndSelf(descendIntoTrivia: true);

            int numberOfNodes = tokensAndNodes.Count(x => x.IsNode);
            int numberOfTokens = tokensAndNodes.Count(x => x.IsToken);
            int numberOfTrivia = root.DescendantTrivia(descendIntoTrivia: true).Count();

            return new Statistic(numberOfNodes, numberOfTokens, numberOfTrivia);
        }

        private static IEnumerable<DiagnosticAnalyzer> FilterAnalyzers(IEnumerable<DiagnosticAnalyzer> analyzers, string[] args)
        {
            bool useAll = args.Contains("/all");

            HashSet<string> ids = new HashSet<string>(args.Where(y => y.StartsWith("/id:", StringComparison.Ordinal)).Select(y => y.Substring(4)));

            foreach (var analyzer in analyzers)
            {
                if (useAll)
                {
                    yield return analyzer;
                }
                else if (ids.Count == 0)
                {
                    if (analyzer.SupportedDiagnostics.Any(i => i.IsEnabledByDefault))
                    {
                        yield return analyzer;
                    }

                    continue;
                }
                else if (analyzer.SupportedDiagnostics.Any(y => ids.Contains(y.Id)))
                {
                    yield return analyzer;
                }
            }
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetAllAnalyzers()
        {
            Assembly assembly = typeof(StyleCop.Analyzers.NoCodeFixAttribute).Assembly;

            var diagnosticAnalyzerType = typeof(DiagnosticAnalyzer);

            List<DiagnosticAnalyzer> analyzers = new List<DiagnosticAnalyzer>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(diagnosticAnalyzerType) && !type.IsAbstract)
                {
                    analyzers.Add((DiagnosticAnalyzer)Activator.CreateInstance(type));
                }
            }

            return analyzers.ToImmutableArray();
        }

        private static ImmutableDictionary<string, ImmutableList<CodeFixProvider>> GetAllCodeFixers()
        {
            Assembly assembly = typeof(StyleCop.Analyzers.SpacingRules.SA1027CodeFixProvider).Assembly;

            var codeFixProviderType = typeof(CodeFixProvider);

            Dictionary<string, ImmutableList<CodeFixProvider>> providers = new Dictionary<string, ImmutableList<CodeFixProvider>>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(codeFixProviderType) && !type.IsAbstract)
                {
                    var codeFixProvider = (CodeFixProvider)Activator.CreateInstance(type);

                    foreach (var diagnosticId in codeFixProvider.FixableDiagnosticIds)
                    {
                        providers.AddToInnerList(diagnosticId, codeFixProvider);
                    }
                }
            }

            return providers.ToImmutableDictionary();
        }

        private static ImmutableDictionary<FixAllProvider, ImmutableHashSet<string>> GetAllFixAllProviders(IEnumerable<CodeFixProvider> providers)
        {
            Dictionary<FixAllProvider, ImmutableHashSet<string>> fixAllProviders = new Dictionary<FixAllProvider, ImmutableHashSet<string>>();

            foreach (var provider in providers)
            {
                var fixAllProvider = provider.GetFixAllProvider();
                var supportedDiagnosticIds = fixAllProvider.GetSupportedFixAllDiagnosticIds(provider);
                foreach (var id in supportedDiagnosticIds)
                {
                    fixAllProviders.AddToInnerSet(fixAllProvider, id);
                }
            }

            return fixAllProviders.ToImmutableDictionary();
        }

        private static async Task<ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>>> GetAnalyzerDiagnosticsAsync(Solution solution, string solutionPath, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>> projectDiagnosticTasks = new List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                {
                    continue;
                }

                projectDiagnosticTasks.Add(new KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>(project.Id, GetProjectAnalyzerDiagnosticsAsync(analyzers, project, cancellationToken)));
            }

            ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>>.Builder projectDiagnosticBuilder = ImmutableDictionary.CreateBuilder<ProjectId, ImmutableArray<Diagnostic>>();
            foreach (var task in projectDiagnosticTasks)
            {
                projectDiagnosticBuilder.Add(task.Key, await task.Value.ConfigureAwait(false));
            }

            return projectDiagnosticBuilder.ToImmutable();
        }

        /// <summary>
        /// Returns a list of all analyzer diagnostics inside the specific project. This is an asynchronous operation.
        /// </summary>
        /// <param name="analyzers">The list of analyzers that should be used</param>
        /// <param name="project">The project that should be analyzed</param>
        /// <param name="cancellationToken">The cancellation token that the task will observe.</param>
        /// <returns>A list of diagnostics inside the project</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetProjectAnalyzerDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project, CancellationToken cancellationToken)
        {
            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, cancellationToken: cancellationToken);

            var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync().ConfigureAwait(false);

            // We want analyzer diagnostics and analyzer exceptions
            return allDiagnostics.RemoveRange(compilation.GetDiagnostics());
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: StyleCopTester [options] <Solution>");
            Console.WriteLine("Options:");
            Console.WriteLine("/all       Run all StyleCopAnalyzers analyzers, including ones that are disabled by default");
            Console.WriteLine("/nostats   Disable the display of statistics");
            Console.WriteLine("/codefixes Test single code fixes");
            Console.WriteLine("/fixall    Test fix all providers");
            Console.WriteLine("/id:<id>   Enable analyzer with diagnostic ID < id > (when this is specified, only this analyzer is enabled)");
        }
    }
}
