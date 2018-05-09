// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// This class provides a code fix for <see cref="SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, change the <c>base.</c> prefix to <c>this.</c>.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1100CodeFixProvider))]
    [Shared]
    internal class SA1100CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1100CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1100CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static BaseExpressionSyntax GetBaseExpressionNode(SyntaxNode root, TextSpan sourceSpan)
        {
            return (BaseExpressionSyntax)root.FindToken(sourceSpan.Start).Parent;
        }

        private static ThisExpressionSyntax RewriteBaseAsThis(BaseExpressionSyntax token)
        {
            return SyntaxFactory.ThisExpression(SyntaxFactory.Token(SyntaxKind.ThisKeyword).WithTriviaFrom(token.Token));
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = GetBaseExpressionNode(root, diagnostic.Location.SourceSpan);

            SyntaxNode newSyntaxRoot = root.ReplaceNode(node, RewriteBaseAsThis(node));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => ReadabilityResources.SA1100CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<BaseExpressionSyntax> nodesToReplace = new List<BaseExpressionSyntax>(diagnostics.Length);
                foreach (var diagnostic in diagnostics)
                {
                    nodesToReplace.Add(GetBaseExpressionNode(syntaxRoot, diagnostic.Location.SourceSpan));
                }

                return syntaxRoot.ReplaceNodes(nodesToReplace, (originalNode, rewrittenNode) => RewriteBaseAsThis(rewrittenNode));
            }
        }
    }
}
