// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.LayoutRules
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
    using StyleCop.Analyzers.Lightup;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1502CodeFixProvider))]
    [Shared]
    internal class SA1502CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1502ElementMustNotBeOnASingleLine.DiagnosticId);

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
                        LayoutResources.SA1502CodeFix,
                        cancellationToken => this.GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1502CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, cancellationToken);
            var newDocument = this.CreateCodeFix(document, settings.Indentation, diagnostic, syntaxRoot);

            return newDocument;
        }

        private Document CreateCodeFix(Document document, IndentationSettings indentationSettings, Diagnostic diagnostic, SyntaxNode syntaxRoot)
        {
            SyntaxNode newSyntaxRoot = syntaxRoot;
            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);

            switch (node.Kind())
            {
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.InterfaceDeclaration:
            case SyntaxKind.StructDeclaration:
            case SyntaxKind.EnumDeclaration:
                newSyntaxRoot = this.RegisterBaseTypeDeclarationCodeFix(syntaxRoot, (BaseTypeDeclarationSyntax)node, indentationSettings);
                break;

            case SyntaxKind.AccessorList:
                newSyntaxRoot = this.RegisterPropertyLikeDeclarationCodeFix(syntaxRoot, (BasePropertyDeclarationSyntax)node.Parent, indentationSettings);
                break;

            case SyntaxKind.Block:
                if (node.Parent.IsKind(SyntaxKindEx.LocalFunctionStatement))
                {
                    newSyntaxRoot = this.RegisterLocalFunctionStatementCodeFix(syntaxRoot, (LocalFunctionStatementSyntaxWrapper)node.Parent, indentationSettings);
                }
                else
                {
                    newSyntaxRoot = this.RegisterMethodLikeDeclarationCodeFix(syntaxRoot, (BaseMethodDeclarationSyntax)node.Parent, indentationSettings);
                }

                break;

            case SyntaxKind.NamespaceDeclaration:
                newSyntaxRoot = this.RegisterNamespaceDeclarationCodeFix(syntaxRoot, (NamespaceDeclarationSyntax)node, indentationSettings);
                break;
            }

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private SyntaxNode RegisterBaseTypeDeclarationCodeFix(SyntaxNode syntaxRoot, BaseTypeDeclarationSyntax node, IndentationSettings indentationSettings)
        {
            return this.ReformatElement(syntaxRoot, node, node.OpenBraceToken, node.CloseBraceToken, indentationSettings);
        }

        private SyntaxNode RegisterPropertyLikeDeclarationCodeFix(SyntaxNode syntaxRoot, BasePropertyDeclarationSyntax node, IndentationSettings indentationSettings)
        {
            return this.ReformatElement(syntaxRoot, node, node.AccessorList.OpenBraceToken, node.AccessorList.CloseBraceToken, indentationSettings);
        }

        private SyntaxNode RegisterMethodLikeDeclarationCodeFix(SyntaxNode syntaxRoot, BaseMethodDeclarationSyntax node, IndentationSettings indentationSettings)
        {
            return this.ReformatElement(syntaxRoot, node, node.Body.OpenBraceToken, node.Body.CloseBraceToken, indentationSettings);
        }

        private SyntaxNode RegisterLocalFunctionStatementCodeFix(SyntaxNode syntaxRoot, LocalFunctionStatementSyntaxWrapper node, IndentationSettings indentationSettings)
        {
            return this.ReformatElement(syntaxRoot, node, node.Body.OpenBraceToken, node.Body.CloseBraceToken, indentationSettings);
        }

        private SyntaxNode RegisterNamespaceDeclarationCodeFix(SyntaxNode syntaxRoot, NamespaceDeclarationSyntax node, IndentationSettings indentationSettings)
        {
            return this.ReformatElement(syntaxRoot, node, node.OpenBraceToken, node.CloseBraceToken, indentationSettings);
        }

        private SyntaxNode ReformatElement(SyntaxNode syntaxRoot, SyntaxNode element, SyntaxToken openBraceToken, SyntaxToken closeBraceToken, IndentationSettings indentationSettings)
        {
            var tokenSubstitutions = new Dictionary<SyntaxToken, SyntaxToken>();

            var parentLastToken = openBraceToken.GetPreviousToken();
            var parentEndLine = parentLastToken.GetLineSpan().EndLinePosition.Line;
            var blockStartLine = openBraceToken.GetLineSpan().StartLinePosition.Line;

            // reformat parent if it is on the same line as the block.
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

                tokenSubstitutions.Add(parentLastToken, parentLastToken.WithTrailingTrivia(newTrailingTrivia));
            }

            var parentIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, element);
            var indentationString = IndentationHelper.GenerateIndentationString(indentationSettings, parentIndentationLevel);
            var contentIndentationString = IndentationHelper.GenerateIndentationString(indentationSettings, parentIndentationLevel + 1);

            // reformat opening brace
            tokenSubstitutions.Add(openBraceToken, this.FormatBraceToken(openBraceToken, indentationString));

            // reformat start of content
            var startOfContentToken = openBraceToken.GetNextToken();
            if (startOfContentToken != closeBraceToken)
            {
                var newStartOfContentTokenLeadingTrivia = startOfContentToken.LeadingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.Whitespace(contentIndentationString));

                tokenSubstitutions.Add(startOfContentToken, startOfContentToken.WithLeadingTrivia(newStartOfContentTokenLeadingTrivia));
            }

            // reformat end of content
            var endOfContentToken = closeBraceToken.GetPreviousToken();
            if (endOfContentToken != openBraceToken)
            {
                var newEndOfContentTokenTrailingTrivia = endOfContentToken.TrailingTrivia
                    .WithoutTrailingWhitespace()
                    .Add(SyntaxFactory.CarriageReturnLineFeed);

                // check if the token already exists (occurs when there is only one token in the block)
                if (tokenSubstitutions.ContainsKey(endOfContentToken))
                {
                    tokenSubstitutions[endOfContentToken] = tokenSubstitutions[endOfContentToken].WithTrailingTrivia(newEndOfContentTokenTrailingTrivia);
                }
                else
                {
                    tokenSubstitutions.Add(endOfContentToken, endOfContentToken.WithTrailingTrivia(newEndOfContentTokenTrailingTrivia));
                }
            }

            // reformat closing brace
            tokenSubstitutions.Add(closeBraceToken, this.FormatBraceToken(closeBraceToken, indentationString));

            var rewriter = new TokenRewriter(tokenSubstitutions);
            var newSyntaxRoot = rewriter.Visit(syntaxRoot);

            return newSyntaxRoot;
        }

        private SyntaxToken FormatBraceToken(SyntaxToken braceToken, string indentationString)
        {
            var newBraceTokenLeadingTrivia = braceToken.LeadingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.Whitespace(indentationString));

            var newBraceTokenTrailingTrivia = braceToken.TrailingTrivia
                .WithoutTrailingWhitespace();

            // only add an end-of-line to the brace if there is none yet.
            if ((newBraceTokenTrailingTrivia.Count == 0) || !newBraceTokenTrailingTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia))
            {
                newBraceTokenTrailingTrivia = newBraceTokenTrailingTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
            }

            return braceToken
                .WithLeadingTrivia(newBraceTokenLeadingTrivia)
                .WithTrailingTrivia(newBraceTokenTrailingTrivia);
        }

        private class TokenRewriter : CSharpSyntaxRewriter
        {
            private readonly Dictionary<SyntaxToken, SyntaxToken> tokensToReplace;

            public TokenRewriter(Dictionary<SyntaxToken, SyntaxToken> tokensToReplace)
            {
                this.tokensToReplace = tokensToReplace;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                SyntaxToken replacementToken;

                if (this.tokensToReplace.TryGetValue(token, out replacementToken))
                {
                    return replacementToken;
                }

                return base.VisitToken(token);
            }
        }
    }
}
