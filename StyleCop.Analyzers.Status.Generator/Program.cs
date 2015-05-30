namespace StyleCop.Analyzers.Status.Generator
{
    using System;
    using System.IO;
    using LibGit2Sharp;
    using Newtonsoft.Json;

    /// <summary>
    /// The starting point of this application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The starting point of this application.
        /// </summary>
        /// <param name="args">The command line parameters.</param>
        internal static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Path to sln file required.");
                return;
            }

            SolutionReader reader = SolutionReader.CreateAsync(args[0]).Result;

            var diagnostics = reader.GetDiagnosticsAsync().Result;

            string commitId;

            using (Repository repository = new Repository(Path.GetDirectoryName(args[0])))
            {
                commitId = repository.Head.Tip.Sha;
            }

            var output = new
            {
                diagnostics,
                commitId
            };

            Console.WriteLine(JsonConvert.SerializeObject(output));
        }
    }
}
