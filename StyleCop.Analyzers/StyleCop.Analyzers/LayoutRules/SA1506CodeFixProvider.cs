﻿namespace StyleCop.Analyzers.LayoutRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1506CodeFixProvider))]
    [Shared]
    public class SA1506CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(LayoutResources.SA1506CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1506CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var triviaList = token.LeadingTrivia;

            var index = triviaList.Count - 1;
            while (!triviaList[index].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                index--;
            }

            var lastEndOfLine = index;

            while (!triviaList[index].IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
            {
                index--;
            }

            var lastDocumentation = index;

            var newLeadingTrivia = triviaList.Take(lastDocumentation + 1).Concat(triviaList.Skip(lastEndOfLine + 1));
            var newSyntaxRoot = syntaxRoot.ReplaceToken(token, token.WithLeadingTrivia(newLeadingTrivia));

            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
