namespace StyleCopTester
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.MSBuild;

    /// <summary>
    /// StyleCopTester is a tool that will analyze a solution, find diagnostics in it and will print out the number of
    /// diagnostics it could find. This is useful to easily test performance without having the overhead of visual
    /// studio running.
    /// </summary>
    internal class Program
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
                string solutionPath = args.SingleOrDefault(i => !i.StartsWith("/"));
                Solution solution = workspace.OpenSolutionAsync(solutionPath).Result;

                Console.WriteLine($"Loaded solution in {stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();

                var diagnostics = GetAnalyzerDiagnosticsAsync(solution, solutionPath, analyzers).Result;

                Console.WriteLine($"Found {diagnostics.Count} diagnostics in {stopwatch.ElapsedMilliseconds}ms");

                foreach (var group in diagnostics.GroupBy(i => i.Id).OrderBy(i => i.Key, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"  {group.Key}: {group.Count()} instances");

                    // Print out analyzer diagnostics like AD0001 for analyzer exceptions
                    if (group.Key.StartsWith("AD"))
                    {
                        foreach (var item in group)
                        {
                            Console.WriteLine(item);
                        }
                    }
                }
            }
        }

        private static IEnumerable<DiagnosticAnalyzer> FilterAnalyzers(IEnumerable<DiagnosticAnalyzer> analyzers, string[] args)
        {
            bool useAll = args.Contains("/all");

            HashSet<string> ids = new HashSet<string>(args.Where(y => y.StartsWith("/id:")).Select(y => y.Substring(4)));

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

        private static async Task<ImmutableList<Diagnostic>> GetAnalyzerDiagnosticsAsync(Solution solution, string solutionPath, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            List<Task<ImmutableArray<Diagnostic>>> projectDiagnosticTasks = new List<Task<ImmutableArray<Diagnostic>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
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
            Console.WriteLine("/id:<id> Enable analyzer with diagnostic ID < id > (when this is specified, only this analyzer is enabled)");
        }
    }
}
