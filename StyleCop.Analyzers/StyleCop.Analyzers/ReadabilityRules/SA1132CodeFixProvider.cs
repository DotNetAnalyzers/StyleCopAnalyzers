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
                        equivalenceKey: nameof(SA1132CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var baseFieldDeclaration = (BaseFieldDeclarationSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            List<BaseFieldDeclarationSyntax> newFieldDeclarations = SplitDeclaration(baseFieldDeclaration);

            if (newFieldDeclarations != null)
            {
                var editor = new SyntaxEditor(syntaxRoot, document.Project.Solution.Workspace);
                editor.InsertAfter(baseFieldDeclaration, newFieldDeclarations);
                editor.RemoveNode(baseFieldDeclaration);
                return document.WithSyntaxRoot(editor.GetChangedRoot());
            }

            return document;
        }

        private static List<BaseFieldDeclarationSyntax> SplitDeclaration(BaseFieldDeclarationSyntax baseFieldDeclaration)
        {
            var fieldDeclaration = baseFieldDeclaration as FieldDeclarationSyntax;
            if (fieldDeclaration != null)
            {
                VariableDeclarationSyntax declaration = fieldDeclaration.Declaration;
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;
                var newFieldDeclarations = new List<BaseFieldDeclarationSyntax>(variables.Count);

                foreach (VariableDeclaratorSyntax variable in variables)
                {
                    var variableDeclarator = SyntaxFactory.SeparatedList(new[] { variable });

                    newFieldDeclarations.Add(
                        fieldDeclaration.WithDeclaration(
                            declaration.WithVariables(variableDeclarator)));
                }

                return newFieldDeclarations;
            }

            var eventFieldDeclaration = baseFieldDeclaration as EventFieldDeclarationSyntax;
            if (eventFieldDeclaration != null)
            {
                VariableDeclarationSyntax declaration = eventFieldDeclaration.Declaration;
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;
                var newEventFieldDeclarations = new List<BaseFieldDeclarationSyntax>(variables.Count);

                foreach (VariableDeclaratorSyntax variable in variables)
                {
                    var variableDeclarator = SyntaxFactory.SeparatedList(new[] { variable });

                    newEventFieldDeclarations.Add(
                        eventFieldDeclaration.WithDeclaration(
                            declaration.WithVariables(variableDeclarator)));
                }

                return newEventFieldDeclarations;
            }

            return null;
        }
    }
}
