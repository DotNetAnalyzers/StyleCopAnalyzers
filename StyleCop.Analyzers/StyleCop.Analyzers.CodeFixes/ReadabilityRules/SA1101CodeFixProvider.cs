// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
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
    /// This class provides a code fix for <see cref="SA1101PrefixLocalCallsWithThis"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, insert the <c>this.</c> prefix before the call to the class
    /// member.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1101CodeFixProvider))]
    [Shared]
    internal class SA1101CodeFixProvider : CodeFixProvider
    {
        private static readonly ThisExpressionSyntax ThisExpressionSyntax = SyntaxFactory.ThisExpression();

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1101PrefixLocalCallsWithThis.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as SimpleNameSyntax;
                if (node == null)
                {
                    return;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1101CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, root, node),
                        nameof(SA1101CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, SimpleNameSyntax node)
        {
            var qualifiedExpression =
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpressionSyntax, node.WithoutTrivia().WithoutFormatting())
                .WithTriviaFrom(node)
                .WithoutFormatting();

            var newSyntaxRoot = root.ReplaceNode(node, qualifiedExpression);

            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                   new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1101CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                List<SyntaxNode> nodesNeedingQualification = new List<SyntaxNode>(diagnostics.Length);

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) as SimpleNameSyntax;
                    if (node == null || node.IsMissing)
                    {
                        continue;
                    }

                    nodesNeedingQualification.Add(node);
                }

                return syntaxRoot.ReplaceNodes(nodesNeedingQualification, (originalNode, rewrittenNode) =>
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpressionSyntax, (SimpleNameSyntax)rewrittenNode.WithoutTrivia().WithoutFormatting())
                .WithTriviaFrom(rewrittenNode)
                .WithoutFormatting());
            }
        }
    }
}
