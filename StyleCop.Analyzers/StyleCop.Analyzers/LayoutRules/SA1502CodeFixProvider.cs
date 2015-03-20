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
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using StyleCop.Analyzers.Helpers;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a code fix for <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    [ExportCodeFixProvider(nameof(SA1502CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1502CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
            ImmutableArray.Create(SA1502ElementMustNotBeOnASingleLine.DiagnosticId);

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
                context.RegisterCodeFix(CodeAction.Create("SA1502: Expand element", token => this.GetTransformedDocumentAsync(context, diagnostic, token)), diagnostic);
            }

            return Task.FromResult(true);
        }

        private async Task<Document> GetTransformedDocumentAsync(CodeFixContext context, Diagnostic diagnostic, CancellationToken token)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var newDocument = this.CreateCodeFix(context, diagnostic, syntaxRoot);

            return newDocument;
        }

        private Document CreateCodeFix(CodeFixContext context, Diagnostic diagnostic, SyntaxNode syntaxRoot)
        {
            SyntaxNode newSyntaxRoot = syntaxRoot;
            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var indentationOptions = IndentationOptions.FromDocument(context.Document);

            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.StructDeclaration:
                    newSyntaxRoot = this.RegisterTypeDeclarationCodeFix(syntaxRoot, (TypeDeclarationSyntax)node, indentationOptions);
                    break;

                case SyntaxKind.AccessorList:
                    newSyntaxRoot = this.RegisterPropertyLikeDeclarationCodeFix(syntaxRoot, (BasePropertyDeclarationSyntax)node.Parent, indentationOptions);
                    break;

                case SyntaxKind.Block:
                    newSyntaxRoot = this.RegisterMethodLikeDeclarationCodeFix(syntaxRoot, (BaseMethodDeclarationSyntax)node.Parent, indentationOptions);
                    break;

                case SyntaxKind.EnumDeclaration:
                    newSyntaxRoot = this.RegisterEnumDeclarationCodeFix(syntaxRoot, (EnumDeclarationSyntax)node, indentationOptions);
                    break;

                case SyntaxKind.NamespaceDeclaration:
                    newSyntaxRoot = this.RegisterNamespaceDeclarationCodeFix(syntaxRoot, (NamespaceDeclarationSyntax)node, indentationOptions);
                    break;
            }

            return context.Document.WithSyntaxRoot(newSyntaxRoot);
        }

        private SyntaxNode RegisterTypeDeclarationCodeFix(SyntaxNode syntaxRoot, TypeDeclarationSyntax node, IndentationOptions indentationOptions)
        {
            return this.ReformatElement(syntaxRoot, node, node.OpenBraceToken, node.CloseBraceToken, indentationOptions);
        }

        private SyntaxNode RegisterPropertyLikeDeclarationCodeFix(SyntaxNode syntaxRoot, BasePropertyDeclarationSyntax node, IndentationOptions indentationOptions)
        {
            return this.ReformatElement(syntaxRoot, node, node.AccessorList.OpenBraceToken, node.AccessorList.CloseBraceToken, indentationOptions);
        }

        private SyntaxNode RegisterMethodLikeDeclarationCodeFix(SyntaxNode syntaxRoot, BaseMethodDeclarationSyntax node, IndentationOptions indentationOptions)
        {
            return this.ReformatElement(syntaxRoot, node, node.Body.OpenBraceToken, node.Body.CloseBraceToken, indentationOptions);
        }

        private SyntaxNode RegisterEnumDeclarationCodeFix(SyntaxNode syntaxRoot, EnumDeclarationSyntax node, IndentationOptions indentationOptions)
        {
            return this.ReformatElement(syntaxRoot, node, node.OpenBraceToken, node.CloseBraceToken, indentationOptions);
        }

        private SyntaxNode RegisterNamespaceDeclarationCodeFix(SyntaxNode syntaxRoot, NamespaceDeclarationSyntax node, IndentationOptions indentationOptions)
        {
            return this.ReformatElement(syntaxRoot, node, node.OpenBraceToken, node.CloseBraceToken, indentationOptions);
        }

        private SyntaxNode ReformatElement(SyntaxNode syntaxRoot, SyntaxNode element, SyntaxToken openBraceToken, SyntaxToken closeBraceToken, IndentationOptions indentationOptions)
        {
            var tokenSubstitutions = new Dictionary<SyntaxToken, SyntaxToken>();

            var parentLastToken = openBraceToken.GetPreviousToken();
            var parentEndLine = parentLastToken.GetLocation().GetLineSpan().EndLinePosition.Line;
            var blockStartLine = openBraceToken.GetLocation().GetLineSpan().StartLinePosition.Line;

            // reformat parent if it is on the same line as the block.
            if (parentEndLine == blockStartLine)
            {
                var newTrailingTrivia = parentLastToken.TrailingTrivia
                .WithoutTrailingWhitespace()
                .Add(SyntaxFactory.CarriageReturnLineFeed);

                tokenSubstitutions.Add(parentLastToken, parentLastToken.WithTrailingTrivia(newTrailingTrivia));
            }

            var parentIndentationLevel = IndentationHelper.GetNodeIndentationSteps(indentationOptions, element);
            var indentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel);
            var contentIndentationString = IndentationHelper.GenerateIndentationString(indentationOptions, parentIndentationLevel + 1);

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
            private Dictionary<SyntaxToken, SyntaxToken> tokensToReplace;

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
