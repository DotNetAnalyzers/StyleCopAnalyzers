namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Use child blocks consistently.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a documentation element contains some children which are block-level
    /// elements, but other children which are not.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1654UseChildBlocksConsistently : BlockLevelDocumentationAnalyzerBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1654UseChildBlocksConsistently"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1654";
        private const string Title = "Use child blocks consistently";
        private const string MessageFormat = "Use child blocks consistently";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The documentation for the element contains some text which is wrapped in block-level elements, and other text which is written inline.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1654.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        protected override bool ElementRequiresBlockContent(XmlElementSyntax element, SemanticModel semanticModel)
        {
            var name = element.StartTag?.Name;
            if (name == null || name.LocalName.IsMissingOrDefault())
            {
                // unrecognized
                return false;
            }

            if (name.Prefix != null)
            {
                // not a standard element
                return false;
            }

            switch (name.LocalName.ValueText)
            {
            case XmlCommentHelper.RemarksXmlTag:
            case XmlCommentHelper.NoteXmlTag:
                if (semanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(SA1653PlaceTextInParagraphs.DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
                {
                    // these elements are covered by SA1653, when enabled
                    return false;
                }

                // otherwise this diagnostic will still apply
                goto default;

            default:
                return element.Content.Any(child => IsBlockLevelNode(child, false));
            }
        }
    }
}
