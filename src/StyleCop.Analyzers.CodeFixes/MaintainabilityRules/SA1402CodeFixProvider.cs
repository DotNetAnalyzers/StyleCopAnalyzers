// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
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
        private const string TrimAnnotationKind = "SA1402Trim";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1402FileMayOnlyContainASingleType.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider? GetFixAllProvider()
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
            var settings = document.Project.AnalyzerOptions.GetStyleCopSettingsInCodeFix(root.SyntaxTree, cancellationToken);
            string extractedDocumentName = FileNameHelpers.GetConventionalFileName(memberDeclarationSyntax, settings.DocumentationRules.FileNamingConvention) + suffix;

            var keptAnnotation = new SyntaxAnnotation("SA1402Kept");
            root = root.ReplaceNode(node, node.WithAdditionalAnnotations(keptAnnotation));
            node = GetAnnotatedNode(root, keptAnnotation)!;

            List<SyntaxNode> nodesToRemoveFromExtracted = new List<SyntaxNode>();
            List<SyntaxNode> nodesToTrim = new List<SyntaxNode>();
            List<int> blankLinesToKeep = new List<int>();
            SyntaxNode? previous = node;
            for (SyntaxNode current = node.Parent; current != null; previous = current, current = current.Parent)
            {
                List<SyntaxNode> children = new List<SyntaxNode>();
                foreach (SyntaxNode child in current.ChildNodes())
                {
                    children.Add(child);
                }

                int keptIndex = IndexOfNode(children, previous);
                for (int i = 0; i < children.Count; i++)
                {
                    if (i == keptIndex)
                    {
                        continue;
                    }

                    if (IsRemovedSibling(children[i]))
                    {
                        nodesToRemoveFromExtracted.Add(children[i]);
                    }
                }

                // Only the node that directly follows a removed sibling owns the leftover separator.
                // Earlier removals at this level are handled on that node, not on every descendant.
                if (keptIndex > 0 && IsRemovedSibling(children[keptIndex - 1]))
                {
                    int runStart = keptIndex - 1;
                    while (runStart > 0 && IsRemovedSibling(children[runStart - 1]))
                    {
                        runStart--;
                    }

                    nodesToTrim.Add(previous);
                    blankLinesToKeep.Add(CountLeadingBlankLines(children[runStart].GetLeadingTrivia()));
                }
            }

            // Annotate a copy so removal targets stay findable after the rewrite, and so the
            // original tree used for the source document is left unchanged.
            SyntaxNode extractedRoot = root;
            if (nodesToTrim.Count > 0)
            {
                List<SyntaxNode> nodesToAnnotate = new List<SyntaxNode>(nodesToRemoveFromExtracted.Count + nodesToTrim.Count);
                foreach (var removedNode in nodesToRemoveFromExtracted)
                {
                    nodesToAnnotate.Add(removedNode);
                }

                foreach (var trimNode in nodesToTrim)
                {
                    nodesToAnnotate.Add(trimNode);
                }

                var removeAnnotation = new SyntaxAnnotation("SA1402Remove");
                extractedRoot = root.ReplaceNodes(
                    nodesToAnnotate,
                    (original, rewritten) =>
                    {
                        if (ContainsNode(nodesToRemoveFromExtracted, original))
                        {
                            rewritten = rewritten.WithAdditionalAnnotations(removeAnnotation);
                        }

                        int trimIndex = IndexOfNode(nodesToTrim, original);
                        if (trimIndex >= 0)
                        {
                            var trimAnnotation = new SyntaxAnnotation(TrimAnnotationKind, blankLinesToKeep[trimIndex].ToString());
                            rewritten = rewritten.WithAdditionalAnnotations(trimAnnotation);
                        }

                        return rewritten;
                    });

                nodesToRemoveFromExtracted.Clear();
                foreach (var annotated in extractedRoot.GetAnnotatedNodes(removeAnnotation))
                {
                    nodesToRemoveFromExtracted.Add(annotated);
                }
            }

            // Add the new file
            SyntaxNode extractedDocumentNode = extractedRoot.RemoveNodes(nodesToRemoveFromExtracted, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            extractedDocumentNode = RemoveBlankLinesLeftByRemovedSiblings(extractedDocumentNode);

            Solution updatedSolution = document.Project.Solution.AddDocument(extractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);
            List<DocumentId> extractedDocumentIds = new List<DocumentId> { extractedDocumentId };

            // Make sure to also add the file to linked projects
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                DocumentId linkedExtractedDocumentId = DocumentId.CreateNewId(linkedDocumentId.ProjectId);
                updatedSolution = updatedSolution.AddDocument(linkedExtractedDocumentId, extractedDocumentName, extractedDocumentNode, document.Folders);
                extractedDocumentIds.Add(linkedExtractedDocumentId);
            }

            foreach (var linkedExtractedDocumentId in extractedDocumentIds)
            {
                Document extractedDocument = updatedSolution.GetDocument(linkedExtractedDocumentId);
                SemanticModel semanticModel = await extractedDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
                SyntaxNode linkedExtractedRoot = await extractedDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                List<SyntaxNode> unnecessaryUsings = new List<SyntaxNode>();
                foreach (var diagnosticInTree in semanticModel.GetDiagnostics(cancellationToken: cancellationToken))
                {
                    if (!string.Equals(diagnosticInTree.Id, "CS8019", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    SyntaxNode unnecessaryUsing = linkedExtractedRoot.FindNode(diagnosticInTree.Location.SourceSpan, getInnermostNodeForTie: true);
                    while (unnecessaryUsing != null && !(unnecessaryUsing is UsingDirectiveSyntax))
                    {
                        unnecessaryUsing = unnecessaryUsing.Parent;
                    }

                    if (unnecessaryUsing != null && !unnecessaryUsing.ContainsDirectives)
                    {
                        unnecessaryUsings.Add(unnecessaryUsing);
                    }
                }

                if (unnecessaryUsings.Count > 0)
                {
                    List<SyntaxNode> namespacesToNormalize = new List<SyntaxNode>();
                    foreach (var unnecessaryUsing in unnecessaryUsings)
                    {
                        if (!(unnecessaryUsing.Parent is NamespaceDeclarationSyntax namespaceDeclaration))
                        {
                            continue;
                        }

                        bool allUsingsRemoved = true;
                        foreach (var usingDirective in namespaceDeclaration.Usings)
                        {
                            if (!ContainsNode(unnecessaryUsings, usingDirective))
                            {
                                allUsingsRemoved = false;
                                break;
                            }
                        }

                        if (allUsingsRemoved && !ContainsNode(namespacesToNormalize, namespaceDeclaration))
                        {
                            namespacesToNormalize.Add(namespaceDeclaration);
                        }
                    }

                    List<SyntaxNode> usingNodesToTrim = new List<SyntaxNode>();
                    List<int> usingBlankLinesToKeep = new List<int>();
                    foreach (var unnecessaryUsing in unnecessaryUsings)
                    {
                        var siblings = new List<SyntaxNode>();
                        foreach (var sibling in unnecessaryUsing.Parent.ChildNodes())
                        {
                            siblings.Add(sibling);
                        }

                        int index = IndexOfNode(siblings, unnecessaryUsing);
                        if (index < 0 || index + 1 >= siblings.Count || ContainsNode(unnecessaryUsings, siblings[index + 1]))
                        {
                            continue;
                        }

                        SyntaxNode nextSibling = siblings[index + 1];
                        if (!ContainsNode(usingNodesToTrim, nextSibling))
                        {
                            usingNodesToTrim.Add(nextSibling);
                            int blankLines = CountLeadingBlankLines(nextSibling.GetLeadingTrivia());
                            bool hasRemainingSiblingBeforeTarget = false;
                            for (int i = 0; i < index; i++)
                            {
                                if (!ContainsNode(unnecessaryUsings, siblings[i]))
                                {
                                    hasRemainingSiblingBeforeTarget = true;
                                    break;
                                }
                            }

                            if (!hasRemainingSiblingBeforeTarget && !(unnecessaryUsing.Parent is CompilationUnitSyntax))
                            {
                                blankLines = CountLeadingBlankLines(unnecessaryUsing.GetLeadingTrivia());
                            }
                            else if (!hasRemainingSiblingBeforeTarget)
                            {
                                blankLines = 0;
                            }

                            usingBlankLinesToKeep.Add(blankLines);
                        }
                    }

                    List<SyntaxNode> nodesToAnnotate = new List<SyntaxNode>(unnecessaryUsings.Count + usingNodesToTrim.Count + namespacesToNormalize.Count);
                    nodesToAnnotate.AddRange(unnecessaryUsings);
                    nodesToAnnotate.AddRange(namespacesToNormalize);
                    foreach (var nodeToTrim in usingNodesToTrim)
                    {
                        if (!ContainsNode(nodesToAnnotate, nodeToTrim))
                        {
                            nodesToAnnotate.Add(nodeToTrim);
                        }
                    }

                    var removeAnnotation = new SyntaxAnnotation("SA1402RemoveUsing");
                    linkedExtractedRoot = linkedExtractedRoot.ReplaceNodes(
                        nodesToAnnotate,
                        (original, rewritten) =>
                        {
                            if (ContainsNode(unnecessaryUsings, original))
                            {
                                rewritten = rewritten.WithAdditionalAnnotations(removeAnnotation);
                            }

                            if (ContainsNode(namespacesToNormalize, original))
                            {
                                rewritten = rewritten.WithAdditionalAnnotations(new SyntaxAnnotation("SA1402NormalizeNamespace"));
                            }

                            int trimIndex = IndexOfNode(usingNodesToTrim, original);
                            if (trimIndex >= 0)
                            {
                                var trimAnnotation = new SyntaxAnnotation(TrimAnnotationKind, usingBlankLinesToKeep[trimIndex].ToString());
                                rewritten = rewritten.WithAdditionalAnnotations(trimAnnotation);
                            }

                            return rewritten;
                        });

                    List<SyntaxNode> nodesToRemove = new List<SyntaxNode>();
                    foreach (var annotated in linkedExtractedRoot.GetAnnotatedNodes(removeAnnotation))
                    {
                        nodesToRemove.Add(annotated);
                    }

                    linkedExtractedRoot = linkedExtractedRoot.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepExteriorTrivia);
                    linkedExtractedRoot = RemoveBlankLinesLeftByRemovedSiblings(linkedExtractedRoot);
                    List<SyntaxNode> annotatedNamespaces = new List<SyntaxNode>();
                    foreach (var annotatedNamespace in linkedExtractedRoot.GetAnnotatedNodes("SA1402NormalizeNamespace"))
                    {
                        annotatedNamespaces.Add(annotatedNamespace);
                    }

                    foreach (var annotatedNamespace in annotatedNamespaces)
                    {
                        if (annotatedNamespace is NamespaceDeclarationSyntax namespaceDeclaration && namespaceDeclaration.Members.Count > 0)
                        {
                            SyntaxNode firstMember = namespaceDeclaration.Members[0];
                            linkedExtractedRoot = linkedExtractedRoot.ReplaceNode(firstMember, TrimLeadingBlankLines(firstMember, 0));
                        }
                    }

                    int leadingHeaderCommentCount = GetLeadingHeaderCommentCount(root);
                    if (leadingHeaderCommentCount > 0)
                    {
                        linkedExtractedRoot = NormalizeLeadingCommentSeparator(linkedExtractedRoot, leadingHeaderCommentCount);
                    }

                    updatedSolution = updatedSolution.WithDocumentSyntaxRoot(linkedExtractedDocumentId, linkedExtractedRoot);
                }
            }

            // Remove the type from its original location
            updatedSolution = updatedSolution.WithDocumentSyntaxRoot(document.Id, root.RemoveNode(node, SyntaxRemoveOptions.KeepUnbalancedDirectives));

            return updatedSolution;
        }

        private static SyntaxNode? GetAnnotatedNode(SyntaxNode root, SyntaxAnnotation annotation)
        {
            foreach (var annotated in root.GetAnnotatedNodes(annotation))
            {
                return annotated;
            }

            return null;
        }

        /// <summary>
        /// Removes blank lines that node removal leaves in front of each kept declaration whose
        /// earlier sibling was removed.
        /// </summary>
        private static SyntaxNode RemoveBlankLinesLeftByRemovedSiblings(SyntaxNode root)
        {
            while (true)
            {
                SyntaxNode? kept = null;
                SyntaxAnnotation? annotation = null;
                foreach (var candidate in root.GetAnnotatedNodes(TrimAnnotationKind))
                {
                    kept = candidate;
                    foreach (var candidateAnnotation in candidate.GetAnnotations(TrimAnnotationKind))
                    {
                        annotation = candidateAnnotation;
                        break;
                    }

                    break;
                }

                if (kept == null)
                {
                    return root;
                }

                int blankLinesToKeep = 0;
                int parsed;
                if (annotation != null && int.TryParse(annotation.Data, out parsed))
                {
                    blankLinesToKeep = parsed;
                }

                var updated = TrimLeadingBlankLines(kept, blankLinesToKeep);
                if (annotation != null)
                {
                    updated = updated.WithoutAnnotations(annotation);
                }

                root = root.ReplaceNode(kept, updated);
            }
        }

        private static SyntaxNode NormalizeLeadingCommentSeparator(SyntaxNode root, int headerCommentCount)
        {
            SyntaxNode? firstChild = null;
            foreach (var child in root.ChildNodes())
            {
                firstChild = child;
                break;
            }

            if (firstChild == null)
            {
                return root;
            }

            var leading = firstChild.GetLeadingTrivia();
            List<int> commentIndices = new List<int>();
            for (int i = 0; i < leading.Count; i++)
            {
                if (leading[i].IsKind(SyntaxKind.SingleLineCommentTrivia)
                    || leading[i].IsKind(SyntaxKind.MultiLineCommentTrivia)
                    || leading[i].IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                    || leading[i].IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    commentIndices.Add(i);
                }
            }

            if (commentIndices.Count < headerCommentCount)
            {
                return root;
            }

            string? lineEnding = null;
            int lastHeaderCommentIndex = commentIndices[headerCommentCount - 1];
            for (int i = 0; i < leading.Count; i++)
            {
                if (leading[i].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    lineEnding ??= leading[i].ToString();
                }
            }

            if (lineEnding == null)
            {
                return root;
            }

            int remainderStart = lastHeaderCommentIndex + 1;
            while (remainderStart < leading.Count)
            {
                if (leading[remainderStart].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    remainderStart++;
                }
                else if (leading[remainderStart].IsKind(SyntaxKind.WhitespaceTrivia)
                    && remainderStart + 1 < leading.Count
                    && leading[remainderStart + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    remainderStart += 2;
                }
                else
                {
                    break;
                }
            }

            List<SyntaxTrivia> normalizedLeading = new List<SyntaxTrivia>(leading.Count + 1);
            for (int i = 0; i <= lastHeaderCommentIndex; i++)
            {
                normalizedLeading.Add(leading[i]);
            }

            normalizedLeading.Add(SyntaxFactory.EndOfLine(lineEnding));
            normalizedLeading.Add(SyntaxFactory.EndOfLine(lineEnding));
            for (int i = remainderStart; i < leading.Count; i++)
            {
                normalizedLeading.Add(leading[i]);
            }

            return root.ReplaceNode(firstChild, firstChild.WithLeadingTrivia(SyntaxFactory.TriviaList(normalizedLeading)));
        }

        private static int GetLeadingHeaderCommentCount(SyntaxNode sourceRoot)
        {
            SyntaxNode? firstChild = null;
            foreach (var child in sourceRoot.ChildNodes())
            {
                firstChild = child;
                break;
            }

            if (!(firstChild is UsingDirectiveSyntax usingDirective))
            {
                return 0;
            }

            int count = 0;
            int endOfLineCount = 0;
            foreach (var trivia in usingDirective.GetLeadingTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia)
                    || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                {
                    count++;
                    endOfLineCount = 0;
                }
                else if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    endOfLineCount++;
                    if (endOfLineCount > 1)
                    {
                        break;
                    }
                }
                else if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia) && trivia.IsDirective)
                {
                    break;
                }
            }

            return count;
        }

        /// <summary>
        /// Drops leading blank lines down to <paramref name="blankLinesToKeep"/>.
        /// A missing previous token means the node now starts the file, so a leading end-of-line is a blank line.
        /// </summary>
        private static SyntaxNode TrimLeadingBlankLines(SyntaxNode kept, int blankLinesToKeep)
        {
            var leading = kept.GetLeadingTrivia();
            int blankLines = CountLeadingBlankLines(leading);
            if (blankLines <= blankLinesToKeep)
            {
                return kept;
            }

            var previous = kept.GetFirstToken().GetPreviousToken();
            if (previous.IsKind(SyntaxKind.None))
            {
                // A leading end-of-line at the start of the file is a blank line, except when it
                // precedes a preserved directive (see roslyn issue 3999).
                if (LeadingBlankLinesAreFollowedByDirective(leading))
                {
                    return kept;
                }
            }
            else if (!EndsWithLineBreak(previous))
            {
                return kept;
            }

            var trimmed = RemoveLeadingBlankLines(leading, blankLines - blankLinesToKeep);
            if (trimmed.Count == leading.Count)
            {
                return kept;
            }

            return kept.WithLeadingTrivia(trimmed);
        }

        private static bool IsRemovedSibling(SyntaxNode child)
        {
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
            case SyntaxKindEx.UnionDeclaration:
                return true;

            case SyntaxKindEx.FileScopedNamespaceDeclaration:
                // Only one file-scoped namespace is allowed per syntax tree
                throw new InvalidOperationException("This location is not reachable");

            default:
                return false;
            }
        }

        private static int IndexOfNode(List<SyntaxNode> nodes, SyntaxNode node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (ReferenceEquals(nodes[i], node))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool ContainsNode(List<SyntaxNode> nodes, SyntaxNode node)
        {
            return IndexOfNode(nodes, node) >= 0;
        }

        private static bool EndsWithLineBreak(SyntaxToken token)
        {
            var trailing = token.TrailingTrivia;
            for (int i = trailing.Count - 1; i >= 0; i--)
            {
                var trivia = trailing[i];
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia) || trivia.IsDirective)
                {
                    return true;
                }

                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    return false;
                }
            }

            return false;
        }

        private static bool LeadingBlankLinesAreFollowedByDirective(SyntaxTriviaList leading)
        {
            int index;
            CountRemovedBlankLines(leading, int.MaxValue, out index);
            return index < leading.Count && leading[index].IsDirective;
        }

        private static int CountLeadingBlankLines(SyntaxTriviaList leading)
        {
            return CountRemovedBlankLines(leading, int.MaxValue, out _);
        }

        private static int CountRemovedBlankLines(SyntaxTriviaList leading, int blankLinesToRemove, out int index)
        {
            index = 0;
            int removed = 0;
            while (index < leading.Count && removed < blankLinesToRemove)
            {
                if (leading[index].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index++;
                    removed++;
                    continue;
                }

                if (leading[index].IsKind(SyntaxKind.WhitespaceTrivia)
                    && index + 1 < leading.Count
                    && leading[index + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    index += 2;
                    removed++;
                    continue;
                }

                break;
            }

            return removed;
        }

        private static SyntaxTriviaList RemoveLeadingBlankLines(SyntaxTriviaList leading, int blankLinesToRemove)
        {
            int index;
            CountRemovedBlankLines(leading, blankLinesToRemove, out index);

            if (index == 0)
            {
                return leading;
            }

            var kept = new List<SyntaxTrivia>(leading.Count - index);
            for (int i = index; i < leading.Count; i++)
            {
                kept.Add(leading[i]);
            }

            return SyntaxFactory.TriviaList(kept);
        }
    }
}
