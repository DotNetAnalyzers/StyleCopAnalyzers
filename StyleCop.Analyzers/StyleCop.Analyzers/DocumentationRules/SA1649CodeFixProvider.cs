// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Implements a code fix for <see cref="SA1649FileNameMustMatchTypeName"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1649CodeFixProvider))]
    [Shared]
    internal class SA1649CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1649FileNameMustMatchTypeName.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            // The batch fixer can't handle code fixes that create new files
            return null;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1649CodeFix,
                        cancellationToken => GetTransformedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1649CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> GetTransformedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var expectedFileName = diagnostic.Properties[SA1649FileNameMustMatchTypeName.ExpectedFileNameKey];
            var newPath = document.FilePath != null ? Path.Combine(Path.GetDirectoryName(document.FilePath), expectedFileName) : null;

            var newDocumentId = DocumentId.CreateNewId(document.Id.ProjectId);

            var newSolution = solution
                .RemoveDocument(document.Id)
                .AddDocument(newDocumentId, expectedFileName, syntaxRoot, document.Folders, newPath);

            // Make sure to also add the file to linked projects
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                DocumentId linkedExtractedDocumentId = DocumentId.CreateNewId(linkedDocumentId.ProjectId);
                newSolution = newSolution.AddDocument(linkedExtractedDocumentId, expectedFileName, syntaxRoot, document.Folders);
            }

            return newSolution;
        }
    }
}
