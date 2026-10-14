// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Class containing the extension methods for the <see cref="UsingDirectiveSyntax"/> class.
    /// </summary>
    internal static class UsingDirectiveSyntaxHelpers
    {
        private const string SystemUsingDirectiveIdentifier = nameof(System);

        /// <summary>
        /// Check if <see cref="UsingDirectiveSyntax"/> is system using directive.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be checked.</param>
        /// <returns>Return true if the <see cref="UsingDirectiveSyntax"/>is system using directive, otherwise false.</returns>
        internal static bool IsSystemUsingDirective(this UsingDirectiveSyntax usingDirective) => string.Equals(SystemUsingDirectiveIdentifier, GetFirstIdentifierInUsingDirective(usingDirective)?.ValueText, StringComparison.Ordinal);

        /// <summary>
        /// Check if <see cref="UsingDirectiveSyntax"/> is preceded by a preprocessor directive.
        /// </summary>
        /// <param name="usingDirective">The using directive.</param>
        /// <returns>True if the <see cref="UsingDirectiveSyntax"/> is preceded by a preprocessor directive, otherwise false.</returns>
        internal static bool IsPrecededByPreprocessorDirective(this UsingDirectiveSyntax usingDirective)
        {
            if (!usingDirective.HasLeadingTrivia)
            {
                return false;
            }

            foreach (var trivia in usingDirective.GetLeadingTrivia())
            {
                if (trivia.IsDirective)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the name of using directive contains a namespace alias qualifier.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be checked.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="UsingDirectiveSyntax"/> contains a namespace alias qualifier;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasNamespaceAliasQualifier(this UsingDirectiveSyntax usingDirective) => usingDirective.DescendantNodes().Any(node => node.IsKind(SyntaxKind.AliasQualifiedName));

        /// <summary>
        /// Get the <see cref="UsingGroup"/> for the give using directive.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be used.</param>
        /// <param name="settings">The <see cref="StyleCopSettings"/> that will be used.</param>
        /// <returns>The <see cref="UsingGroup"/> for the given <paramref name="usingDirective"/>.</returns>
        internal static UsingGroup GetUsingGroupType(this UsingDirectiveSyntax usingDirective, StyleCopSettings settings)
        {
            if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
            {
                return UsingGroup.Static;
            }

            if (usingDirective.Alias != null)
            {
                return UsingGroup.Alias;
            }

            if (settings.OrderingRules.SystemUsingDirectivesFirst
                && usingDirective.IsSystemUsingDirective())
            {
                return UsingGroup.System;
            }

            return UsingGroup.Regular;
        }

        /// <summary>
        /// Checks if the Name part of the given using directive starts with an alias.
        /// </summary>
        /// <param name="usingDirective">The <see cref="UsingDirectiveSyntax"/> that will be used.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> that will be used.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to interrupt the operation.</param>
        /// <returns>True if the name part of the using directive starts with an alias.</returns>
        internal static bool StartsWithAlias(this UsingDirectiveSyntax usingDirective, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var firstPart = usingDirective.Name.DescendantNodes().FirstOrDefault() ?? usingDirective.Name;
            return semanticModel.GetAliasInfo(firstPart, cancellationToken) != null;
        }

        private static bool ExcludeGlobalKeyword(IdentifierNameSyntax token) => !token.Identifier.IsKind(SyntaxKind.GlobalKeyword);

        private static SyntaxToken? GetFirstIdentifierInUsingDirective(UsingDirectiveSyntax usingDirective)
        {
            foreach (var identifier in usingDirective.DescendantNodes())
            {
                if (identifier is IdentifierNameSyntax identifierName
                    && ExcludeGlobalKeyword(identifierName))
                {
                    return identifierName.Identifier;
                }
            }

            return null;
        }
    }
}
