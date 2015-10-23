// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1400AccessModifierMustBeDeclared"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add an access modifier to the declaration of the element.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1400CodeFixProvider))]
    [Shared]
    internal class SA1400CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1400AccessModifierMustBeDeclared.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                if (node == null || node.IsMissing)
                {
                    continue;
                }

                SyntaxNode declarationNode = FindParentDeclarationNode(node);
                if (declarationNode == null)
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        MaintainabilityResources.SA1400CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, root, declarationNode),
                        nameof(SA1400CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SyntaxNode declarationNode)
        {
            SyntaxNode updatedDeclarationNode;
            switch (declarationNode.Kind())
            {
            case SyntaxKind.ClassDeclaration:
                updatedDeclarationNode = HandleClassDeclaration((ClassDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.InterfaceDeclaration:
                updatedDeclarationNode = HandleInterfaceDeclaration((InterfaceDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.EnumDeclaration:
                updatedDeclarationNode = HandleEnumDeclaration((EnumDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.StructDeclaration:
                updatedDeclarationNode = HandleStructDeclaration((StructDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.DelegateDeclaration:
                updatedDeclarationNode = HandleDelegateDeclaration((DelegateDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.EventDeclaration:
                updatedDeclarationNode = HandleEventDeclaration((EventDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.EventFieldDeclaration:
                updatedDeclarationNode = HandleEventFieldDeclaration((EventFieldDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.MethodDeclaration:
                updatedDeclarationNode = HandleMethodDeclaration((MethodDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.PropertyDeclaration:
                updatedDeclarationNode = HandlePropertyDeclaration((PropertyDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.FieldDeclaration:
                updatedDeclarationNode = HandleFieldDeclaration((FieldDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.OperatorDeclaration:
                updatedDeclarationNode = HandleOperatorDeclaration((OperatorDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.ConversionOperatorDeclaration:
                updatedDeclarationNode = HandleConversionOperatorDeclaration((ConversionOperatorDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.IndexerDeclaration:
                updatedDeclarationNode = HandleIndexerDeclaration((IndexerDeclarationSyntax)declarationNode);
                break;

            case SyntaxKind.ConstructorDeclaration:
                updatedDeclarationNode = HandleConstructorDeclaration((ConstructorDeclarationSyntax)declarationNode);
                break;

            default:
                throw new InvalidOperationException("Unhandled declaration kind: " + declarationNode.Kind());
            }

            var newSyntaxRoot = root.ReplaceNode(declarationNode, updatedDeclarationNode);
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private static SyntaxNode HandleClassDeclaration(ClassDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleEnumDeclaration(EnumDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EnumKeyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithEnumKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleStructDeclaration(StructDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Keyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.DelegateKeyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxKind defaultVisibility = IsNestedType(node) ? SyntaxKind.PrivateKeyword : SyntaxKind.InternalKeyword;
            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, defaultVisibility);
            return node
                .WithDelegateKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleEventDeclaration(EventDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EventKeyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithEventKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.EventKeyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithEventKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleMethodDeclaration(MethodDeclarationSyntax node)
        {
            TypeSyntax type = node.ReturnType;
            if (type == null || type.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithReturnType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandlePropertyDeclaration(PropertyDeclarationSyntax node)
        {
            TypeSyntax type = node.Type;
            if (type == null || type.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleFieldDeclaration(FieldDeclarationSyntax node)
        {
            VariableDeclarationSyntax declaration = node.Declaration;
            if (declaration == null || declaration.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref declaration, SyntaxKind.PrivateKeyword);
            return node
                .WithDeclaration(declaration)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            TypeSyntax type = node.ReturnType;
            if (type == null || type.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref type, SyntaxKind.PublicKeyword);
            return node
                .WithReturnType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.ImplicitOrExplicitKeyword;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PublicKeyword);
            return node
                .WithImplicitOrExplicitKeyword(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            TypeSyntax type = node.Type;
            if (type == null || type.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref type, SyntaxKind.PrivateKeyword);
            return node
                .WithType(type)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode HandleConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            SyntaxToken triviaToken = node.Identifier;
            if (triviaToken.IsMissing)
            {
                return null;
            }

            SyntaxTokenList modifiers = DeclarationModifiersHelper.AddModifier(node.Modifiers, ref triviaToken, SyntaxKind.PrivateKeyword);
            return node
                .WithIdentifier(triviaToken)
                .WithModifiers(modifiers)
                .WithoutFormatting();
        }

        private static SyntaxNode FindParentDeclarationNode(SyntaxNode node)
        {
            while (node != null)
            {
                switch (node.Kind())
                {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                    return node;

                default:
                    node = node.Parent;
                    break;
                }
            }

            return node;
        }

        private static bool IsNestedType(BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }

        private static bool IsNestedType(DelegateDeclarationSyntax delegateDeclaration)
        {
            return delegateDeclaration?.Parent is BaseTypeDeclarationSyntax;
        }
    }
}
