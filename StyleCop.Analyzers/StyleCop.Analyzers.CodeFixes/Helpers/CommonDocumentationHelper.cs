// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.DocumentationRules;

    internal static class CommonDocumentationHelper
    {
        /// <summary>
        /// Creates the summery node.
        /// </summary>
        /// <param name="summeryContent">Content of the summery.</param>
        /// <param name="newLineText">The new line text.</param>
        /// <returns>The summery node.</returns>
        public static XmlNodeSyntax CreateSummaryNode(string summeryContent, string newLineText)
        {
            var summerySyntax = XmlSyntaxFactory.Text(summeryContent);
            return XmlSyntaxFactory.SummaryElement(newLineText, summerySyntax);
        }

        /// <summary>
        /// Creates default summery comment.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="newLineText">The new line text.</param>
        /// <returns>The method comment.</returns>
        public static XmlNodeSyntax CreateDefaultSummaryNode(string name, string newLineText)
        {
            return CreateSummaryNode(GetNameDocumentation(name, false), newLineText);
        }

        public static string SplitNameAndToLower(string name, bool isFirstCharacterLower, bool skipSingleCharIfFirst = false)
        {
            var splitName = NameSplitter.Split(name)
                .Select((n, i) => !isFirstCharacterLower && i == 0 ? n : n.ToLower())
                .ToList();

            var skipFirst = splitName.Count != 1 && skipSingleCharIfFirst && splitName[0].Length == 1;
            return string.Join(" ", skipFirst ? splitName.Skip(1) : splitName);
        }

        public static bool IsBooleanParameter(TypeSyntax type)
        {
            if (type.IsKind(SyntaxKind.PredefinedType))
            {
                return ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.BoolKeyword);
            }
            else if (type.IsKind(SyntaxKind.NullableType))
            {
                // If it is not predefined type syntax, it should be IdentifierNameSyntax.
                if (((NullableTypeSyntax)type).ElementType is PredefinedTypeSyntax predefinedType)
                {
                    return predefinedType.Keyword.IsKind(SyntaxKind.BoolKeyword);
                }
            }

            return false;
        }

        public static string CreateCommonComment(string name, bool skipSingleCharIfFirst = false)
        {
            return "The " + GetNameDocumentation(name, skipSingleCharIfFirst: skipSingleCharIfFirst);
        }

        public static string GetNameDocumentation(string name, bool isFirstCharacterLower = true, bool skipSingleCharIfFirst = false)
        {
            return $"{SplitNameAndToLower(name, isFirstCharacterLower, skipSingleCharIfFirst)}.";
        }

        public static SyntaxTrivia GetLastDocumentationCommentExteriorTrivia(SyntaxNode node)
        {
            return node
                .DescendantTrivia(descendIntoTrivia: true)
                .LastOrDefault(trivia => trivia.IsKind(SyntaxKind.DocumentationCommentExteriorTrivia));
        }
    }
}
