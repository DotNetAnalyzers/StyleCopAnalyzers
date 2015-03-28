namespace StyleCop.Analyzers
{
    using System;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// This attribute is applied to <see cref="DiagnosticAnalyzer"/>'s if will not get a code fix.
    /// Reasons for this would be:
    /// <list type="bullet">
    /// <item>Visual Studio provides a built-in code fix.</item>
    /// <item>A code fix could not provide a usefull solution.</item>
    /// </list>
    /// The reason should be provided.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NoCodeFixAttribute : System.Attribute
    {
        private readonly string reason;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoCodeFixAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason why the <see cref="DiagnosticAnalyzer"/> does not have a code fix.</param>
        public NoCodeFixAttribute(string reason)
        {
            this.reason = reason;
        }

        /// <summary>
        /// The reason why the <see cref="DiagnosticAnalyzer"/> does not have a code fix.
        /// </summary>
        public string Reason
        {
            get { return this.reason; }
        }
    }
}
