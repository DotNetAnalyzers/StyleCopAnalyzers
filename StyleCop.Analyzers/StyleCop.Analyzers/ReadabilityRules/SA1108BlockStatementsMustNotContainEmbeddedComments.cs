namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A C# statement contains a comment between the declaration of the statement and the opening curly bracket of the
    /// statement.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code contains a comment in between the declaration and the
    /// opening curly bracket. For example:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// // Make sure x does not equal y
    /// {
    /// }
    /// </code>
    /// <para>The comment can legally be placed above the statement, or within the body of the statement:</para>
    /// <code language="csharp">
    /// // Make sure x does not equal y
    /// if (x != y)
    /// {
    /// }
    ///
    /// if (x != y)
    /// {
    ///     // Make sure x does not equal y
    /// }
    /// </code>
    /// <para>If the comment is being used to comment out a line of code, begin the comment with four forward slashes
    /// rather than two:</para>
    /// <code language="csharp">
    /// if (x != y)
    /// ////if (x == y)
    /// {
    /// }
    /// </code>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1108BlockStatementsMustNotContainEmbeddedComments : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1108";
        internal const string Title = "Block statements must not contain embedded comments";
        internal const string MessageFormat = "TODO: Message format";
        internal const string Category = "StyleCop.CSharp.ReadabilityRules";
        internal const string Description = "A C# statement contains a comment between the declaration of the statement and the opening curly bracket of the statement.";
        internal const string HelpLink = "http://www.stylecop.com/docs/SA1108.html";

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
