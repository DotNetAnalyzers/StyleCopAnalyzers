// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.DocumentationRules;

    internal static class EventDocumentationHelper
    {
        /// <summary>
        /// Creates the event documentation.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="newLineText">The new line text.</param>
        /// <returns>The event documentation node.</returns>
        public static XmlNodeSyntax CreateEventDocumentation(SyntaxToken identifier, string newLineText)
        {
            return CommonDocumentationHelper.CreateSummaryNode(
                DocumentationResources.EventDocumentationPrefix + CommonDocumentationHelper.GetNameDocumentation(identifier.ValueText),
                newLineText);
        }
    }
}
