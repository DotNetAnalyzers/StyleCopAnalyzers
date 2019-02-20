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
    /// Implements a code fix for <see cref="SA1134AttributesMustNotShareLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1134CodeFixProvider))]
    [Shared]
    internal class SA1134CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1134AttributesMustNotShareLine.DiagnosticId);

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
                // Do not offer the code fix if the error is found at an invalid node (like IncompleteMemberSyntax)
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is AttributeListSyntax)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            ReadabilityResources.SA1134CodeFix,
                            cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                            nameof(SA1134CodeFixProvider)),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);

            var attributeListSyntax = (AttributeListSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);

            // use the containing type to determine the indentation level, anything else is less reliable.
            var containingType = attributeListSyntax.Parent?.Parent;
            var indentationSteps = (containingType != null) ? IndentationHelper.GetIndentationSteps(settings.Indentation, containingType) + 1 : 0;
            var indentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, indentationSteps);

            var tokensToReplace = new Dictionary<SyntaxToken, SyntaxToken>();

            if (diagnostic.Properties.ContainsKey(SA1134AttributesMustNotShareLine.FixWithNewLineBeforeKey))
            {
                var token = attributeListSyntax.OpenBracketToken;
                var prevToken = token.GetPreviousToken();

                tokensToReplace[prevToken] = prevToken.WithTrailingTrivia(prevToken.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.CarriageReturnLineFeed));

                var newLeadingTrivia = token.LeadingTrivia.Insert(0, indentationTrivia);
                tokensToReplace[token] = token.WithLeadingTrivia(newLeadingTrivia);
            }

            if (diagnostic.Properties.ContainsKey(SA1134AttributesMustNotShareLine.FixWithNewLineAfterKey))
            {
                var token = attributeListSyntax.CloseBracketToken;
                var nextToken = token.GetNextToken();

                tokensToReplace[token] = token.WithTrailingTrivia(token.TrailingTrivia.WithoutTrailingWhitespace().Add(SyntaxFactory.CarriageReturnLineFeed));

                var newLeadingTrivia = nextToken.LeadingTrivia.Insert(0, indentationTrivia);
                tokensToReplace[nextToken] = nextToken.WithLeadingTrivia(newLeadingTrivia);
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(tokensToReplace.Keys, (original, rewritten) => tokensToReplace[original]);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }
    }
}
