using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace StyleCop.Analyzers.Helpers
{
    /// <summary>
    /// Provides helper methods to work with Xml comments
    /// </summary>
    internal static class XmlCommentHelper
    {
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
                return true;
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
                return true;
            var structuredTrivia = commentTrivia.GetStructure() as DocumentationCommentTriviaSyntax;
            if (structuredTrivia != null)
            {
                return IsConsideredEmpty(structuredTrivia);
            }

            return true;
        }
    }
}