// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements code fixes for <see cref="SA1212PropertyAccessorsMustFollowOrder"/> and <see cref="SA1213EventAccessorsMustFollowOrder"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1212SA1213CodeFixProvider))]
    [Shared]
    internal class SA1212SA1213CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1212PropertyAccessorsMustFollowOrder.DiagnosticId, SA1213EventAccessorsMustFollowOrder.DiagnosticId);

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

            if (HasLeadingBlankLines(secondAccessor))
            {
                trackedFirstAccessor = syntaxRoot.GetCurrentNode(firstAccesor);
                var newFirstAccessor = trackedFirstAccessor.WithLeadingTrivia(new[] { SyntaxFactory.CarriageReturnLineFeed }.Concat(firstAccesor.GetFirstToken().WithoutLeadingBlankLines().LeadingTrivia));
                syntaxRoot = syntaxRoot.ReplaceNode(trackedFirstAccessor, newFirstAccessor);
            }

            var trackedLastAccessor = syntaxRoot.GetCurrentNode(secondAccessor);
            var keepTriviaOptions = AccessorsAreOnTheSameLine(firstAccesor, secondAccessor)
                ? SyntaxRemoveOptions.KeepEndOfLine
                : SyntaxRemoveOptions.KeepNoTrivia;

            syntaxRoot = syntaxRoot.RemoveNode(trackedLastAccessor, keepTriviaOptions);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static bool HasLeadingBlankLines(SyntaxNode node)
        {
            var firstTriviaIgnoringWhitespace = node.GetLeadingTrivia().FirstOrDefault(x => !x.IsKind(SyntaxKind.WhitespaceTrivia));
            return firstTriviaIgnoringWhitespace.IsKind(SyntaxKind.EndOfLineTrivia);
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
                var leadingWhitespace = firstAccessor.GetLeadingTrivia().Where(x => x.IsKind(SyntaxKind.WhitespaceTrivia));
                newLeadingTrivia = SyntaxFactory.TriviaList(TriviaHelper.MergeTriviaLists(newLeadingTrivia, SyntaxTriviaList.Empty.AddRange(leadingWhitespace)));
            }

            var newAccessor = accessorList.Accessors[1]
                .WithBody(secondAccessor.Body)
                .WithLeadingTrivia(newLeadingTrivia);

            return newAccessor;
        }

        private static SyntaxTriviaList GetLeadingTriviaWithoutLeadingBlankLines(SyntaxNode secondAccessor)
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
