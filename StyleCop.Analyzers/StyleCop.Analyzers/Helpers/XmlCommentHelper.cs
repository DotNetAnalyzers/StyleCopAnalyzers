// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers.ObjectPools;

    /// <summary>
    /// Provides helper methods to work with XML comments
    /// </summary>
    internal static class XmlCommentHelper
    {
        internal const string SummaryXmlTag = "summary";
        internal const string ContentXmlTag = "content";
        internal const string InheritdocXmlTag = "inheritdoc";
        internal const string ReturnsXmlTag = "returns";
        internal const string ValueXmlTag = "value";
        internal const string SeeXmlTag = "see";
        internal const string ParamXmlTag = "param";
        internal const string TypeParamXmlTag = "typeparam";
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
        /// <param name="xmlComment">The xmlComment that should be checked</param>
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
        /// <param name="xmlSyntax">The xmlSyntax that should be checked</param>
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

            if (xmlSyntax is XmlEmptyElementSyntax emptyElement)
            {
                // This includes <inheritdoc/>
                return considerEmptyElements;
            }

            if (xmlSyntax is XmlProcessingInstructionSyntax processingElement)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if an XML comment should be considered empty.
        /// A comment is empty if it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="node">The XML node that should be checked</param>
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

            if (node is XProcessingInstruction processingElement)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a SyntaxTrivia contains a DocumentationCommentTriviaSyntax and returns true if it is considered empty
        /// </summary>
        /// <param name="commentTrivia">A SyntaxTrivia containing possible documentation</param>
        /// <returns>true if commentTrivia does not have documentation in it or the documentation in SyntaxTriviais considered empty. False otherwise.</returns>
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

            StringBuilder stringBuilder = StringBuilderPool.Allocate();

            foreach (var item in textElement.TextTokens)
            {
                stringBuilder.Append(item);
            }

            string result = StringBuilderPool.ReturnAndFree(stringBuilder);
            if (normalizeWhitespace)
            {
                result = Regex.Replace(result, @"\s+", " ");
            }

            return result;
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
    }
}
