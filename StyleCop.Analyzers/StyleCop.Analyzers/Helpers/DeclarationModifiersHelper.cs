﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class DeclarationModifiersHelper
    {
        /// <summary>
        /// Adds a modifier token for <paramref name="modifierKeyword"/> to the beginning of
        /// <paramref name="modifiers"/>. The trivia for the new modifier and the trivia for the token that follows it
        /// are updated to ensure that the new modifier is placed immediately before the syntax token that follows it,
        /// separated by exactly one space.
        /// </summary>
        /// <typeparam name="T">The type of syntax node which follows the modifier list.</typeparam>
        /// <param name="modifiers">The existing modifiers. This may be empty if no modifiers are present.</param>
        /// <param name="leadingTriviaNode">The syntax node which follows the modifiers. The trivia for this node is
        /// updated if (and only if) the existing <paramref name="modifiers"/> list is empty.</param>
        /// <param name="modifierKeyword">The modifier keyword to add.</param>
        /// <returns>A <see cref="SyntaxTokenList"/> representing the original modifiers (if any) with the addition of a
        /// modifier of the specified <paramref name="modifierKeyword"/> at the beginning of the list.</returns>
        internal static SyntaxTokenList AddModifier<T>(SyntaxTokenList modifiers, ref T leadingTriviaNode, SyntaxKind modifierKeyword)
            where T : SyntaxNode
        {
            SyntaxToken modifier = SyntaxFactory.Token(modifierKeyword);
            if (modifiers.Count > 0)
            {
                modifier = modifier.WithLeadingTrivia(modifiers[0].LeadingTrivia);
                modifiers = modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(SyntaxFactory.ElasticSpace));
                modifiers = modifiers.Insert(0, modifier);
            }
            else
            {
                modifiers = SyntaxTokenList.Create(modifier.WithLeadingTrivia(leadingTriviaNode.GetLeadingTrivia()));
                leadingTriviaNode = leadingTriviaNode.WithLeadingTrivia(SyntaxFactory.ElasticSpace);
            }

            return modifiers;
        }

        /// <summary>
        /// Adds a modifier token for <paramref name="modifierKeyword"/> to the beginning of
        /// <paramref name="modifiers"/>. The trivia for the new modifier and the trivia for the token that follows it
        /// are updated to ensure that the new modifier is placed immediately before the syntax token that follows it,
        /// separated by exactly one space.
        /// </summary>
        /// <param name="modifiers">The existing modifiers. This may be empty if no modifiers are present.</param>
        /// <param name="leadingTriviaToken">The syntax token which follows the modifiers. The trivia for this token is
        /// updated if (and only if) the existing <paramref name="modifiers"/> list is empty.</param>
        /// <param name="modifierKeyword">The modifier keyword to add.</param>
        /// <returns>A <see cref="SyntaxTokenList"/> representing the original modifiers (if any) with the addition of a
        /// modifier of the specified <paramref name="modifierKeyword"/> at the beginning of the list.</returns>
        internal static SyntaxTokenList AddModifier(SyntaxTokenList modifiers, ref SyntaxToken leadingTriviaToken, SyntaxKind modifierKeyword)
        {
            SyntaxToken modifier = SyntaxFactory.Token(modifierKeyword);
            if (modifiers.Count > 0)
            {
                modifier = modifier.WithLeadingTrivia(modifiers[0].LeadingTrivia);
                modifiers = modifiers.Replace(modifiers[0], modifiers[0].WithLeadingTrivia(SyntaxFactory.ElasticSpace));
                modifiers = modifiers.Insert(0, modifier);
            }
            else
            {
                modifiers = SyntaxTokenList.Create(modifier.WithLeadingTrivia(leadingTriviaToken.LeadingTrivia));
                leadingTriviaToken = leadingTriviaToken.WithLeadingTrivia(SyntaxFactory.ElasticSpace);
            }

            return modifiers;
        }

        /// <summary>
        /// Adds a number of modifier tokens for <paramref name="modifierKeywords"/> to the beginning of
        /// <paramref name="modifiers"/>. The trivia for the new modifier and the trivia for the token that follows it
        /// are updated to ensure that the new modifier is placed immediately before the syntax token that follows it,
        /// separated by exactly one space.
        /// </summary>
        /// <param name="modifiers">The existing modifiers. This may be empty if no modifiers are present.</param>
        /// <param name="leadingTriviaToken">The syntax token which follows the modifiers. The trivia for this token is
        /// updated if (and only if) the existing <paramref name="modifiers"/> list is empty.</param>
        /// <param name="modifierKeywords">The modifier keywords to add.</param>
        /// <returns>A <see cref="SyntaxTokenList"/> representing the original modifiers (if any) with the addition of a
        /// modifier of the specified <paramref name="modifierKeywords"/> at the beginning of the list.</returns>
        internal static SyntaxTokenList AddModifiers(SyntaxTokenList modifiers, ref SyntaxToken leadingTriviaToken, IEnumerable<SyntaxKind> modifierKeywords)
        {
            foreach (var modifierKeyword in modifierKeywords.Reverse())
            {
                modifiers = AddModifier(modifiers, ref leadingTriviaToken, modifierKeyword);
            }

            return modifiers;
        }

        internal static SyntaxTokenList GetModifiers(this MemberDeclarationSyntax syntax)
        {
            if (syntax is BaseMethodDeclarationSyntax)
            {
                return ((BaseMethodDeclarationSyntax)syntax).Modifiers;
            }
            else if (syntax is BasePropertyDeclarationSyntax)
            {
                return ((BasePropertyDeclarationSyntax)syntax).Modifiers;
            }
            else if (syntax is BaseTypeDeclarationSyntax)
            {
                return ((BaseTypeDeclarationSyntax)syntax).Modifiers;
            }
            else if (syntax is BaseFieldDeclarationSyntax)
            {
                return ((BaseFieldDeclarationSyntax)syntax).Modifiers;
            }
            else if (syntax is DelegateDeclarationSyntax)
            {
                return ((DelegateDeclarationSyntax)syntax).Modifiers;
            }
            else if (syntax is IncompleteMemberSyntax)
            {
                return ((IncompleteMemberSyntax)syntax).Modifiers;
            }

            return default;
        }

        internal static SyntaxNode WithModifiers(this SyntaxNode node, SyntaxTokenList modifiers)
        {
            switch (node.Kind())
            {
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.ConstructorDeclaration:
                return ((ConstructorDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.OperatorDeclaration:
                return ((OperatorDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.ConversionOperatorDeclaration:
                return ((ConversionOperatorDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.PropertyDeclaration:
                return ((PropertyDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.EventDeclaration:
                return ((EventDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.EnumDeclaration:
                return ((EnumDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.DelegateDeclaration:
                return ((DelegateDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.FieldDeclaration:
                return ((FieldDeclarationSyntax)node).WithModifiers(modifiers);

            case SyntaxKind.EventFieldDeclaration:
                return ((EventFieldDeclarationSyntax)node).WithModifiers(modifiers);

            default:
                return node;
            }
        }
    }
}
