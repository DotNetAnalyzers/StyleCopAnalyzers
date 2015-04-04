namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
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
    public class SA1653PlaceTextInParagraphs : DiagnosticAnalyzer
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
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleXmlElementSyntax, SyntaxKind.XmlElement);
        }

        private static void HandleXmlElementSyntax(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax syntax = (XmlElementSyntax)context.Node;
            if (!ElementRequiresBlockContent(syntax.StartTag?.Name))
            {
                return;
            }

            int nonBlockStartIndex = -1;
            int nonBlockEndIndex = -1;
            for (int i = 0; i < syntax.Content.Count; i++)
            {
                if (IsIgnoredElement(syntax.Content[i]))
                {
                    continue;
                }

                if (IsBlockLevelNode(syntax.Content[i]))
                {
                    ReportDiagnosticIfRequired(context, syntax.Content, nonBlockStartIndex, nonBlockEndIndex);
                    nonBlockStartIndex = -1;
                    continue;
                }
                else
                {
                    nonBlockEndIndex = i;
                    if (nonBlockStartIndex < 0)
                    {
                        nonBlockStartIndex = i;
                    }
                }
            }

            ReportDiagnosticIfRequired(context, syntax.Content, nonBlockStartIndex, nonBlockEndIndex);
        }

        private static void ReportDiagnosticIfRequired(SyntaxNodeAnalysisContext context, SyntaxList<XmlNodeSyntax> content, int nonBlockStartIndex, int nonBlockEndIndex)
        {
            if (nonBlockStartIndex < 0 || nonBlockEndIndex < nonBlockStartIndex)
            {
                return;
            }

            XmlNodeSyntax startNode = content[nonBlockStartIndex];
            XmlNodeSyntax stopNode = content[nonBlockEndIndex];
            Location location = Location.Create(startNode.GetLocation().SourceTree, TextSpan.FromBounds(GetEffectiveStart(startNode), GetEffectiveEnd(stopNode)));
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location));
        }

        private static int GetEffectiveStart(XmlNodeSyntax node)
        {
            XmlTextSyntax textNode = node as XmlTextSyntax;
            if (textNode != null)
            {
                foreach (SyntaxToken textToken in textNode.TextTokens)
                {
                    if (string.IsNullOrWhiteSpace(textToken.Text))
                    {
                        continue;
                    }

                    for (int i = 0; i < textToken.Text.Length; i++)
                    {
                        if (!char.IsWhiteSpace(textToken.Text[i]))
                        {
                            return textToken.SpanStart + i;
                        }
                    }

                    return textToken.SpanStart + textToken.Text.Length;
                }
            }

            return node.SpanStart;
        }

        private static int GetEffectiveEnd(XmlNodeSyntax node)
        {
            XmlTextSyntax textNode = node as XmlTextSyntax;
            if (textNode != null)
            {
                foreach (SyntaxToken textToken in textNode.TextTokens.Reverse())
                {
                    if (string.IsNullOrWhiteSpace(textToken.Text))
                    {
                        continue;
                    }

                    for (int i = textToken.Text.Length - 1; i >= 0; i--)
                    {
                        if (!char.IsWhiteSpace(textToken.Text[i]))
                        {
                            return textToken.Span.End - (textToken.Text.Length - 1 - i);
                        }
                    }

                    return textToken.Span.End - textToken.Text.Length;
                }
            }

            return node.Span.End;
        }

        private static bool ElementRequiresBlockContent(XmlNameSyntax name)
        {
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

        private static bool IsIgnoredElement(XmlNodeSyntax node)
        {
            if (node == null)
            {
                return true;
            }

            return XmlCommentHelper.IsConsideredEmpty(node);
        }

        private static bool IsBlockLevelNode(XmlNodeSyntax node)
        {
            XmlElementSyntax elementSyntax = node as XmlElementSyntax;
            if (elementSyntax != null)
            {
                return IsBlockLevelElement(elementSyntax);
            }

            XmlEmptyElementSyntax emptyElementSyntax = node as XmlEmptyElementSyntax;
            if (emptyElementSyntax != null)
            {
                return IsBlockLevelElement(emptyElementSyntax);
            }

            // ignored elements may appear at block level
            return IsIgnoredElement(node);
        }

        private static bool IsBlockLevelElement(XmlElementSyntax element)
        {
            return IsBlockLevelName(element.StartTag?.Name);
        }

        private static bool IsBlockLevelElement(XmlEmptyElementSyntax element)
        {
            return IsBlockLevelName(element.Name);
        }

        private static bool IsBlockLevelName(XmlNameSyntax name)
        {
            if (name == null || name.LocalName.IsMissingOrDefault())
            {
                // unrecognized => allow
                return true;
            }

            if (name.Prefix != null)
            {
                // not a standard element => allow
                return true;
            }

            switch (name.LocalName.ValueText)
            {
            // certain block-level elements
            case "code":
            case "list":
            case XmlCommentHelper.NoteXmlTag:
            case "para":
                return true;

            // potential block-level elements => allow
            case XmlCommentHelper.InheritdocXmlTag:
            case "include":
            case "token":
                return true;

            // block-level HTML elements => allow for this diagnostic
            case "div":
            case "p":
                return true;

            default:
                return false;
            }
        }
    }
}
