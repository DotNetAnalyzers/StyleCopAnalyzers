namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The C# comment does not contain any comment text.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs whenever the code contains a C# comment which does not contain any
    /// text.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1120CommentsMustContainText : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SA1120";
        private const string Title = "Comments must contain text";
        private const string MessageFormat = "TODO: Message format";
        private const string Category = "StyleCop.CSharp.ReadabilityRules";
        private const string Description = "The C# comment does not contain any comment text.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1120.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> supportedDiagnostics =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return supportedDiagnostics;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
