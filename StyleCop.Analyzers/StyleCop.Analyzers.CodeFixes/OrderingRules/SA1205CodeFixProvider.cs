// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements code fixes for <see cref="SA1205PartialElementsMustDeclareAccess"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1205CodeFixProvider))]
    [Shared]
    internal class SA1205CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1205PartialElementsMustDeclareAccess.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.SA1205CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1205CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var typeDeclarationNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as TypeDeclarationSyntax;
            if (typeDeclarationNode == null)
            {
                return document;
            }

            var symbol = semanticModel.GetDeclaredSymbol(typeDeclarationNode);
            var accessModifierKind = (symbol.DeclaredAccessibility == Accessibility.Public) ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword;

            var keywordToken = typeDeclarationNode.Keyword;

            var replacementModifiers = DeclarationModifiersHelper.AddModifier(typeDeclarationNode.Modifiers, ref keywordToken, accessModifierKind);
            var replacementNode = ReplaceModifiers(typeDeclarationNode, replacementModifiers);
            replacementNode = ReplaceKeyword(replacementNode, keywordToken);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(typeDeclarationNode, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        // This code was copied from the Roslyn code base (and slightly modified). It can be removed if
        // TypeDeclarationSyntaxExtensions.WithModifiers is made public (Roslyn issue #2186)
        private static TypeDeclarationSyntax ReplaceModifiers(TypeDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            switch (node.Kind())
            {
            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)node).WithModifiers(modifiers);
            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)node).WithModifiers(modifiers);
            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)node).WithModifiers(modifiers);
            }

            return node;
        }

        // This code was copied from the Roslyn code base (and slightly modified). It can be removed if
        // TypeDeclarationSyntaxExtensions.WithModifiers is made public (Roslyn issue #2186)
        private static TypeDeclarationSyntax ReplaceKeyword(TypeDeclarationSyntax node, SyntaxToken keyword)
        {
            switch (node.Kind())
            {
            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)node).WithKeyword(keyword);
            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)node).WithKeyword(keyword);
            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)node).WithKeyword(keyword);
            }

            return node;
        }
    }
}
