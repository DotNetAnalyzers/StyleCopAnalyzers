// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
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
    /// Implements a code fix for <see cref="SA1402FileMayOnlyContainASingleClass"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, move each class into its own file.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1402CodeFixProvider))]
    [Shared]
    internal class SA1402CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1402FileMayOnlyContainASingleClass.DiagnosticId);

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
                        MaintainabilityResources.SA1402CodeFix,
                        cancellationToken => GetTransformedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1402CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> GetTransformedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            BaseTypeDeclarationSyntax baseTypeDeclarationSyntax = node as BaseTypeDeclarationSyntax;
            DelegateDeclarationSyntax delegateDeclarationSyntax = node as DelegateDeclarationSyntax;
            if (baseTypeDeclarationSyntax == null && delegateDeclarationSyntax == null)
            {
                return document.Project.Solution;
            }

            DocumentId extractedDocumentId = DocumentId.CreateNewId(document.Project.Id);
            string extractedDocumentName = baseTypeDeclarationSyntax.Identifier.ValueText;
            if (baseTypeDeclarationSyntax != null)
            {
                TypeDeclarationSyntax typeDeclarationSyntax = baseTypeDeclarationSyntax as TypeDeclarationSyntax;
                if (typeDeclarationSyntax?.TypeParameterList?.Parameters.Count > 0)
                {
                    extractedDocumentName += "`" + typeDeclarationSyntax.TypeParameterList.Parameters.Count;
                }
            }
            else
            {
                if (delegateDeclarationSyntax.TypeParameterList?.Parameters.Count > 0)
                {
                    extractedDocumentName += "`" + delegateDeclarationSyntax.TypeParameterList.Parameters.Count;
                }
            }

            extractedDocumentName += ".cs";

            List<SyntaxNode> nodesToRemoveFromExtracted = new List<SyntaxNode>();
            SyntaxNode previous = node;
            for (SyntaxNode current = node.Parent; current != null; previous = current, current = current.Parent)
            {
                foreach (SyntaxNode child in current.ChildNodes())
                {
                    if (child == previous)
                    {
                        continue;
                    }

                    switch (child.Kind())
                    {
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.DelegateDeclaration:
                        nodesToRemoveFromExtracted.Add(child);
                        break;

                    default:
                        break;
                    }
                }
            }

            SyntaxNode extractedDocumentNode = root.RemoveNodes(nodesToRemoveFromExtracted, SyntaxRemoveOptions.KeepUnbalancedDirectives);

            Solution updatedSolution = document.Project.Solution.AddDocument(extractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);

            // Make sure to also add the file to linked projects
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                DocumentId linkedExtractedDocumentId = DocumentId.CreateNewId(linkedDocumentId.ProjectId);
                updatedSolution = updatedSolution.AddDocument(linkedExtractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);
            }

            // Remove the type from its original location
            updatedSolution = updatedSolution.WithDocumentSyntaxRoot(document.Id, root.RemoveNode(node, SyntaxRemoveOptions.KeepUnbalancedDirectives));

            return updatedSolution;
        }
    }
}
