namespace StyleCop.Analyzers.OrderingRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements code fixes for <see cref="SA1213EventAccessorsMustFollowOrder"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1213CodeFixProvider))]
    [Shared]
    public class SA1213CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1213EventAccessorsMustFollowOrder.DiagnosticId);

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
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.SA1213CodeFix,
                        token => GetTransformedDocumentAsync(context.Document, diagnostic, token),
                        nameof(SA1213CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var accessorToken = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
            var accessorList = (AccessorListSyntax)accessorToken.Parent.Parent;

            var firstAccesor = accessorList.Accessors[0];
            var secondAccessor = accessorList.Accessors[1];
            var trackedRoot = syntaxRoot.TrackNodes(firstAccesor, secondAccessor);
            var trackedFirstAccessor = trackedRoot.GetCurrentNode(firstAccesor);
            var newAccessor = GetNewAccessor(accessorList, firstAccesor, secondAccessor);

            syntaxRoot = trackedRoot.InsertNodesBefore(trackedFirstAccessor, new[] { newAccessor });

            var trackedLastAccessor = syntaxRoot.GetCurrentNode(secondAccessor);
            var keepTriviaOptions = AccessorsAreOnTheSameLine(firstAccesor, secondAccessor)
                ? SyntaxRemoveOptions.KeepEndOfLine
                : SyntaxRemoveOptions.KeepNoTrivia;

            syntaxRoot = syntaxRoot.RemoveNode(trackedLastAccessor, keepTriviaOptions);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static bool AccessorsAreOnTheSameLine(AccessorDeclarationSyntax firstAccesor, AccessorDeclarationSyntax secondAccessor)
        {
            return firstAccesor.GetLocation().GetLineSpan().EndLinePosition.Line ==
                   secondAccessor.GetLocation().GetLineSpan().EndLinePosition.Line;
        }

        private static AccessorDeclarationSyntax GetNewAccessor(AccessorListSyntax accessorList, AccessorDeclarationSyntax firstAccessor, AccessorDeclarationSyntax secondAccessor)
        {
            var newAccessor = accessorList.Accessors[1]
                .WithBody(secondAccessor.Body)
                .WithLeadingTrivia(firstAccessor.GetLeadingTrivia());
            var trailingTrivia = newAccessor.GetTrailingTrivia();
            if (!trailingTrivia.Any(x => x.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                newAccessor = newAccessor.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
            }

            return newAccessor;
        }
    }
}