// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertyDocumentationHelper
    {
        /// <summary>
        /// Creates the indexer summery comment.
        /// </summary>
        /// <param name="indexerDeclarationSyntax">The indexer declaration syntax.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="newLineText">The new line text.</param>
        /// <returns>The indexer summery comment text.</returns>
        public static XmlNodeSyntax CreateIndexerSummeryNode(
            IndexerDeclarationSyntax indexerDeclarationSyntax,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            string newLineText)
        {
            var propertyData = PropertyAnalyzerHelper.AnalyzeIndexerAccessors(indexerDeclarationSyntax, semanticModel, cancellationToken);
            string comment = GetPropertyGetsOrSetsPrefix(ref propertyData);
            return CommonDocumentationHelper.CreateSummaryNode(comment + " the element at the specified index.", newLineText);
        }

        /// <summary>
        /// Creates the property summery comment.
        /// </summary>
        /// <param name="propertyDeclaration">The property declaration.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="newLineText">The new line text.</param>
        /// <returns>Create property summery comment.</returns>
        public static XmlNodeSyntax CreatePropertySummeryComment(
            PropertyDeclarationSyntax propertyDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            string newLineText)
        {
            var propertyName = propertyDeclaration.Identifier.ValueText;
            var propertyData = PropertyAnalyzerHelper.AnalyzePropertyAccessors(propertyDeclaration, semanticModel, cancellationToken);
            string comment = GetPropertyGetsOrSetsPrefix(ref propertyData);

            if (CommonDocumentationHelper.IsBooleanParameter(propertyDeclaration.Type))
            {
                comment += CreatePropertyBooleanPart(propertyName);
            }
            else
            {
                comment += " the " + CommonDocumentationHelper.SplitNameAndToLower(propertyName, true);
            }

            return CommonDocumentationHelper.CreateSummaryNode(comment + ".", newLineText);
        }

        private static string GetPropertyGetsOrSetsPrefix(
            ref PropertyAnalyzerHelper.PropertyData propertyData)
        {
            var getsPrefix = "Gets";
            return propertyData.SetterVisible ? getsPrefix + " or sets" : getsPrefix;
        }

        private static string CreatePropertyBooleanPart(string name)
        {
            string booleanPart = " a value indicating whether ";

            var nameDocumentation = CommonDocumentationHelper.SplitNameAndToLower(name, true);

            var isWord = nameDocumentation.IndexOf("is", StringComparison.OrdinalIgnoreCase);
            if (isWord != -1)
            {
                nameDocumentation = nameDocumentation.Remove(isWord, 2) + " is";
            }

            return booleanPart + nameDocumentation;
        }
    }
}
