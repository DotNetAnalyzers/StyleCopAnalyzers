// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
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

        private static SyntaxToken GetBaseKeywordToken(SyntaxNode root, TextSpan sourceSpan)
        {
            return root.FindToken(sourceSpan.Start);
        }

        private static SyntaxToken RewriteBaseAsThis(SyntaxToken token)
        {
            // By creating a `base` token with the literal text `this`, we can replace a token with the code fix instead
            // of replacing an entire syntax node. This improves the performance of the code fix, but the resulting tree
            // could be considered in a bad state. However, this is not a problem under any interpretation of the result
            // because SA1100 is only reported in cases where `base.` and `this.` have the same meaning.
            return SyntaxFactory.Token(token.LeadingTrivia, SyntaxKind.BaseKeyword, "this", "this", token.TrailingTrivia);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = GetBaseKeywordToken(root, diagnostic.Location.SourceSpan);

            SyntaxNode newSyntaxRoot = root.ReplaceToken(token, RewriteBaseAsThis(token));
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => ReadabilityResources.SA1100CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);

                List<SyntaxToken> tokensToReplace = new List<SyntaxToken>(diagnostics.Length);
                foreach (var diagnostic in diagnostics)
                {
                    tokensToReplace.Add(GetBaseKeywordToken(syntaxRoot, diagnostic.Location.SourceSpan));
                }

                return syntaxRoot.ReplaceTokens(tokensToReplace, (originalToken, rewrittenToken) => RewriteBaseAsThis(rewrittenToken));
            }
        }
    }
}
