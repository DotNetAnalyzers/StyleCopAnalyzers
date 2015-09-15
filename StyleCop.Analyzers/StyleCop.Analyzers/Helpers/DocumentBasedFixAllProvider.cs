﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;

    /// <summary>
    /// Provides a base class to write a <see cref="FixAllProvider"/> that fixes documents independently.
    /// </summary>
    internal abstract class DocumentBasedFixAllProvider : FixAllProvider
    {
        protected abstract string CodeActionTitle { get; }

        public override Task<CodeAction> GetFixAsync(FixAllContext fixAllContext)
        {
            CodeAction fixAction;
            switch (fixAllContext.Scope)
            {
            case FixAllScope.Document:
                fixAction = CodeAction.Create(
                    this.CodeActionTitle,
                    cancellationToken => this.GetDocumentFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                    nameof(DocumentBasedFixAllProvider));
                break;

            case FixAllScope.Project:
                fixAction = CodeAction.Create(
                    this.CodeActionTitle,
                    cancellationToken => this.GetProjectFixesAsync(fixAllContext.WithCancellationToken(cancellationToken), fixAllContext.Project),
                    nameof(DocumentBasedFixAllProvider));
                break;

            case FixAllScope.Solution:
                fixAction = CodeAction.Create(
                    this.CodeActionTitle,
                    cancellationToken => this.GetSolutionFixesAsync(fixAllContext.WithCancellationToken(cancellationToken)),
                    nameof(DocumentBasedFixAllProvider));
                break;

            case FixAllScope.Custom:
            default:
                fixAction = null;
                break;
            }

            return Task.FromResult(fixAction);
        }

        /// <summary>
        /// Fixes all occurrences of a diagnostic in a specific document.
        /// </summary>
        /// <param name="fixAllContext">The context for the Fix All operation.</param>
        /// <param name="document">The document to fix.</param>
        /// <returns>
        /// <para>The new <see cref="SyntaxNode"/> representing the root of the fixed document.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/>, if no changes were made to the document.</para>
        /// </returns>
        protected abstract Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document);

        private async Task<Document> GetDocumentFixesAsync(FixAllContext fixAllContext)
        {
            var newRoot = await this.FixAllInDocumentAsync(fixAllContext, fixAllContext.Document).ConfigureAwait(false);
            if (newRoot == null)
            {
                return fixAllContext.Document;
            }

            return fixAllContext.Document.WithSyntaxRoot(newRoot);
        }

        private async Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext, ImmutableArray<Document> documents)
        {
            Solution solution = fixAllContext.Solution;
            List<Task<SyntaxNode>> newDocuments = new List<Task<SyntaxNode>>(documents.Length);
            foreach (var document in documents)
            {
                newDocuments.Add(this.FixAllInDocumentAsync(fixAllContext, document));
            }

            for (int i = 0; i < documents.Length; i++)
            {
                var newDocumentRoot = await newDocuments[i].ConfigureAwait(false);
                if (newDocumentRoot == null)
                {
                    continue;
                }

                solution = solution.WithDocumentSyntaxRoot(documents[i].Id, newDocumentRoot);
            }

            return solution;
        }

        private Task<Solution> GetProjectFixesAsync(FixAllContext fixAllContext, Project project)
        {
            return this.GetSolutionFixesAsync(fixAllContext, project.Documents.ToImmutableArray());
        }

        private Task<Solution> GetSolutionFixesAsync(FixAllContext fixAllContext)
        {
            ImmutableArray<Document> documents = fixAllContext.Solution.Projects.SelectMany(i => i.Documents).ToImmutableArray();
            return this.GetSolutionFixesAsync(fixAllContext, documents);
        }
    }
}
