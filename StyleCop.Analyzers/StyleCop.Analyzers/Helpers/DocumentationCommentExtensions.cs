// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.SpacingRules;

    internal static class DocumentationCommentExtensions
    {
        public static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }

            foreach (var leadingTrivia in node.GetLeadingTrivia())
            {
                var structure = leadingTrivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (structure != null)
                {
                    return structure;
                }
            }

            return null;
        }

        public static XmlNodeSyntax GetFirstXmlElement(this SyntaxList<XmlNodeSyntax> content, string elementName)
        {
            return content.GetXmlElements(elementName).FirstOrDefault();
        }

        public static IEnumerable<XmlNodeSyntax> GetXmlElements(this SyntaxList<XmlNodeSyntax> content, string elementName)
        {
            foreach (XmlNodeSyntax syntax in content)
            {
                XmlEmptyElementSyntax emptyElement = syntax as XmlEmptyElementSyntax;
                if (emptyElement != null)
                {
                    if (string.Equals(elementName, emptyElement.Name.ToString(), StringComparison.Ordinal))
                    {
                        yield return emptyElement;
                    }

                    continue;
                }

                XmlElementSyntax elementSyntax = syntax as XmlElementSyntax;
                if (elementSyntax != null)
                {
                    if (string.Equals(elementName, elementSyntax.StartTag?.Name?.ToString(), StringComparison.Ordinal))
                    {
                        yield return elementSyntax;
                    }

                    continue;
                }
            }
        }

        public static T ReplaceExteriorTrivia<T>(this T node, SyntaxTrivia trivia)
            where T : XmlNodeSyntax
        {
            // Make sure to include a space after the '///' characters.
            SyntaxTrivia triviaWithSpace = SyntaxFactory.DocumentationCommentExterior(trivia.ToString() + " ");

            return node.ReplaceTrivia(
                node.DescendantTrivia(descendIntoTrivia: true).Where(i => i.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia)),
                (originalTrivia, rewrittenTrivia) => SelectExteriorTrivia(rewrittenTrivia, trivia, triviaWithSpace));
        }

        public static SyntaxList<XmlNodeSyntax> WithoutFirstAndLastNewlines(this SyntaxList<XmlNodeSyntax> summaryContent)
        {
            if (summaryContent.Count == 0)
            {
                return summaryContent;
            }

            XmlTextSyntax firstSyntax = summaryContent[0] as XmlTextSyntax;
            if (firstSyntax == null)
            {
                return summaryContent;
            }

            XmlTextSyntax lastSyntax = summaryContent[summaryContent.Count - 1] as XmlTextSyntax;
            if (lastSyntax == null)
            {
                return summaryContent;
            }

            SyntaxTokenList firstSyntaxTokens = firstSyntax.TextTokens;

            int removeFromStart;
            if (IsXmlNewLine(firstSyntaxTokens[0]))
            {
                removeFromStart = 1;
            }
            else
            {
                if (!IsXmlWhitespace(firstSyntaxTokens[0]))
                {
                    return summaryContent;
                }

                if (!IsXmlNewLine(firstSyntaxTokens[1]))
                {
                    return summaryContent;
                }

                removeFromStart = 2;
            }

            SyntaxTokenList lastSyntaxTokens = lastSyntax.TextTokens;

            int removeFromEnd;
            if (IsXmlNewLine(lastSyntaxTokens[lastSyntaxTokens.Count - 1]))
            {
                removeFromEnd = 1;
            }
            else
            {
                if (!IsXmlWhitespace(lastSyntaxTokens[lastSyntaxTokens.Count - 1]))
                {
                    return summaryContent;
                }

                if (!IsXmlNewLine(lastSyntaxTokens[lastSyntaxTokens.Count - 2]))
                {
                    return summaryContent;
                }

                removeFromEnd = 2;
            }

            for (int i = 0; i < removeFromStart; i++)
            {
                firstSyntaxTokens = firstSyntaxTokens.RemoveAt(0);
            }

            if (firstSyntax == lastSyntax)
            {
                lastSyntaxTokens = firstSyntaxTokens;
            }

            for (int i = 0; i < removeFromEnd; i++)
            {
                lastSyntaxTokens = lastSyntaxTokens.RemoveAt(lastSyntaxTokens.Count - 1);
            }

            summaryContent = summaryContent.RemoveAt(summaryContent.Count - 1);
            if (lastSyntaxTokens.Count != 0)
            {
                summaryContent = summaryContent.Add(lastSyntax.WithTextTokens(lastSyntaxTokens));
            }

            if (firstSyntax != lastSyntax)
            {
                summaryContent = summaryContent.RemoveAt(0);
                if (firstSyntaxTokens.Count != 0)
                {
                    summaryContent = summaryContent.Insert(0, firstSyntax.WithTextTokens(firstSyntaxTokens));
                }
            }

            if (summaryContent.Count > 0)
            {
                // Make sure to remove the leading trivia
                summaryContent = summaryContent.Replace(summaryContent[0], summaryContent[0].WithLeadingTrivia());

                // Remove leading spaces (between the <para> start tag and the start of the paragraph content)
                XmlTextSyntax firstTextSyntax = summaryContent[0] as XmlTextSyntax;
                if (firstTextSyntax != null && firstTextSyntax.TextTokens.Count > 0)
                {
                    SyntaxToken firstTextToken = firstTextSyntax.TextTokens[0];
                    string firstTokenText = firstTextToken.Text;
                    string trimmed = firstTokenText.TrimStart();
                    if (trimmed != firstTokenText)
                    {
                        SyntaxToken newFirstToken = SyntaxFactory.Token(
                            firstTextToken.LeadingTrivia,
                            firstTextToken.Kind(),
                            trimmed,
                            firstTextToken.ValueText.TrimStart(),
                            firstTextToken.TrailingTrivia);

                        summaryContent = summaryContent.Replace(firstTextSyntax, firstTextSyntax.ReplaceToken(firstTextToken, newFirstToken));
                    }
                }
            }

            return summaryContent;
        }

        public static bool IsXmlNewLine(this SyntaxToken node)
        {
            return node.IsKind(SyntaxKind.XmlTextLiteralNewLineToken);
        }

        public static bool IsXmlWhitespace(this SyntaxToken node)
        {
            return node.IsKind(SyntaxKind.XmlTextLiteralToken)
                && string.IsNullOrWhiteSpace(node.Text);
        }

        /// <summary>
        /// Adjust the leading and trailing trivia associated with <see cref="SyntaxKind.XmlTextLiteralNewLineToken"/>
        /// tokens to ensure the formatter properly indents the exterior trivia.
        /// </summary>
        /// <typeparam name="T">The type of syntax node.</typeparam>
        /// <param name="node">The syntax node to adjust tokens.</param>
        /// <returns>A <see cref="SyntaxNode"/> equivalent to the input <paramref name="node"/>, adjusted by moving any
        /// trailing trivia from <see cref="SyntaxKind.XmlTextLiteralNewLineToken"/> tokens to be leading trivia of the
        /// following token.</returns>
        public static T AdjustDocumentationCommentNewLineTrivia<T>(this T node)
            where T : SyntaxNode
        {
            var tokensForAdjustment =
                from token in node.DescendantTokens()
                where token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken)
                where token.HasTrailingTrivia
                let next = token.GetNextToken(includeZeroWidth: true, includeSkipped: true, includeDirectives: true, includeDocumentationComments: true)
                where !next.IsMissingOrDefault()
                select new KeyValuePair<SyntaxToken, SyntaxToken>(token, next);

            Dictionary<SyntaxToken, SyntaxToken> replacements = new Dictionary<SyntaxToken, SyntaxToken>();
            foreach (var pair in tokensForAdjustment)
            {
                replacements[pair.Key] = pair.Key.WithTrailingTrivia();
                replacements[pair.Value] = pair.Value.WithLeadingTrivia(pair.Value.LeadingTrivia.InsertRange(0, pair.Key.TrailingTrivia));
            }

            return node.ReplaceTokens(replacements.Keys, (originalToken, rewrittenToken) => replacements[originalToken]);
        }

        private static SyntaxTrivia SelectExteriorTrivia(SyntaxTrivia rewrittenTrivia, SyntaxTrivia trivia, SyntaxTrivia triviaWithSpace)
        {
            // if the trivia had a trailing space, make sure to preserve it
            if (rewrittenTrivia.ToString().EndsWith(" "))
            {
                return triviaWithSpace;
            }

            // otherwise the space is part of the leading trivia of the following token, so don't add an extra one to
            // the exterior trivia
            return trivia;
        }
    }
}
