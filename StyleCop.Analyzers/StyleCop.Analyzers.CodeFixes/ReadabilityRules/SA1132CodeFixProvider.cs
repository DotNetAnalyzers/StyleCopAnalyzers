﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
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
    using Microsoft.CodeAnalysis.Editing;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1132DoNotCombineFields"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, declare each field as part of its own field definition.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1132CodeFixProvider))]
    [Shared]
    internal class SA1132CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1132DoNotCombineFields.DiagnosticId);

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
                        ReadabilityResources.SA1132CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1132CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var baseFieldDeclaration = (BaseFieldDeclarationSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            List<BaseFieldDeclarationSyntax> newFieldDeclarations = SplitDeclaration(document, baseFieldDeclaration);

            if (newFieldDeclarations != null)
            {
                var editor = new SyntaxEditor(syntaxRoot, document.Project.Solution.Workspace);
                editor.InsertAfter(baseFieldDeclaration, newFieldDeclarations);
                editor.RemoveNode(baseFieldDeclaration, SyntaxRemoveOptions.KeepNoTrivia);
                return document.WithSyntaxRoot(editor.GetChangedRoot().WithoutFormatting());
            }

            return document;
        }

        private static List<BaseFieldDeclarationSyntax> SplitDeclaration(Document document, BaseFieldDeclarationSyntax baseFieldDeclaration)
        {
            if (baseFieldDeclaration is FieldDeclarationSyntax fieldDeclaration)
            {
                return DeclarationSplitter(
                    document,
                    fieldDeclaration.Declaration,
                    fieldDeclaration.WithDeclaration,
                    fieldDeclaration.SemicolonToken.TrailingTrivia);
            }

            if (baseFieldDeclaration is EventFieldDeclarationSyntax eventFieldDeclaration)
            {
                return DeclarationSplitter(
                    document,
                    eventFieldDeclaration.Declaration,
                    eventFieldDeclaration.WithDeclaration,
                    eventFieldDeclaration.SemicolonToken.TrailingTrivia);
            }

            return null;
        }

        private static List<BaseFieldDeclarationSyntax> DeclarationSplitter(
            Document document,
            VariableDeclarationSyntax declaration,
            Func<VariableDeclarationSyntax, BaseFieldDeclarationSyntax> declarationFactory,
            SyntaxTriviaList declarationTrailingTrivia)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;
            VariableDeclaratorSyntax first = variables.First();
            BaseFieldDeclarationSyntax previous = null;
            var newFieldDeclarations = new List<BaseFieldDeclarationSyntax>(variables.Count);

            foreach (SyntaxNodeOrToken nodeOrToken in variables.GetWithSeparators())
            {
                if (previous == null)
                {
                    VariableDeclaratorSyntax variable = (VariableDeclaratorSyntax)nodeOrToken.AsNode();
                    variable = variable.WithIdentifier(variable.Identifier.WithoutLeadingWhitespace());
                    var variableDeclarator = SyntaxFactory.SingletonSeparatedList(variable);
                    previous = declarationFactory(declaration.WithVariables(variableDeclarator));

                    if (variable != first)
                    {
                        var triviaList = previous.GetLeadingTrivia().WithoutDirectiveTrivia();

                        // Remove all leading blank lines
                        var nonBlankLinetriviaIndex = TriviaHelper.IndexOfFirstNonBlankLineTrivia(triviaList);
                        if (nonBlankLinetriviaIndex > 0)
                        {
                            triviaList = triviaList.RemoveRange(0, nonBlankLinetriviaIndex);
                        }

                        // Add a blank line if the first line contains a comment.
                        var nonWhitespaceTriviaIndex = TriviaHelper.IndexOfFirstNonWhitespaceTrivia(triviaList, false);
                        if (nonWhitespaceTriviaIndex >= 0)
                        {
                            switch (triviaList[nonWhitespaceTriviaIndex].Kind())
                            {
                            case SyntaxKind.SingleLineCommentTrivia:
                            case SyntaxKind.MultiLineCommentTrivia:
                                triviaList = triviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
                                break;
                            }
                        }

                        previous = previous.WithLeadingTrivia(triviaList);
                    }
                }
                else
                {
                    SyntaxToken commaToken = nodeOrToken.AsToken();
                    SyntaxTriviaList trailingTrivia = commaToken.TrailingTrivia;
                    if (trailingTrivia.Any())
                    {
                        if (!trailingTrivia.Last().IsKind(SyntaxKind.EndOfLineTrivia))
                        {
                            trailingTrivia = trailingTrivia.WithoutTrailingWhitespace().Add(FormattingHelper.GetNewLineTrivia(document));
                        }
                    }
                    else
                    {
                        trailingTrivia = SyntaxTriviaList.Create(FormattingHelper.GetNewLineTrivia(document));
                    }

                    newFieldDeclarations.Add(previous.WithTrailingTrivia(trailingTrivia));
                    previous = null;
                }
            }

            newFieldDeclarations.Add(previous.WithTrailingTrivia(declarationTrailingTrivia));
            return newFieldDeclarations;
        }
    }
}
