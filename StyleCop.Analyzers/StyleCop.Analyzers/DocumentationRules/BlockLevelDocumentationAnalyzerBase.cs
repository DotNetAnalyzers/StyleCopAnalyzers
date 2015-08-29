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

        /// <summary>
        /// Determines if a particular node is a block-level documentation element.
        /// </summary>
        /// <param name="node">The syntax node to examine.</param>
        /// <param name="includePotentialElements">
        /// <para><see langword="true"/> to only check for elements that are always block level elements.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> to check for elements that are allowed to be block-level or inline elements,
        /// including but not limited to XML comments represented by a <see cref="XmlCommentSyntax"/> node.</para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified node is a block-level element with respect to the
        /// <paramref name="includePotentialElements"/> option; otherwise, <see langword="false"/>.
        /// </returns>
        protected static bool IsBlockLevelNode(XmlNodeSyntax node, bool includePotentialElements)
        {
            XmlElementSyntax elementSyntax = node as XmlElementSyntax;
            if (elementSyntax != null)
            {
                return IsBlockLevelElement(elementSyntax, includePotentialElements);
            }

            XmlEmptyElementSyntax emptyElementSyntax = node as XmlEmptyElementSyntax;
            if (emptyElementSyntax != null)
            {
                return IsBlockLevelElement(emptyElementSyntax, includePotentialElements);
            }

            if (!includePotentialElements)
            {
                // Caller is only interested in elements which are *certainly* block-level
                return false;
            }

            // ignored elements may appear at block level
            return IsIgnoredElement(node);
        }

        /// <summary>
        /// Determines if a particular <see cref="XmlElementSyntax"/> syntax node requires its content to be placed in
        /// block-level elements according to the rules of the current diagnostic.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> for the current analysis context.</param>
        /// <returns>
        /// <para><see langword="true"/> if the element requires content to be placed in block-level elements.</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if the element allows inline content which is not placed in a block-level
        /// element.</para>
        /// </returns>
        protected abstract bool ElementRequiresBlockContent(XmlElementSyntax element, SemanticModel semanticModel);

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

        private static bool IsIgnoredElement(XmlNodeSyntax node)
        {
            if (node == null)
            {
                return true;
            }

            return XmlCommentHelper.IsConsideredEmpty(node);
        }

        private static bool IsBlockLevelElement(XmlElementSyntax element, bool includePotentialElements)
        {
            return IsBlockLevelName(element.StartTag?.Name, includePotentialElements);
        }

        private static bool IsBlockLevelElement(XmlEmptyElementSyntax element, bool includePotentialElements)
        {
            return IsBlockLevelName(element.Name, includePotentialElements);
        }

        private static bool IsBlockLevelName(XmlNameSyntax name, bool includePotentialElements)
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

        private void HandleXmlElementSyntax(SyntaxNodeAnalysisContext context)
        {
            XmlElementSyntax syntax = (XmlElementSyntax)context.Node;
            if (!this.ElementRequiresBlockContent(syntax, context.SemanticModel))
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

                if (IsBlockLevelNode(syntax.Content[i], true))
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
    }
}
