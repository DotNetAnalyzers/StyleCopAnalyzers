// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCopTester
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.MSBuild;
    using File = System.IO.File;
    using Path = System.IO.Path;

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

            // Since Console apps do not have a SynchronizationContext, we're leveraging the built-in support
            // in WPF to pump the messages via the Dispatcher.
            // See the following for additional details:
            //   http://blogs.msdn.com/b/pfxteam/archive/2012/01/21/10259307.aspx
            //   https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1362
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
                bool applyChanges = args.Contains("/apply");
                if (applyChanges)
                {
                    if (!args.Contains("/fixall"))
                    {
                        Console.Error.WriteLine("Error: /apply can only be used with /fixall");
                        return;
                    }
                }

                MSBuildLocator.RegisterDefaults();

                Stopwatch stopwatch = Stopwatch.StartNew();
                var analyzers = GetAllAnalyzers();

                analyzers = FilterAnalyzers(analyzers, args).ToImmutableArray();

                if (analyzers.Length == 0)
                {
                    PrintHelp();
                    return;
                }

                var properties = new Dictionary<string, string>
                {
                    // This property ensures that XAML files will be compiled in the current AppDomain
                    // rather than a separate one. Any tasks isolated in AppDomains or tasks that create
                    // AppDomains will likely not work due to https://github.com/Microsoft/MSBuildLocator/issues/16.
                    { "AlwaysCompileMarkupFilesInSeparateDomain", bool.FalseString },
                };

                MSBuildWorkspace workspace = MSBuildWorkspace.Create();
                string solutionPath = args.SingleOrDefault(i => !i.StartsWith("/", StringComparison.Ordinal));
                Solution solution = await workspace.OpenSolutionAsync(solutionPath, cancellationToken: cancellationToken).ConfigureAwait(false);

                Console.WriteLine($"Loaded solution in {stopwatch.ElapsedMilliseconds}ms");

                if (args.Contains("/stats"))
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

                bool force = args.Contains("/force");

                var diagnostics = await GetAnalyzerDiagnosticsAsync(solution, analyzers, force, cancellationToken).ConfigureAwait(true);
                var allDiagnostics = diagnostics.SelectMany(i => i.Value).ToImmutableArray();

                Console.WriteLine($"Found {allDiagnostics.Length} diagnostics in {stopwatch.ElapsedMilliseconds}ms");

                bool testDocuments = args.Contains("/editperf") || args.Any(arg => arg.StartsWith("/editperf:"));
                if (testDocuments)
                {
                    Func<string, bool> documentMatch = _ => true;
                    string matchArg = args.FirstOrDefault(arg => arg.StartsWith("/editperf:"));
                    if (matchArg != null)
                    {
                        Regex expression = new Regex(matchArg.Substring("/editperf:".Length), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        documentMatch = documentPath => expression.IsMatch(documentPath);
                    }

                    int iterations = 10;
                    string iterationsArg = args.FirstOrDefault(arg => arg.StartsWith("/edititer:"));
                    if (iterationsArg != null)
                    {
                        iterations = int.Parse(iterationsArg.Substring("/edititer:".Length));
                    }

                    var projectPerformance = new Dictionary<ProjectId, double>();
                    var documentPerformance = new Dictionary<DocumentId, DocumentAnalyzerPerformance>();
                    foreach (var projectId in solution.ProjectIds)
                    {
                        Project project = solution.GetProject(projectId);
                        if (project.Language != LanguageNames.CSharp)
                        {
                            continue;
                        }

                        foreach (var documentId in project.DocumentIds)
                        {
                            var document = project.GetDocument(documentId);
                            if (!documentMatch(document.FilePath))
                            {
                                continue;
                            }

                            var currentDocumentPerformance = await TestDocumentPerformanceAsync(analyzers, project, documentId, iterations, force, cancellationToken).ConfigureAwait(false);
                            Console.WriteLine($"{document.FilePath ?? document.Name}: {currentDocumentPerformance.EditsPerSecond:0.00}");
                            documentPerformance.Add(documentId, currentDocumentPerformance);
                        }

                        double sumOfDocumentAverages = documentPerformance.Where(x => x.Key.ProjectId == projectId).Sum(x => x.Value.EditsPerSecond);
                        double documentCount = documentPerformance.Where(x => x.Key.ProjectId == projectId).Count();
                        if (documentCount > 0)
                        {
                            projectPerformance[project.Id] = sumOfDocumentAverages / documentCount;
                        }
                    }

                    var slowestFiles = documentPerformance.OrderBy(pair => pair.Value.EditsPerSecond).GroupBy(pair => pair.Key.ProjectId);
                    Console.WriteLine("Slowest files in each project:");
                    foreach (var projectGroup in slowestFiles)
                    {
                        Console.WriteLine($"  {solution.GetProject(projectGroup.Key).Name}");
                        foreach (var pair in projectGroup.Take(5))
                        {
                            var document = solution.GetDocument(pair.Key);
                            Console.WriteLine($"    {document.FilePath ?? document.Name}: {pair.Value.EditsPerSecond:0.00}");
                        }
                    }

                    foreach (var projectId in solution.ProjectIds)
                    {
                        double averageEditsInProject;
                        if (!projectPerformance.TryGetValue(projectId, out averageEditsInProject))
                        {
                            continue;
                        }

                        Project project = solution.GetProject(projectId);
                        Console.WriteLine($"{project.Name} ({project.DocumentIds.Count} documents): {averageEditsInProject:0.00} edits per second");
                    }
                }

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

                string logArgument = args.FirstOrDefault(x => x.StartsWith("/log:"));
                if (logArgument != null)
                {
                    string fileName = logArgument.Substring(logArgument.IndexOf(':') + 1);
                    WriteDiagnosticResults(diagnostics.SelectMany(i => i.Value.Select(j => Tuple.Create(i.Key, j))).ToImmutableArray(), fileName);
                }

                if (args.Contains("/codefixes"))
                {
                    await TestCodeFixesAsync(stopwatch, solution, allDiagnostics, cancellationToken).ConfigureAwait(true);
                }

                if (args.Contains("/fixall"))
                {
                    await TestFixAllAsync(stopwatch, solution, diagnostics, applyChanges, cancellationToken).ConfigureAwait(true);
                }
            }
        }

        private static async Task<DocumentAnalyzerPerformance> TestDocumentPerformanceAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project, DocumentId documentId, int iterations, bool force, CancellationToken cancellationToken)
        {
            var supportedDiagnosticsSpecificOptions = new Dictionary<string, ReportDiagnostic>();
            if (force)
            {
                foreach (var analyzer in analyzers)
                {
                    foreach (var diagnostic in analyzer.SupportedDiagnostics)
                    {
                        // make sure the analyzers we are testing are enabled
                        supportedDiagnosticsSpecificOptions[diagnostic.Id] = ReportDiagnostic.Default;
                    }
                }
            }

            // Report exceptions during the analysis process as errors
            supportedDiagnosticsSpecificOptions.Add("AD0001", ReportDiagnostic.Error);

            // update the project compilation options
            var modifiedSpecificDiagnosticOptions = supportedDiagnosticsSpecificOptions.ToImmutableDictionary().SetItems(project.CompilationOptions.SpecificDiagnosticOptions);
            var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var processedProject = project.WithCompilationOptions(modifiedCompilationOptions);

                Compilation compilation = await processedProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
                CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, new CompilationWithAnalyzersOptions(processedProject.AnalyzerOptions, null, true, false));

                SyntaxTree tree = await project.GetDocument(documentId).GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                await compilationWithAnalyzers.GetAnalyzerSyntaxDiagnosticsAsync(tree, cancellationToken).ConfigureAwait(false);
                await compilationWithAnalyzers.GetAnalyzerSemanticDiagnosticsAsync(compilation.GetSemanticModel(tree), null, cancellationToken).ConfigureAwait(false);
            }

            return new DocumentAnalyzerPerformance(iterations / stopwatch.Elapsed.TotalSeconds);
        }

        private static void WriteDiagnosticResults(ImmutableArray<Tuple<ProjectId, Diagnostic>> diagnostics, string fileName)
        {
            var orderedDiagnostics =
                diagnostics
                .OrderBy(i => i.Item2.Id)
                .ThenBy(i => i.Item2.Location.SourceTree?.FilePath, StringComparer.OrdinalIgnoreCase)
                .ThenBy(i => i.Item2.Location.SourceSpan.Start)
                .ThenBy(i => i.Item2.Location.SourceSpan.End);

            var uniqueLines = new HashSet<string>();
            StringBuilder completeOutput = new StringBuilder();
            StringBuilder uniqueOutput = new StringBuilder();
            foreach (var diagnostic in orderedDiagnostics)
            {
                string message = diagnostic.Item2.ToString();
                string uniqueMessage = $"{diagnostic.Item1}: {diagnostic.Item2}";
                completeOutput.AppendLine(message);
                if (uniqueLines.Add(uniqueMessage))
                {
                    uniqueOutput.AppendLine(message);
                }
            }

            string directoryName = Path.GetDirectoryName(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string uniqueFileName = Path.Combine(directoryName, $"{fileNameWithoutExtension}-Unique{extension}");

            File.WriteAllText(fileName, completeOutput.ToString(), Encoding.UTF8);
            File.WriteAllText(uniqueFileName, uniqueOutput.ToString(), Encoding.UTF8);
        }

        private static async Task TestFixAllAsync(Stopwatch stopwatch, Solution solution, ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>> diagnostics, bool applyChanges, CancellationToken cancellationToken)
        {
            Console.WriteLine("Calculating fixes");

            var codeFixers = GetAllCodeFixers().SelectMany(x => x.Value).Distinct();

            var equivalenceGroups = new List<CodeFixEquivalenceGroup>();

            foreach (var codeFixer in codeFixers)
            {
                equivalenceGroups.AddRange(await CodeFixEquivalenceGroup.CreateAsync(codeFixer, diagnostics, solution, cancellationToken).ConfigureAwait(true));
            }

            Console.WriteLine($"Found {equivalenceGroups.Count} equivalence groups.");
            if (applyChanges && equivalenceGroups.Count > 1)
            {
                Console.Error.WriteLine("/apply can only be used with a single equivalence group.");
                return;
            }

            Console.WriteLine("Calculating changes");

            foreach (var fix in equivalenceGroups)
            {
                try
                {
                    stopwatch.Restart();
                    Console.WriteLine($"Calculating fix for {fix.CodeFixEquivalenceKey} using {fix.FixAllProvider} for {fix.NumberOfDiagnostics} instances.");
                    var operations = await fix.GetOperationsAsync(cancellationToken).ConfigureAwait(true);
                    if (applyChanges)
                    {
                        var applyOperations = operations.OfType<ApplyChangesOperation>().ToList();
                        if (applyOperations.Count > 1)
                        {
                            Console.Error.WriteLine("/apply can only apply a single code action operation.");
                        }
                        else if (applyOperations.Count == 0)
                        {
                            Console.WriteLine("No changes were found to apply.");
                        }
                        else
                        {
                            applyOperations[0].Apply(solution.Workspace, cancellationToken);
                        }
                    }

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

            var analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(diagnosticAnalyzerType) && !type.IsAbstract)
                {
                    analyzers.Add((DiagnosticAnalyzer)Activator.CreateInstance(type));
                }
            }

            return analyzers.ToImmutable();
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

        private static async Task<ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>>> GetAnalyzerDiagnosticsAsync(Solution solution, ImmutableArray<DiagnosticAnalyzer> analyzers, bool force, CancellationToken cancellationToken)
        {
            List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>> projectDiagnosticTasks = new List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                {
                    continue;
                }

                projectDiagnosticTasks.Add(new KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>(project.Id, GetProjectAnalyzerDiagnosticsAsync(analyzers, project, force, cancellationToken)));
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
        /// <param name="analyzers">The list of analyzers that should be used.</param>
        /// <param name="project">The project that should be analyzed.</param>
        /// <param name="force"><see langword="true"/> to force the analyzers to be enabled; otherwise,
        /// <see langword="false"/> to use the behavior configured for the specified <paramref name="project"/>.</param>
        /// <param name="cancellationToken">The cancellation token that the task will observe.</param>
        /// <returns>A list of diagnostics inside the project.</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetProjectAnalyzerDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project, bool force, CancellationToken cancellationToken)
        {
            var supportedDiagnosticsSpecificOptions = new Dictionary<string, ReportDiagnostic>();
            if (force)
            {
                foreach (var analyzer in analyzers)
                {
                    foreach (var diagnostic in analyzer.SupportedDiagnostics)
                    {
                        // make sure the analyzers we are testing are enabled
                        supportedDiagnosticsSpecificOptions[diagnostic.Id] = ReportDiagnostic.Default;
                    }
                }
            }

            // Report exceptions during the analysis process as errors
            supportedDiagnosticsSpecificOptions.Add("AD0001", ReportDiagnostic.Error);

            // update the project compilation options
            var modifiedSpecificDiagnosticOptions = supportedDiagnosticsSpecificOptions.ToImmutableDictionary().SetItems(project.CompilationOptions.SpecificDiagnosticOptions);
            var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);
            var processedProject = project.WithCompilationOptions(modifiedCompilationOptions);

            Compilation compilation = await processedProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, new CompilationWithAnalyzersOptions(processedProject.AnalyzerOptions, null, true, false));

            var diagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
            return diagnostics;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: StyleCopTester [options] <Solution>");
            Console.WriteLine("Options:");
            Console.WriteLine("/all       Run all StyleCopAnalyzers analyzers, including ones that are disabled by default");
            Console.WriteLine("/stats     Display statistics of the solution");
            Console.WriteLine("/codefixes Test single code fixes");
            Console.WriteLine("/fixall    Test fix all providers");
            Console.WriteLine("/id:<id>   Enable analyzer with diagnostic ID <id> (when this is specified, only this analyzer is enabled)");
            Console.WriteLine("/apply     Write code fix changes back to disk");
            Console.WriteLine("/force     Force an analyzer to be enabled, regardless of the configured rule set(s) for the solution");
            Console.WriteLine("/editperf[:<match>]     Test the incremental performance of analyzers to simulate the behavior of editing files. If <match> is specified, only files matching this regular expression are evaluated for editor performance.");
            Console.WriteLine("/edititer:<iterations>  Specifies the number of iterations to use for testing documents with /editperf. When this is not specified, the default value is 10.");
        }

        private readonly struct DocumentAnalyzerPerformance
        {
            public DocumentAnalyzerPerformance(double editsPerSecond)
            {
                this.EditsPerSecond = editsPerSecond;
            }

            public double EditsPerSecond
            {
                get;
            }
        }
    }
}
