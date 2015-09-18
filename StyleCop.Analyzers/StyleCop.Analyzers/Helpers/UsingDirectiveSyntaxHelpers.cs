// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        internal static bool IsSystemUsingDirective(this UsingDirectiveSyntax usingDirective) => string.Equals(SystemUsingDirectiveIdentifier, GetFirstIdentifierInUsingDirective(usingDirective)?.Text, StringComparison.Ordinal);

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

        private static bool ExcludeGlobalKeyword(IdentifierNameSyntax token) => !token.Identifier.IsKind(SyntaxKind.GlobalKeyword);

        private static SyntaxToken? GetFirstIdentifierInUsingDirective(UsingDirectiveSyntax usingDirective)
        {
            foreach (var identifier in usingDirective.DescendantNodes())
            {
                IdentifierNameSyntax identifierName = identifier as IdentifierNameSyntax;

                if (identifierName != null && ExcludeGlobalKeyword(identifierName))
                {
                    return identifierName.Identifier;
                }
            }

            return null;
        }
    }
}
