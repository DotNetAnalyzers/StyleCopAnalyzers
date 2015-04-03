namespace StyleCop.Analyzers.LayoutRules
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1513ClosingCurlyBracketMustBeFollowedByBlankLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure a blank line follows closing curly brackets.</para>
    /// </remarks>
    [ExportCodeFixProvider(nameof(SA1513CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1513CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1513ClosingCurlyBracketMustBeFollowedByBlankLine.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create("Insert blank line after curly brace", token => GetTransformedDocumentAsync(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.End);

            var newTrivia = token.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
            var newToken = token.WithLeadingTrivia(newTrivia);
            var newSyntaxRoot = syntaxRoot.ReplaceToken(token, newToken);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return newDocument;
        }
    }
}
