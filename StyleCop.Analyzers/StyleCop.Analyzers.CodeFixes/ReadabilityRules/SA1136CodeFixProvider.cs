// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);

            var enumMemberDeclaration = (EnumMemberDeclarationSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var enumDeclaration = (EnumDeclarationSyntax)enumMemberDeclaration.Parent;

            var memberIndex = enumDeclaration.Members.IndexOf(enumMemberDeclaration);
            var precedingSeparatorToken = enumDeclaration.Members.GetSeparator(memberIndex - 1);

            // determine the indentation for enum members (which is parent + 1 step)
            var parentIndentationSteps = IndentationHelper.GetIndentationSteps(settings.Indentation, enumDeclaration);
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, parentIndentationSteps + 1);

            // combine all trivia between the separator and the enum member and place them after the separator, followed by a new line.
            var enumMemberDeclarationFirstToken = enumMemberDeclaration.GetFirstToken();
            var sharedTrivia = TriviaHelper.MergeTriviaLists(precedingSeparatorToken.TrailingTrivia, enumMemberDeclarationFirstToken.LeadingTrivia);

            var newTrailingTrivia = SyntaxFactory.TriviaList(sharedTrivia)
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

            // replace the trivia for the tokens
            var replacements = new Dictionary<SyntaxToken, SyntaxToken>
            {
                [precedingSeparatorToken] = precedingSeparatorToken.WithTrailingTrivia(newTrailingTrivia),
                [enumMemberDeclarationFirstToken] = enumMemberDeclarationFirstToken.WithLeadingTrivia(indentation),
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replacements.Keys, (original, rewritten) => replacements[original]);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }
    }
}
