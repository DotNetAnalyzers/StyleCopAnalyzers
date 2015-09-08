﻿namespace StyleCop.Analyzers.OrderingRules
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
    /// Implements code fixes for <see cref="SA1212PropertyAccessorsMustFollowOrder"/> and <see cref="SA1213EventAccessorsMustFollowOrder"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1212SA1213CodeFixProvider))]
    [Shared]
    public class SA1212SA1213CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1212PropertyAccessorsMustFollowOrder.DiagnosticId, SA1213EventAccessorsMustFollowOrder.DiagnosticId);

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
                        nameof(SA1212SA1213CodeFixProvider)),
                    diagnostic);
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
            return firstAccesor.GetEndLine() == secondAccessor.GetEndLine();
        }

        private static AccessorDeclarationSyntax GetNewAccessor(AccessorListSyntax accessorList, AccessorDeclarationSyntax firstAccessor, AccessorDeclarationSyntax secondAccessor)
        {
            var newLeadingTrivia = GetLeadingTriviaWithoutLeadingBlankLines(secondAccessor);
            if (AccessorsAreOnTheSameLine(firstAccessor, secondAccessor))
            {
                var leadingWhitespace = firstAccessor.GetLeadingTrivia().Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia)).ToList();
                newLeadingTrivia = SyntaxFactory.TriviaList(TriviaHelper.MergeTriviaLists(newLeadingTrivia, leadingWhitespace));
            }

            var newAccessor = accessorList.Accessors[1]
                .WithBody(secondAccessor.Body)
                .WithLeadingTrivia(newLeadingTrivia);

            if (secondAccessor.GetFirstToken().HasLeadingBlankLines())
            {
                newAccessor = newAccessor.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);
            }

            return newAccessor;
        }

        private static SyntaxTriviaList GetLeadingTriviaWithoutLeadingBlankLines(AccessorDeclarationSyntax secondAccessor)
        {
            var leadingTrivia = secondAccessor.GetLeadingTrivia();

            var skipIndex = 0;
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    skipIndex = i + 1;
                }
                else if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    // Preceded by whitespace
                    skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    break;
                }
            }

            return SyntaxFactory.TriviaList(leadingTrivia.Skip(skipIndex));
        }
    }
}
