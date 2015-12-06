// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    /// <summary>
    /// Implements a code fix for <see cref="SA1653UseTrailingCommasInMultiLineInitializers"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1653CodeFixProvider))]
    [Shared]
    internal class SA1653CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1653UseTrailingCommasInMultiLineInitializers.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        MaintainabilityResources.SA1653CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1653CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var parent = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).Parent;

            SyntaxNode newParent = null;
            switch (parent.Kind())
            {
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.ArrayInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                    newParent = RewriteInitializer((InitializerExpressionSyntax)parent);
                    break;

                case SyntaxKind.AnonymousObjectCreationExpression:
                    newParent = RewriteAnonymousObjectInitializer((AnonymousObjectCreationExpressionSyntax)parent);
                    break;

                default:
                    throw new InvalidOperationException("Unknown initializer type: " + parent.Kind());
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNode(parent, newParent);

            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
            return newDocument;
        }

        private static SyntaxNode RewriteInitializer(InitializerExpressionSyntax initializer)
        {
            var existingItems = new List<ExpressionSyntax>(initializer.Expressions);
            var last = existingItems.Last();
            existingItems.Remove(last);
            existingItems.Add(last.WithoutTrailingTrivia());

            var existingSeparators = initializer.Expressions.GetSeparators();
            var newSeparators = new List<SyntaxToken>(existingSeparators);
            newSeparators.Add(SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(last.GetTrailingTrivia()));

            var newInitializerExpressions = SyntaxFactory.SeparatedList(
                existingItems,
                newSeparators);

            var fixedInitializer = initializer.WithExpressions(newInitializerExpressions);
            return fixedInitializer;
        }

        private static SyntaxNode RewriteAnonymousObjectInitializer(AnonymousObjectCreationExpressionSyntax initializer)
        {
            var existingItems = new List<AnonymousObjectMemberDeclaratorSyntax>(initializer.Initializers);
            var last = existingItems.Last();
            existingItems.Remove(last);
            existingItems.Add(last.WithoutTrailingTrivia());

            var existingSeparators = initializer.Initializers.GetSeparators();
            var newSeparators = new List<SyntaxToken>(existingSeparators);
            newSeparators.Add(SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(last.GetTrailingTrivia()));

            var newInitializerExpressions = SyntaxFactory.SeparatedList(
                existingItems,
                newSeparators);

            var fixedInitializer = initializer.WithInitializers(newInitializerExpressions);
            return fixedInitializer;
        }
    }
}
