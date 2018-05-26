// Copyright (c) Dennis Fischer. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    using System;
    using System.IO;
    using System.Linq;
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
                        Parents = commit.Parents.Select(x => x.Sha)
                    }
                };

                Console.WriteLine(JsonConvert.SerializeObject(output));
            }
        }
    }
}
