namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Provides helper methods to work with Xml comments
    /// </summary>
    internal static class XmlCommentHelper
    {
        internal const string SummaryXmlTag = "summary";
        internal const string ContentXmlTag = "content";
        internal const string InheritdocXmlTag = "inheritdoc";
        internal const string ReturnsXmlTag = "returns";
        internal const string ValueXmlTag = "value";
        internal const string SeeXmlTag = "see";
        internal const string CrefArgumentName = "cref";

        /// <summary>
        /// The &lt;placeholder&gt; tag is a Sandcastle Help File Builder extension to the standard XML documentation
        /// comment tags, and is used to mark sections of documentation which need to be reviewed.
        /// </summary>
        internal const string PlaceholderTag = "placeholder";

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a xml comment should be considered empty.
        /// A comment is empty if 
        /// - it is null
        /// - it does not have any text in any xml element and it does not have an empty xml element in it.
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
        /// This helper is used by documentation diagnostics to check if a xml comment should be considered empty.
        /// A comment is empty if it does not have any text in any xml element and it does not have an empty xml element in it.
        /// </summary>
        /// <param name="xmlSyntax">The xmlSyntax that should be checked</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(XmlNodeSyntax xmlSyntax)
        {
            var text = xmlSyntax as XmlTextSyntax;
            if (text != null)
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

            var element = xmlSyntax as XmlElementSyntax;
            if (element != null)
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

            var cDataElement = xmlSyntax as XmlCDataSectionSyntax;
            if (cDataElement != null)
            {
                foreach (SyntaxToken token in cDataElement.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ToString()))
                    {
                        return false;
                    }
                }
                return true;
            }

            var emptyElement = xmlSyntax as XmlEmptyElementSyntax;
            if (emptyElement != null)
            {
                // This includes <inheritdoc/>
                return false;
            }

            var processingElement = xmlSyntax as XmlProcessingInstructionSyntax;
            if (processingElement != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a SyntaxTrivia contains a DocumentationCommentTriviaSyntax and returns true if it is considered empty
        /// </summary>
        /// <param name="commentTrivia">A SyntaxTrivia containing possible documentation</param>
        /// <returns>true if commentTrivia does not have documentation in it orthe documentation in SyntaxTriviais considered empty. False otherwise.</returns>
        internal static bool IsMissingOrEmpty(SyntaxTrivia commentTrivia)
        {
            if (!commentTrivia.HasStructure)
            {
                return true;
            }

            var structuredTrivia = commentTrivia.GetStructure() as DocumentationCommentTriviaSyntax;
            if (structuredTrivia != null)
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
            var commentTrivia = GetCommentTrivia(node);

            return !IsMissingOrEmpty(commentTrivia);
        }

        internal static DocumentationCommentTriviaSyntax GetDocumentationStructure(SyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }

            var commentTrivia = GetCommentTrivia(node);

            if (!commentTrivia.HasStructure)
            {
                return null;
            }

            return commentTrivia.GetStructure() as DocumentationCommentTriviaSyntax;
        }

        internal static XmlNodeSyntax GetTopLevelElement(DocumentationCommentTriviaSyntax syntax, string tagName)
        {
            XmlElementSyntax elementSyntax = syntax.Content.OfType<XmlElementSyntax>().FirstOrDefault(element => string.Equals(element.StartTag.Name.ToString(), tagName));
            if (elementSyntax != null)
            {
                return elementSyntax;
            }

            XmlEmptyElementSyntax emptyElementSyntax = syntax.Content.OfType<XmlEmptyElementSyntax>().FirstOrDefault(element => string.Equals(element.Name.ToString(), tagName));
            return emptyElementSyntax;
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

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in textElement.TextTokens)
            {
                stringBuilder.Append(item);
            }

            string result = stringBuilder.ToString();
            if (normalizeWhitespace)
            {
                result = Regex.Replace(result, @"\s+", " ");
            }

            return result;
        }

        private static SyntaxTrivia GetCommentTrivia(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();
            var commentTrivia = leadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia));
            if (commentTrivia != default(SyntaxTrivia))
            {
                return commentTrivia;
            }
            return leadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia));
        }
    }
}