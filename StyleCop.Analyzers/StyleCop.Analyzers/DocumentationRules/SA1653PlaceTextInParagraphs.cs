namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// Place text in paragraphs.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a &lt;remarks&gt; or &lt;note&gt; documentation element contains
    /// content which is not wrapped in a block-level element.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1653PlaceTextInParagraphs : BlockLevelDocumentationAnalyzerBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1653PlaceTextInParagraphs"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1653";
        private const string Title = "Place text in paragraphs";
        private const string MessageFormat = "Place text in paragraphs";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The documentation for the element contains text which is not placed in paragraphs.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1653.md";

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
                return true;

            default:
                return false;
            }
        }
    }
}
