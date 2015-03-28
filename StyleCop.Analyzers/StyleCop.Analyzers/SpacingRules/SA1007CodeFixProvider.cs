﻿namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    /// <summary>
    /// Implements a code fix for <see cref="SA1007OperatorKeywordMustBeFollowedBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add a single space after the operator keyword.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1007CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1007CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1007OperatorKeywordMustBeFollowedBySpace.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1007OperatorKeywordMustBeFollowedBySpace.DiagnosticId))
                {
                    continue;
                }

                SyntaxToken token = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (token.IsMissing)
                {
                    continue;
                }

                if (token.HasTrailingTrivia && token.TrailingTrivia[0].IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    continue;
                }

                context.RegisterCodeFix(CodeAction.Create("Insert space", t => GetTransformedDocument(context.Document, root, token)), diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocument(Document document, SyntaxNode root, SyntaxToken token)
        {
            SyntaxTrivia whitespace = SyntaxFactory.Whitespace(" ");
            SyntaxToken corrected = token.WithTrailingTrivia(token.TrailingTrivia.Insert(0, whitespace)).WithoutFormatting();
            Document updatedDocument = document.WithSyntaxRoot(root.ReplaceToken(token, corrected));

            return Task.FromResult(updatedDocument);
        }
    }
}
