// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using System;
    using System.IO;
    using System.Linq;
    using LibGit2Sharp;
    using Microsoft.Build.Locator;
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
        /// <returns>Zero if the tool completes successfully; otherwise, a non-zero error code.</returns>
        internal static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Path to sln file required.");
                return 1;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"Could not find solution file: {Path.GetFullPath(args[0])}");
                return 1;
            }

            MSBuildLocator.RegisterDefaults();
            SolutionReader reader = SolutionReader.CreateAsync(args[0]).Result;

            var diagnostics = reader.GetDiagnosticsAsync().Result;

            diagnostics = diagnostics.Sort((a, b) => a.Id.CompareTo(b.Id));

            Commit commit;
            string commitId;

            using (Repository repository = new Repository(Path.GetDirectoryName(args[0])))
            {
                commitId = repository.Head.Tip.Sha;
                commit = repository.Head.Tip;

                var output = new
                {
                    diagnostics,
                    git = new
                    {
                        commit.Sha,
                        commit.Message,
                        commit.Author,
                        commit.Committer,
                        Parents = commit.Parents.Select(x => x.Sha),
                    },
                };

                Console.WriteLine(JsonConvert.SerializeObject(output, Formatting.Indented));
            }

            return 0;
        }
    }
}
