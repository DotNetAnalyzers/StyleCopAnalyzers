// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using ObjectPools;

    /// <summary>
    /// Class containing the extension methods for the <see cref="NameSyntax"/> class.
    /// </summary>
    internal static class NameSyntaxHelpers
    {
        private const string DotChar = ".";

        /// <summary>
        /// Gets the name contained in the <see cref="NameSyntax"/>, including the alias prefix (if any).
        /// </summary>
        /// <param name="nameSyntax">The <see cref="NameSyntax"/> from which the name will be extracted.</param>
        /// <returns>The name contained in the <see cref="NameSyntax"/>, including its alias (if any).</returns>
        internal static string ToNormalizedString(this NameSyntax nameSyntax)
        {
            var sb = new StringBuilder();

            BuildName(nameSyntax, sb, true);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the name contained in the <see cref="NameSyntax"/>, without an alias prefix.
        /// </summary>
        /// <param name="nameSyntax">The <see cref="NameSyntax"/> from which the name will be extracted.</param>
        /// <returns>The name contained in the <see cref="NameSyntax"/>, with its alias removed (if any).</returns>
        internal static string ToUnaliasedString(this NameSyntax nameSyntax)
        {
            var sb = StringBuilderPool.Allocate();

            BuildName(nameSyntax, sb, false);

            return StringBuilderPool.ReturnAndFree(sb);
        }

        private static void BuildName(NameSyntax nameSyntax, StringBuilder builder, bool includeAlias)
        {
            if (nameSyntax.IsKind(SyntaxKind.IdentifierName))
            {
                var identifierNameSyntax = (IdentifierNameSyntax)nameSyntax;
                builder.Append(identifierNameSyntax.Identifier.ValueText);
            }
            else if (nameSyntax.IsKind(SyntaxKind.QualifiedName))
            {
                var qualifiedNameSyntax = (QualifiedNameSyntax)nameSyntax;
                BuildName(qualifiedNameSyntax.Left, builder, includeAlias);
                builder.Append(DotChar);
                BuildName(qualifiedNameSyntax.Right, builder, includeAlias);
            }
            else if (nameSyntax.IsKind(SyntaxKind.GenericName))
            {
                var genericNameSyntax = (GenericNameSyntax)nameSyntax;
                builder.AppendFormat("{0}{1}", genericNameSyntax.Identifier.ValueText, genericNameSyntax.TypeArgumentList);
            }
            else if (nameSyntax.IsKind(SyntaxKind.AliasQualifiedName))
            {
                var aliasQualifiedNameSyntax = (AliasQualifiedNameSyntax)nameSyntax;
                if (includeAlias)
                {
                    builder.Append(aliasQualifiedNameSyntax.Alias.Identifier.ValueText);
                    builder.Append("::");
                }

                builder.Append(aliasQualifiedNameSyntax.Name.Identifier.ValueText);
            }
        }
    }
}
