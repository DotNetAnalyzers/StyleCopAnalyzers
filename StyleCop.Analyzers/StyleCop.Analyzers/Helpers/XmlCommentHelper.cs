// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers.ObjectPools;

    /// <summary>
    /// Provides helper methods to work with XML comments.
    /// </summary>
    internal static class XmlCommentHelper
    {
        internal const string SummaryXmlTag = "summary";
        internal const string ContentXmlTag = "content";
        internal const string InheritdocXmlTag = "inheritdoc";
        internal const string ReturnsXmlTag = "returns";
        internal const string ValueXmlTag = "value";
        internal const string CXmlTag = "c";
        internal const string SeeXmlTag = "see";
        internal const string CodeXmlTag = "code";
        internal const string ListXmlTag = "list";
        internal const string NoteXmlTag = "note";
        internal const string ParaXmlTag = "para";
        internal const string SeeAlsoXmlTag = "seealso";
        internal const string ParamXmlTag = "param";
        internal const string ParamRefXmlTag = "paramref";
        internal const string TypeParamXmlTag = "typeparam";
        internal const string TypeParamRefXmlTag = "typeparamref";
        internal const string RemarksXmlTag = "remarks";
        internal const string ExampleXmlTag = "example";
        internal const string PermissionXmlTag = "permission";
        internal const string ExceptionXmlTag = "exception";
        internal const string IncludeXmlTag = "include";
        internal const string FileAttributeName = "file";
        internal const string PathAttributeName = "path";
        internal const string CrefArgumentName = "cref";
        internal const string NameArgumentName = "name";

        /// <summary>
        /// The &lt;placeholder&gt; tag is a Sandcastle Help File Builder extension to the standard XML documentation
        /// comment tags, and is used to mark sections of documentation which need to be reviewed.
        /// </summary>
        internal const string PlaceholderTag = "placeholder";

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if
        /// - it is null
        /// - it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlComment">The XML comment that should be checked.</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(DocumentationCommentTriviaSyntax xmlComment)
        {
            if (xmlComment == null)
            {
                return true;
            }

            foreach (XmlNodeSyntax syntax in xmlComment.Content)
            {
                if (!IsConsideredEmpty(syntax))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlSyntax">The XML syntax that should be checked.</param>
        /// <param name="considerEmptyElements">Flag indicating if empty elements should be considered or assumed non-empty.</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(XmlNodeSyntax xmlSyntax, bool considerEmptyElements = false)
        {
            if (xmlSyntax is XmlTextSyntax text)
            {
                foreach (SyntaxToken token in text.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ToString()))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (xmlSyntax is XmlElementSyntax element)
            {
                foreach (XmlNodeSyntax syntax in element.Content)
                {
                    if (!IsConsideredEmpty(syntax))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (xmlSyntax is XmlCDataSectionSyntax cdataElement)
            {
                foreach (SyntaxToken token in cdataElement.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ToString()))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (xmlSyntax is XmlEmptyElementSyntax)
            {
                // This includes <inheritdoc/>
                return considerEmptyElements;
            }

            return !(xmlSyntax is XmlProcessingInstructionSyntax);
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if an XML comment should be considered empty.
        /// A comment is empty if it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="node">The XML node that should be checked.</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(XNode node)
        {
            if (node is XText text)
            {
                return string.IsNullOrWhiteSpace(text.Value);
            }

            if (node is XElement element)
            {
                foreach (XNode syntax in element.Nodes())
                {
                    if (!IsConsideredEmpty(syntax))
                    {
                        return false;
                    }
                }

                return true;
            }

            return !(node is XProcessingInstruction);
        }

        /// <summary>
        /// Returns the first <see cref="XmlTextSyntax"/> which is not simply empty or whitespace.
        /// </summary>
        /// <param name="node">The XML content to search.</param>
        /// <returns>The first <see cref="XmlTextSyntax"/> which is not simply empty or whitespace, or
        /// <see langword="null"/> if no such element exists.</returns>
        internal static XmlTextSyntax TryGetFirstTextElementWithContent(XmlNodeSyntax node)
        {
            if (node is XmlEmptyElementSyntax)
            {
                return null;
            }
            else if (node is XmlTextSyntax xmlText)
            {
                return !IsConsideredEmpty(node) ? xmlText : null;
            }
            else if (node is XmlElementSyntax xmlElement)
            {
                foreach (var child in xmlElement.Content)
                {
                    var nestedContent = TryGetFirstTextElementWithContent(child);
                    if (nestedContent != null)
                    {
                        return nestedContent;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the last <see cref="XmlTextSyntax"/> which is not simply empty or whitespace.
        /// </summary>
        /// <param name="node">The XML content to search.</param>
        /// <returns>The last <see cref="XmlTextSyntax"/> which is not simply empty or whitespace, or
        /// <see langword="null"/> if no such element exists.</returns>
        internal static XmlTextSyntax TryGetLastTextElementWithContent(XmlNodeSyntax node)
        {
            if (node is XmlEmptyElementSyntax)
            {
                return null;
            }
            else if (node is XmlTextSyntax xmlText)
            {
                return !IsConsideredEmpty(node) ? xmlText : null;
            }
            else if (node is XmlElementSyntax xmlElement)
            {
                for (var i = xmlElement.Content.Count - 1; i >= 0; i--)
                {
                    var nestedContent = TryGetFirstTextElementWithContent(xmlElement.Content[i]);
                    if (nestedContent != null)
                    {
                        return nestedContent;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a <see cref="SyntaxTrivia"/> contains a <see cref="DocumentationCommentTriviaSyntax"/> and returns
        /// <see langword="true"/> if it is considered empty.
        /// </summary>
        /// <param name="commentTrivia">A <see cref="SyntaxTrivia"/> containing possible documentation.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="commentTrivia"/> does not have documentation in it or the
        /// documentation in <paramref name="commentTrivia"/> is considered empty; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsMissingOrEmpty(SyntaxTrivia commentTrivia)
        {
            if (!commentTrivia.HasStructure)
            {
                return true;
            }

            if (commentTrivia.GetStructure() is DocumentationCommentTriviaSyntax structuredTrivia)
            {
                return IsConsideredEmpty(structuredTrivia);
            }

            return true;
        }

        /// <summary>
        /// Checks if a specific SyntaxNode has documentation in it's leading trivia.
        /// </summary>
        /// <param name="node">The syntax node that should be checked.</param>
        /// <returns>true if the node has documentation, false otherwise.</returns>
        internal static bool HasDocumentation(SyntaxNode node)
        {
            var commentTrivia = node.GetDocumentationCommentTriviaSyntax();

            return commentTrivia != null && !IsMissingOrEmpty(commentTrivia.ParentTrivia);
        }

        internal static string GetText(XmlNodeSyntax nodeSyntax, bool normalizeWhitespace = false)
        {
            if (nodeSyntax is XmlTextSyntax xmlTextSyntax)
            {
                return GetText(xmlTextSyntax, normalizeWhitespace);
            }

            if (nodeSyntax is XmlElementSyntax xmlElementSyntax)
            {
                var stringBuilder = StringBuilderPool.Allocate();

                foreach (var node in xmlElementSyntax.Content)
                {
                    stringBuilder.Append(GetText(node, normalizeWhitespace));
                }

                return StringBuilderPool.ReturnAndFree(stringBuilder);
            }

            if (nodeSyntax is XmlEmptyElementSyntax emptyXmlElement)
            {
                return emptyXmlElement.NormalizeWhitespace(string.Empty).ToString();
            }

            return null;
        }

        internal static string GetText(XmlTextSyntax textElement)
        {
            return GetText(textElement, false);
        }

        internal static string GetText(XmlTextSyntax textElement, bool normalizeWhitespace)
        {
            if (textElement == null)
            {
                return null;
            }

            bool lastWhitespace = false;

            string single = string.Empty;

            StringBuilder stringBuilder = null;

            foreach (var item in textElement.TextTokens)
            {
                if (single.Length == 0)
                {
                    single = item.ToString();
                }
                else
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = StringBuilderPool.Allocate();
                        stringBuilder.AppendNormalize(single, normalizeWhitespace, ref lastWhitespace);
                    }

                    stringBuilder.AppendNormalize(item.ToString(), normalizeWhitespace, ref lastWhitespace);
                }
            }

            if (stringBuilder == null)
            {
                if (normalizeWhitespace)
                {
                    stringBuilder = StringBuilderPool.Allocate();

                    if (!stringBuilder.AppendNormalize(single, normalizeWhitespace, ref lastWhitespace))
                    {
                        StringBuilderPool.Free(stringBuilder);

                        // No change is needed, return original string.
                        return single;
                    }
                }
                else
                {
                    return single;
                }
            }

            return StringBuilderPool.ReturnAndFree(stringBuilder);
        }

        /// <summary>
        /// Append to StringBuilder and perform white space normalization.
        /// </summary>
        /// <param name="builder">StringBuilder to append to.</param>
        /// <param name="text">String to append.</param>
        /// <param name="normalizeWhitespace">Normalize flag.</param>
        /// <param name="lastWhitespace">last char is white space flag.</param>
        /// <returns>True if output is different.</returns>
        internal static bool AppendNormalize(this StringBuilder builder, string text, bool normalizeWhitespace, ref bool lastWhitespace)
        {
            bool diff = false;

            if (normalizeWhitespace)
            {
                foreach (char ch in text)
                {
                    if (char.IsWhiteSpace(ch))
                    {
                        if (lastWhitespace)
                        {
                            diff = true;
                        }
                        else
                        {
                            if (ch != ' ')
                            {
                                diff = true;
                            }

                            builder.Append(' ');
                        }

                        lastWhitespace = true;
                    }
                    else
                    {
                        builder.Append(ch);

                        lastWhitespace = false;
                    }
                }
            }
            else
            {
                builder.Append(text);
            }

            return diff;
        }

        internal static T GetFirstAttributeOrDefault<T>(XmlNodeSyntax nodeSyntax)
            where T : XmlAttributeSyntax
        {
            if (nodeSyntax is XmlEmptyElementSyntax emptyElementSyntax)
            {
                return emptyElementSyntax.Attributes.OfType<T>().FirstOrDefault();
            }

            if (nodeSyntax is XmlElementSyntax elementSyntax)
            {
                return elementSyntax.StartTag?.Attributes.OfType<T>().FirstOrDefault();
            }

            return null;
        }

        internal static bool IsInlineElement(this XmlNodeSyntax nodeSyntax)
        {
            if (nodeSyntax is XmlEmptyElementSyntax emptyElementSyntax)
            {
                return IsInlineElement(emptyElementSyntax.Name?.LocalName.ValueText);
            }

            if (nodeSyntax is XmlElementSyntax elementSyntax)
            {
                return IsInlineElement(elementSyntax.StartTag?.Name?.LocalName.ValueText);
            }

            return false;
        }

        internal static bool IsBlockElement(this XmlNodeSyntax nodeSyntax)
        {
            if (nodeSyntax is XmlElementSyntax elementSyntax)
            {
                return IsBlockElement(elementSyntax.StartTag?.Name?.LocalName.ValueText);
            }

            return false;
        }

        /// <summary>
        /// Parses a <see cref="XmlComment"/> object from a <see cref="SyntaxNodeAnalysisContext"/>.
        /// </summary>
        /// <param name="context">The analysis context that will be checked.</param>
        /// <param name="node">The node to parse the documentation for.</param>
        /// <returns>The parsed <see cref="XmlComment"/>.</returns>
        internal static XmlComment GetParsedXmlComment(this SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var documentation = node.GetDocumentationCommentTriviaSyntax();

            if (documentation == null || IsMissingOrEmpty(documentation.ParentTrivia))
            {
                return XmlComment.MissingSummary;
            }

            bool hasInheritdoc = false;
            string[] documentedParameterNames;

            if (documentation.Content.GetFirstXmlElement(InheritdocXmlTag) != null)
            {
                hasInheritdoc = true;
            }

            bool hasIncludedDocumentation = documentation.Content.GetFirstXmlElement(IncludeXmlTag) != null;

            if (hasIncludedDocumentation)
            {
                var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(node, context.CancellationToken);
                var rawDocumentation = declaredSymbol?.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: context.CancellationToken);
                var includedDocumentation = XElement.Parse(rawDocumentation ?? "<doc></doc>", LoadOptions.None);

                if (includedDocumentation.Nodes().OfType<XElement>().Any(element => element.Name == InheritdocXmlTag))
                {
                    hasInheritdoc = true;
                }

                IEnumerable<XElement> paramElements = includedDocumentation.Nodes()
                    .OfType<XElement>()
                    .Where(x => x.Name == ParamXmlTag);

                documentedParameterNames = paramElements
                    .SelectMany(x => x.Attributes().Where(y => y.Name == NameArgumentName))
                    .Select(x => x.Value)
                    .ToArray();
            }
            else
            {
                IEnumerable<XmlNodeSyntax> xmlNodes = documentation.Content.GetXmlElements(ParamXmlTag);
                documentedParameterNames = xmlNodes.Select(GetFirstAttributeOrDefault<XmlNameAttributeSyntax>)
                    .Where(x => x != null)
                    .Select(x => x.Identifier.Identifier.ValueText)
                    .ToArray();
            }

            return new XmlComment(documentedParameterNames, hasInheritdoc);
        }

        private static bool IsInlineElement(string localName)
        {
            switch (localName)
            {
            case CXmlTag:
            case ParamRefXmlTag:
            case SeeXmlTag:
            case TypeParamRefXmlTag:
                return true;

            default:
                return false;
            }
        }

        private static bool IsBlockElement(string localName)
        {
            switch (localName)
            {
            case CodeXmlTag:
            case ListXmlTag:
            case NoteXmlTag:
            case ParaXmlTag:
                return true;

            default:
                return false;
            }
        }
    }
}
