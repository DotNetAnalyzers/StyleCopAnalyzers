// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
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
    /// Implements a code fix for <see cref="SA1128ConstructorInitializerMustBeOnOwnLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, place the constructor initializer on its own line.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1128CodeFixProvider))]
    [Shared]
    internal class SA1128CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1128ConstructorInitializerMustBeOnOwnLine.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1128CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1128CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var indentationOptions = IndentationOptions.FromDocument(document);
            var newLine = FormattingHelper.GetNewLineTrivia(document);

            var constructorInitializer = (ConstructorInitializerSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            var constructorDeclaration = (ConstructorDeclarationSyntax)constructorInitializer.Parent;

            var newConstructorDeclaration = ReformatConstructorDeclaration(constructorDeclaration, indentationOptions, newLine);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(constructorDeclaration, newConstructorDeclaration);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static ConstructorDeclarationSyntax ReformatConstructorDeclaration(ConstructorDeclarationSyntax constructorDeclaration, IndentationOptions indentationOptions, SyntaxTrivia newLine)
        {
            var constructorInitializer = constructorDeclaration.Initializer;

            var newParameterList = constructorDeclaration.ParameterList
                .WithTrailingTrivia(constructorDeclaration.ParameterList.GetTrailingTrivia().WithoutTrailingWhitespace().Add(newLine));

            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, constructorDeclaration);
            var indentation = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps + 1);

            var newColonTrailingTrivia = constructorInitializer.ColonToken.TrailingTrivia.WithoutTrailingWhitespace();

            var newColonToken = constructorInitializer.ColonToken
                .WithLeadingTrivia(indentation)
                .WithTrailingTrivia(newColonTrailingTrivia);

            var newInitializer = constructorInitializer
                .WithColonToken(newColonToken)
                .WithThisOrBaseKeyword(constructorInitializer.ThisOrBaseKeyword.WithLeadingTrivia(SyntaxFactory.Space));

            return constructorDeclaration
                .WithParameterList(newParameterList)
                .WithInitializer(newInitializer);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle { get; } =
                ReadabilityResources.SA1128CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var indentationOptions = IndentationOptions.FromDocument(document);
                var newLine = FormattingHelper.GetNewLineTrivia(document);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan).Parent);

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => ReformatConstructorDeclaration((ConstructorDeclarationSyntax)rewrittenNode, indentationOptions, newLine));
            }
        }
    }
}