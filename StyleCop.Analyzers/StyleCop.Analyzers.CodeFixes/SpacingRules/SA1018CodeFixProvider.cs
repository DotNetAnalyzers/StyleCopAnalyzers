// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
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
    /// Implements a code fix for <see cref="SA1018NullableTypeSymbolsMustNotBePrecededBySpace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that there is no whitespace before the nullable type
    /// symbol.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1018CodeFixProvider))]
    [Shared]
    internal class SA1018CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1018NullableTypeSymbolsMustNotBePrecededBySpace.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var nullableType = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as NullableTypeSyntax;
                if (nullableType == null)
                {
                    continue;
                }

                var questionToken = nullableType.QuestionToken;
                var precedingToken = questionToken.GetPreviousToken();
                var triviaList = precedingToken.TrailingTrivia.AddRange(questionToken.LeadingTrivia);

                if (triviaList.Any(UnsupportedTriviaKind))
                {
                    // cannot only automatically fix multiline comments (/* ... */)
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1018CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1018CodeFixProvider)),
                    diagnostic);
            }
        }

        private static bool UnsupportedTriviaKind(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
            case SyntaxKind.WhitespaceTrivia:
            case SyntaxKind.EndOfLineTrivia:
            case SyntaxKind.MultiLineCommentTrivia:
                return false;

            default:
                return true;
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var nullableType = (NullableTypeSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var questionToken = nullableType.QuestionToken;
            var precedingToken = questionToken.GetPreviousToken();

            var triviaList = precedingToken.TrailingTrivia.AddRange(questionToken.LeadingTrivia);
            var correctedTriviaList = triviaList.Where(t => !t.IsKind(SyntaxKind.WhitespaceTrivia) && !t.IsKind(SyntaxKind.EndOfLineTrivia));

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [precedingToken] = precedingToken.WithTrailingTrivia(correctedTriviaList),
                [questionToken] = questionToken.WithLeadingTrivia()
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
