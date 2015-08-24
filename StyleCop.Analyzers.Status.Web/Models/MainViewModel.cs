namespace StyleCop.Analyzers.Status.Web.Models
{
    using System.Collections.Generic;
    using Generator;
    using LibGit2Sharp;

    public class MainViewModel
    {
        public string CommitId { get; set; }

        public IEnumerable<StyleCopDiagnostic> Diagnostics { get; set; }

        public GitInformation Git { get; set; }

        public class GitInformation
        {
            public string Sha { get; set; }

            public string Message { get; set; }

            public Signature Author { get; set; }

            public Signature Committer { get; set; }

            public string[] Parents { get; set; }
        }
    }
}