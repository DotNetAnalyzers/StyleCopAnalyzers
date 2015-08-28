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
                Solution solution = workspace.OpenSolutionAsync(solutionPath).Result;

                Console.WriteLine($"Loaded solution in {stopwatch.ElapsedMilliseconds}ms");

                if (!args.Contains("/nostats"))
                {
                    List<Project> csharpProjects = solution.Projects.Where(i => i.Language == LanguageNames.CSharp).ToList();

                    Console.WriteLine("Number of projects:\t\t" + csharpProjects.Count);
                    Console.WriteLine("Number of documents:\t\t" + csharpProjects.Sum(x => x.DocumentIds.Count));

                    var statistics = GetAnalyzerStatistics(csharpProjects);

                    Console.WriteLine("Number of syntax nodes:\t\t" + statistics.NumberofNodes);
                    Console.WriteLine("Number of syntax tokens:\t" + statistics.NumberOfTokens);
                    Console.WriteLine("Number of syntax trivia:\t" + statistics.NumberOfTrivia);
                }

                stopwatch.Restart();

                var diagnostics = GetAnalyzerDiagnosticsAsync(solution, solutionPath, analyzers).Result;

                Console.WriteLine($"Found {diagnostics.Count} diagnostics in {stopwatch.ElapsedMilliseconds}ms");

                foreach (var group in diagnostics.GroupBy(i => i.Id).OrderBy(i => i.Key, StringComparer.OrdinalIgnoreCase))
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

                Console.WriteLine("Calculating fixes");

                List<CodeAction> fixes = new List<CodeAction>();

                var codeFixers = GetAllCodeFixers();

                foreach (var item in diagnostics)
                {
                    foreach (var codeFixer in codeFixers.GetValueOrDefault(item.Id, ImmutableList.Create<CodeFixProvider>()))
                    {
                        fixes.AddRange(GetFixesAsync(solution, codeFixer, item).GetAwaiter().GetResult());
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
                        fix.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        // Report thrown exceptions
                        lock (lockObject)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"The fix '{fix.Title} 'threw an exception:");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex);
                            Console.ResetColor();
                        }
                    }
                });

                Console.WriteLine($"Calculating changes completed in {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static async Task<IEnumerable<CodeAction>> GetFixesAsync(Solution solution, CodeFixProvider codeFixProvider, Diagnostic diagnostic)
        {
            List<CodeAction> codeActions = new List<CodeAction>();

            await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => codeActions.Add(a), CancellationToken.None));

            return codeActions;
        }

        private static Statistic GetAnalyzerStatistics(IEnumerable<Project> projects)
        {
            ConcurrentBag<Statistic> sums = new ConcurrentBag<Statistic>();

            Parallel.ForEach(projects.SelectMany(i => i.Documents), document =>
            {
                var documentStatistics = GetAnalyzerStatistics(document).ConfigureAwait(false).GetAwaiter().GetResult();
                sums.Add(documentStatistics);
            });

            Statistic sum = sums.Aggregate(new Statistic(0, 0, 0), (currentResult, value) => currentResult + value);
            return sum;
        }

        private static async Task<Statistic> GetAnalyzerStatistics(Document document)
        {
            SyntaxTree tree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);

            SyntaxNode root = await tree.GetRootAsync().ConfigureAwait(false);

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
            Assembly assembly = typeof(StyleCop.Analyzers.NoCodeFixAttribute).Assembly;

            var diagnosticAnalyzerType = typeof(CodeFixProvider);

            Dictionary<string, ImmutableList<CodeFixProvider>> providers = new Dictionary<string, ImmutableList<CodeFixProvider>>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(diagnosticAnalyzerType) && !type.IsAbstract)
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

        private static async Task<ImmutableList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Solution solution, string solutionPath, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            List<Task<ImmutableArray<Diagnostic>>> projectDiagnosticTasks = new List<Task<ImmutableArray<Diagnostic>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                {
                    continue;
                }

                projectDiagnosticTasks.Add(GetProjectAnalyzerDiagnostics(analyzers, project));
            }

            ImmutableList<Diagnostic>.Builder diagnosticBuilder = ImmutableList.CreateBuilder<Diagnostic>();
            foreach (var task in projectDiagnosticTasks)
            {
                diagnosticBuilder.AddRange(await task.ConfigureAwait(false));
            }

            return diagnosticBuilder.ToImmutable();
        }

        /// <summary>
        /// Returns a list of all analyzer diagnostics inside the specific project. This is an asynchronous operation.
        /// </summary>
        /// <param name="analyzers">The list of analyzers that should be used</param>
        /// <param name="project">The project that should be analyzed</param>
        /// <returns>A list of diagnostics inside the project</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetProjectAnalyzerDiagnostics(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project)
        {
            Compilation compilation = await project.GetCompilationAsync().ConfigureAwait(false);
            CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);

            var allDiagnostics = await compilationWithAnalyzers.GetAllDiagnosticsAsync().ConfigureAwait(false);

            // We want analyzer diagnostics and analyzer exceptions
            return allDiagnostics.RemoveRange(compilation.GetDiagnostics());
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: StyleCopTester [options] <Solution>");
            Console.WriteLine("Options:");
            Console.WriteLine("/all     Run all StyleCopAnalyzers analyzers, including ones that are disabled by default");
            Console.WriteLine("/nostats Disable the display of statistics");
            Console.WriteLine("/id:<id> Enable analyzer with diagnostic ID < id > (when this is specified, only this analyzer is enabled)");
        }
    }
}
