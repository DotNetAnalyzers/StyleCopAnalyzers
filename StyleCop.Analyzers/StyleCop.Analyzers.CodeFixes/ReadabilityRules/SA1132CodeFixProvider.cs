// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

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
                editor.RemoveNode(baseFieldDeclaration);
                return document.WithSyntaxRoot(editor.GetChangedRoot().WithoutFormatting());
            }

            return document;
        }

        private static List<BaseFieldDeclarationSyntax> SplitDeclaration(Document document, BaseFieldDeclarationSyntax baseFieldDeclaration)
        {
            var fieldDeclaration = baseFieldDeclaration as FieldDeclarationSyntax;
            if (fieldDeclaration != null)
            {
                return DeclarationSplitter(
                    document,
                    fieldDeclaration.Declaration,
                    fieldDeclaration.WithDeclaration,
                    fieldDeclaration.SemicolonToken.TrailingTrivia);
            }

            var eventFieldDeclaration = baseFieldDeclaration as EventFieldDeclarationSyntax;
            if (eventFieldDeclaration != null)
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
