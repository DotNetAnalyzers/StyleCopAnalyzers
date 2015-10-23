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
    /// Implements code fixes for <see cref="SA1207ProtectedMustComeBeforeInternal"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1207CodeFixProvider))]
    [Shared]
    internal class SA1207CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1207ProtectedMustComeBeforeInternal.DiagnosticId);

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
                        OrderingResources.SA1207CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1207CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var originalDeclarationNode = syntaxRoot.FindNode(diagnostic.Location.SourceSpan) as MemberDeclarationSyntax;

            var childTokens = originalDeclarationNode?.ChildTokens();
            if (childTokens == null)
            {
                return document;
            }

            var newDeclarationNode = originalDeclarationNode.ReplaceTokens(childTokens, ComputeReplacementToken);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(originalDeclarationNode, newDeclarationNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxToken ComputeReplacementToken(SyntaxToken originalToken, SyntaxToken rewrittenToken)
        {
            if (originalToken.IsKind(SyntaxKind.InternalKeyword))
            {
                return SyntaxFactory.Token(SyntaxKind.ProtectedKeyword).WithTriviaFrom(rewrittenToken);
            }
            else if (originalToken.IsKind(SyntaxKind.ProtectedKeyword))
            {
                return SyntaxFactory.Token(SyntaxKind.InternalKeyword).WithTriviaFrom(rewrittenToken);
            }
            else
            {
                return rewrittenToken;
            }
        }
    }
}
