namespace StyleCop.Analyzers.DocumentationRules
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.SpacingRules;

    /// <summary>
    /// This is the base class for diagnostic analyzers which report diagnostics for inline content in documentation
    /// comments which should be placed in block element.
    /// </summary>
    public abstract class BlockLevelDocumentationAnalyzerBase : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(this.HandleXmlElementSyntax, SyntaxKind.XmlElement);
        }

        private void HandleXmlElementSyntax(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax syntax = (XmlElementSyntax)context.Node;
            if (!this.ElementRequiresBlockContent(syntax))
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
                    this.ReportDiagnosticIfRequired(context, syntax.Content, nonBlockStartIndex, nonBlockEndIndex);
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

            this.ReportDiagnosticIfRequired(context, syntax.Content, nonBlockStartIndex, nonBlockEndIndex);
        }

        private void ReportDiagnosticIfRequired(SyntaxNodeAnalysisContext context, SyntaxList<XmlNodeSyntax> content, int nonBlockStartIndex, int nonBlockEndIndex)
        {
            if (nonBlockStartIndex < 0 || nonBlockEndIndex < nonBlockStartIndex)
            {
                return;
            }

            XmlNodeSyntax startNode = content[nonBlockStartIndex];
            XmlNodeSyntax stopNode = content[nonBlockEndIndex];
            Location location = Location.Create(startNode.GetLocation().SourceTree, TextSpan.FromBounds(GetEffectiveStart(startNode), GetEffectiveEnd(stopNode)));
            context.ReportDiagnostic(Diagnostic.Create(this.SupportedDiagnostics[0], location));
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

        /// <summary>
        /// Determines if a particular <see cref="XmlElementSyntax"/> syntax node requires its content to be placed in
        /// block-level elements according to the rules of the current diagnostic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <para><see langword="true"/> if the element requires content to be placed in block-level elements.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the element allows inline content which is not placed in a block-level
        /// element.</para>
        /// </returns>
        protected abstract bool ElementRequiresBlockContent(XmlElementSyntax element);

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
