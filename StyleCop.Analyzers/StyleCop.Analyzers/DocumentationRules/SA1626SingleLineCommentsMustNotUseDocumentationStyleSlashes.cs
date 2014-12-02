namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# code contains a single-line comment which begins with three forward slashes in a row.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a single-line comment which begins with three
    /// slashes. Comments beginning with three slashes are reserved for XML documentation headers. Single-line comments
    /// should begin with only two slashes. When commenting out lines of code, it is advisable to begin the comment with
    /// four slashes to differentiate it from normal comments. For example:</para>
    ///
    /// <code language="csharp">
    ///     /// &lt;summary&gt;
    ///     /// Joins a first name and a last name together into a single string.
    ///     /// &lt;/summary&gt;
    ///     /// &lt;param name="firstName"&gt;Part of the name.&lt;/param&gt;
    ///     /// &lt;param name="lastName"&gt;Part of the name.&lt;/param&gt;
    ///     /// &lt;returns&gt;The joined names.&lt;/returns&gt;
    ///     public string JoinNames(string firstName, string lastName)
    ///     {
    /// A legal comment beginning with two slashes:
    ///         // Join the names together.
    ///         string fullName = firstName + " " + lastName;
    ///
    /// An illegal comment beginning with three slashes:
    ///         /// Trim the name.
    ///         fullName = fullName.Trim();
    ///
    /// A line of commented-out code beginning with four slashes:
    ///         ////fullName = asfd; 
    ///
    ///         return fullName;
    ///     }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1626SingleLineCommentsMustNotUseDocumentationStyleSlashes : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1626";
        internal const string Title = "The C# code contains a single-line comment which begins with three forward slashes in a row.";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.DocumentationRules";
        internal const string Description = "The C# code contains a single-line comment which begins with three forward slashes in a row.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1626.html";

        public static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return _supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
