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
    /// Use child blocks consistently across elements of the same kind.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when a documentation contains sibling elements of the same kind, where
    /// some siblings use block-level content, but others do not.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind : BlockLevelDocumentationAnalyzerBase
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1655";
        private const string Title = "Use child blocks consistently across elements of the same kind";
        private const string MessageFormat = "Use child blocks consistently across elements of the same kind";
        private const string Category = "StyleCop.CSharp.DocumentationRules";
        private const string Description = "The documentation for the element contains inline text, but the documentation for a sibling element of the same kind uses block-level elements.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1655.md";

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
                if (semanticModel.Compilation.Options.SpecificDiagnosticOptions.GetValueOrDefault(SA1654UseChildBlocksConsistently.DiagnosticId, ReportDiagnostic.Default) != ReportDiagnostic.Suppress)
                {
                    if (element.Content.Any(child => IsBlockLevelNode(child, false)))
                    {
                        // these elements are covered by SA1654, when enabled
                        return false;
                    }
                }

                break;
            }

            SyntaxList<XmlNodeSyntax> parentContent;
            XmlElementSyntax parentElement = element.Parent as XmlElementSyntax;
            if (parentElement != null)
            {
                parentContent = parentElement.Content;
            }
            else
            {
                DocumentationCommentTriviaSyntax parentSyntax = element.Parent as DocumentationCommentTriviaSyntax;
                if (parentSyntax != null)
                {
                    parentContent = parentSyntax.Content;
                }
                else
                {
                    return false;
                }
            }

            foreach (XmlElementSyntax sibling in parentContent.GetXmlElements(name.LocalName.ValueText).OfType<XmlElementSyntax>())
            {
                if (sibling == element)
                {
                    continue;
                }

                if (sibling.Content.Any(child => IsBlockLevelNode(child, false)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
