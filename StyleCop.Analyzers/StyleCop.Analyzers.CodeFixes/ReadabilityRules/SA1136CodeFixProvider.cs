// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1136EnumValuesShouldBeOnSeparateLines"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1136CodeFixProvider))]
    [Shared]
    internal class SA1136CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1136EnumValuesShouldBeOnSeparateLines.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1136CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1136CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var indentationOptions = IndentationOptions.FromDocument(document);

            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var enumMemberDeclarationFirstToken = enumMemberDeclaration.GetFirstToken();

            var enumDeclaration = (EnumDeclarationSyntax)enumMemberDeclaration.Parent;

            var memberIndex = enumDeclaration.Members.IndexOf(enumMemberDeclaration);
            var precedingSeparatorToken = enumDeclaration.Members.GetSeparator(memberIndex - 1);

            var parentIndentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, enumDeclaration);
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, parentIndentationSteps + 1);

            var replacements = new Dictionary<SyntaxToken, SyntaxToken>();

            var sharedTrivia = TriviaHelper.MergeTriviaLists(precedingSeparatorToken.TrailingTrivia, enumMemberDeclarationFirstToken.LeadingTrivia);

            var newTrailingTrivia = SyntaxFactory.TriviaList(sharedTrivia)
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

            replacements[precedingSeparatorToken] = precedingSeparatorToken.WithTrailingTrivia(newTrailingTrivia);
            replacements[enumMemberDeclarationFirstToken] = enumMemberDeclarationFirstToken.WithLeadingTrivia(indentation);

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replacements.Keys, (original, rewritten) => replacements[original]);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }
    }
}
