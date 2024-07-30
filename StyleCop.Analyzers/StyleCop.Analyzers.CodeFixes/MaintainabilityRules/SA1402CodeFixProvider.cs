// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System;
    using System.Collections.Generic;
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
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Implements a code fix for <see cref="SA1402FileMayOnlyContainASingleType"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, move each type into its own file.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1402CodeFixProvider))]
    [Shared]
    internal class SA1402CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1402FileMayOnlyContainASingleType.DiagnosticId);

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
            if (!(node is MemberDeclarationSyntax memberDeclarationSyntax))
            {
                return document.Project.Solution;
            }

            DocumentId extractedDocumentId = DocumentId.CreateNewId(document.Project.Id);
            string suffix;
            FileNameHelpers.GetFileNameAndSuffix(document.Name, out suffix);
            var settings = document.Project.AnalyzerOptions.GetStyleCopSettings(root.SyntaxTree, cancellationToken);
            string extractedDocumentName = FileNameHelpers.GetConventionalFileName(memberDeclarationSyntax, settings.DocumentationRules.FileNamingConvention) + suffix;

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
                    case SyntaxKindEx.RecordDeclaration:
                    case SyntaxKindEx.RecordStructDeclaration:
                        nodesToRemoveFromExtracted.Add(child);
                        break;

                    case SyntaxKindEx.FileScopedNamespaceDeclaration:
                        // Only one file-scoped namespace is allowed per syntax tree
                        throw new InvalidOperationException("This location is not reachable");

                    default:
                        break;
                    }
                }
            }

            // Add the new file
            SyntaxNode extractedDocumentNode = root.RemoveNodes(nodesToRemoveFromExtracted, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            Solution updatedSolution = document.Project.Solution.AddDocument(extractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);

            updatedSolution = await RemoveUnnecessaryUsingsAsync(updatedSolution, extractedDocumentId, cancellationToken).ConfigureAwait(false);

            // Make sure to also add the file to linked projects
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                DocumentId linkedExtractedDocumentId = DocumentId.CreateNewId(linkedDocumentId.ProjectId);
                updatedSolution = updatedSolution.AddDocument(linkedExtractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);
            }

            // Remove the type from its original location
            var newRootOriginal = root.RemoveNode(node, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            updatedSolution = updatedSolution.WithDocumentSyntaxRoot(document.Id, newRootOriginal);

            return updatedSolution;
        }

        private static async Task<Solution> RemoveUnnecessaryUsingsAsync(Solution solution, DocumentId documentId, CancellationToken cancellationToken)
        {
            var document = solution.GetDocument(documentId);
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var unnecessaryUsings = await GetUnnecessaryUsingsAsync(document, root, cancellationToken).ConfigureAwait(false);

            var hasPreprocessorDirectives = root.DescendantTrivia().Any(t => t.IsKind(SyntaxKind.IfDirectiveTrivia) || t.IsKind(SyntaxKind.EndIfDirectiveTrivia));

            if (hasPreprocessorDirectives)
            {
                root = RemoveUnnecessaryPreprocessorDirectives(root, unnecessaryUsings);

                root = StripMultipleBlankLines(root);

                // Recalculate unnecessary usings after modifying the root
                unnecessaryUsings = await GetUnnecessaryUsingsAsync(document, root, cancellationToken).ConfigureAwait(false);
            }

            var newRoot = root.RemoveNodes(unnecessaryUsings, SyntaxRemoveOptions.KeepNoTrivia);

            return solution.WithDocumentSyntaxRoot(documentId, newRoot);
        }

        private static async Task<List<UsingDirectiveSyntax>> GetUnnecessaryUsingsAsync(Document document, SyntaxNode root, CancellationToken cancellationToken)
        {
            var semanticModel = await document.WithSyntaxRoot(root).GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var diagnostics = semanticModel.GetDiagnostics();
            var cs8019Diagnostics = diagnostics.Where(d => d.Id == "CS8019").ToList();

            return cs8019Diagnostics.Select(diagnostic =>
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;
                var usingDirective = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().First();
                return usingDirective;
            }).ToList();
        }

        private static SyntaxNode RemoveUnnecessaryPreprocessorDirectives(SyntaxNode root, List<UsingDirectiveSyntax> unnecessaryUsings)
        {
            var nodesToRemove = new List<SyntaxNode>();

            foreach (var usingDirective in unnecessaryUsings)
            {
                var ifDirective = usingDirective.GetLeadingTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.IfDirectiveTrivia));
                var elifDirective = root.DescendantTrivia()
                                        .FirstOrDefault(t => t.IsKind(SyntaxKind.ElifDirectiveTrivia) &&
                                                             t.SpanStart > usingDirective.Span.End);
                var elseDirective = root.DescendantTrivia()
                                        .FirstOrDefault(t => t.IsKind(SyntaxKind.ElseDirectiveTrivia) &&
                                                             t.SpanStart > usingDirective.Span.End);
                var endIfDirective = root.DescendantTrivia()
                                         .FirstOrDefault(t => t.IsKind(SyntaxKind.EndIfDirectiveTrivia) &&
                                                              t.SpanStart > usingDirective.Span.End);

                if (ifDirective != default)
                {
                    nodesToRemove.Add(ifDirective.GetStructure() as DirectiveTriviaSyntax);
                }

                if (elifDirective != default)
                {
                    nodesToRemove.Add(elifDirective.GetStructure() as DirectiveTriviaSyntax);
                }

                if (elseDirective != default)
                {
                    nodesToRemove.Add(elseDirective.GetStructure() as DirectiveTriviaSyntax);
                }

                if (endIfDirective != default)
                {
                    nodesToRemove.Add(endIfDirective.GetStructure() as DirectiveTriviaSyntax);
                }
            }

            root = root.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia);
            return root;
        }

        private static SyntaxNode StripMultipleBlankLines(SyntaxNode syntaxRoot)
        {
            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>();

            var usingDirectives = syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>();

            foreach (var usingDirective in usingDirectives)
            {
                var nextToken = usingDirective.GetLastToken().GetNextToken();

                // Start at -1 to compensate for the always present end-of-line.
                var trailingCount = -1;

                // Count the blank lines at the end of the using statement.
                foreach (var trivia in usingDirective.GetTrailingTrivia().Reverse())
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    trailingCount++;
                }

                // Count the blank lines at the start of the next token
                var leadingCount = 0;

                foreach (var trivia in nextToken.LeadingTrivia)
                {
                    if (!trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        break;
                    }

                    leadingCount++;
                }

                if ((trailingCount + leadingCount) > 1)
                {
                    var totalStripCount = trailingCount + leadingCount - 1;

                    if (trailingCount > 0)
                    {
                        var trailingStripCount = Math.Min(totalStripCount, trailingCount);

                        var trailingTrivia = usingDirective.GetTrailingTrivia();
                        replaceMap[usingDirective.GetLastToken()] = usingDirective.GetLastToken().WithTrailingTrivia(trailingTrivia.Take(trailingTrivia.Count - trailingStripCount));
                        totalStripCount -= trailingStripCount;
                    }

                    if (totalStripCount > 0)
                    {
                        replaceMap[nextToken] = nextToken.WithLeadingTrivia(nextToken.LeadingTrivia.Skip(totalStripCount));
                    }
                }
            }

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (original, rewritten) => replaceMap[original]);
            return newSyntaxRoot;
        }
    }
}
