namespace StyleCop.Analyzers.Test.Helpers
{
    /// <summary>
    /// Indicates that a specific test is related to an open issue.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class OpenIssueAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIssueAttribute"/> class.
        /// </summary>
        /// <param name="issue">A link to the open issue.</param>
        public OpenIssueAttribute(string issue)
        {
            this.Issue = issue;
        }

        public string Issue { get; }
    }
}
