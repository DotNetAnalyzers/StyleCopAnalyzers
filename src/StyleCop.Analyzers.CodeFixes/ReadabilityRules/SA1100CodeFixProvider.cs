// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;

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
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                var node = GetBaseExpressionNode(root, diagnostic.Location.SourceSpan);
                if (!CanReplaceBaseWithThis(node, semanticModel, context.CancellationToken))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1100CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1100CodeFixProvider)),
                    diagnostic);
            }
        }

        private static bool CanReplaceBaseWithThis(BaseExpressionSyntax node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var symbol = semanticModel.GetSymbolInfo(node.Parent, cancellationToken).Symbol;
            if (symbol == null)
            {
                return false;
            }

            if (symbol.IsSealed || (!symbol.IsVirtual && !symbol.IsOverride))
            {
                return true;
            }

            // A subclass can override the member even when the current class does not.
            var enclosingSymbol = semanticModel.GetEnclosingSymbol(node.SpanStart, cancellationToken);
            return enclosingSymbol?.ContainingType?.IsSealed == true;
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

            protected override async Task<SyntaxNode?> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                List<BaseExpressionSyntax> nodesToReplace = new List<BaseExpressionSyntax>(diagnostics.Length);
                foreach (var diagnostic in diagnostics)
                {
                    var node = GetBaseExpressionNode(syntaxRoot, diagnostic.Location.SourceSpan);
                    if (CanReplaceBaseWithThis(node, semanticModel, fixAllContext.CancellationToken))
                    {
                        nodesToReplace.Add(node);
                    }
                }

                return nodesToReplace.Count == 0
                    ? null
                    : syntaxRoot.ReplaceNodes(nodesToReplace, (originalNode, rewrittenNode) => RewriteBaseAsThis(rewrittenNode));
            }
        }
    }
}
