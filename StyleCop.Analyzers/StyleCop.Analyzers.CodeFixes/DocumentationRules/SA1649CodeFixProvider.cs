// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

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

            var newSolution = ReplaceDocument(solution, document, document.Id, syntaxRoot, expectedFileName);

            // Make sure to also update other projects which reference the same file
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                newSolution = ReplaceDocument(newSolution, null, linkedDocumentId, syntaxRoot, expectedFileName);
            }

            return newSolution;
        }

        private static Solution ReplaceDocument(Solution solution, Document document, DocumentId documentId, SyntaxNode syntaxRoot, string expectedFileName)
        {
            document ??= solution.GetDocument(documentId);

            var newDocumentFilePath = document.FilePath != null ? Path.Combine(Path.GetDirectoryName(document.FilePath), expectedFileName) : null;
            var newDocumentId = DocumentId.CreateNewId(documentId.ProjectId);

            var newSolution = solution
                .RemoveDocument(documentId)
                .AddDocument(newDocumentId, expectedFileName, syntaxRoot, document.Folders, newDocumentFilePath);
            return newSolution;
        }
    }
}
