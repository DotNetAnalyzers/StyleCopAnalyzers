// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.LayoutRules
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
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1503BracesMustNotBeOmitted"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, the violating statement will be converted to a block statement.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1503CodeFixProvider))]
    [Shared]
    internal class SA1503CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1503BracesMustNotBeOmitted.DiagnosticId, SA1519BracesMustNotBeOmittedFromMultiLineChildStatement.DiagnosticId, SA1520UseBracesConsistently.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) is StatementSyntax node) || node.IsMissing)
                {
                    continue;
                }

                // If the parent of the statement contains a conditional directive, stuff will be really hard to fix correctly, so don't offer a code fix.
                if (ContainsConditionalDirectiveTrivia(node.Parent))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        LayoutResources.SA1503CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, syntaxRoot, node, cancellationToken),
                        nameof(SA1503CodeFixProvider)),
                    diagnostic);
            }
        }

        private static Task<Document> GetTransformedDocumentAsync(Document document, SyntaxNode root, StatementSyntax node, CancellationToken cancellationToken)
        {
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, root.SyntaxTree, cancellationToken);
            var newSyntaxRoot = AddBraces(root, new[] { node }, settings.Indentation);
            return Task.FromResult(document.WithSyntaxRoot(newSyntaxRoot));
        }

        private static SyntaxNode AddBraces(SyntaxNode root, IReadOnlyCollection<SyntaxNode> nodes, IndentationSettings indentationSettings)
        {
            var annotation = new SyntaxAnnotation();
            var shiftAnnotation = new SyntaxAnnotation();
            var annotatedRoot = root.ReplaceNodes(
                nodes,
                (originalNode, rewrittenNode) => NeedsShift(originalNode, indentationSettings)
                    ? rewrittenNode.WithAdditionalAnnotations(annotation, shiftAnnotation)
                    : rewrittenNode.WithAdditionalAnnotations(annotation));

            // Every line of a statement that is not already indented below its parent moves one indentation step to the right.
            string indentationStep = IndentationHelper.GenerateIndentationString(indentationSettings, 1);
            var tokensToIndent = new List<SyntaxToken>();
            foreach (SyntaxNode annotatedNode in annotatedRoot.GetAnnotatedNodes(annotation))
            {
                if (!HasAnnotatedAncestor(annotatedNode, annotation))
                {
                    tokensToIndent.AddRange(annotatedNode.DescendantTokens());
                }
            }

            var indentedRoot = annotatedRoot.ReplaceTokens(
                tokensToIndent,
                (originalToken, rewrittenToken) =>
                {
                    int depth = 0;
                    for (SyntaxNode current = originalToken.Parent; current != null; current = current.Parent)
                    {
                        if (current.HasAnnotation(shiftAnnotation))
                        {
                            depth++;
                        }
                    }

                    if (depth == 0)
                    {
                        return rewrittenToken;
                    }

                    var step = new System.Text.StringBuilder();
                    for (int i = 0; i < depth; i++)
                    {
                        step.Append(indentationStep);
                    }

                    return rewrittenToken.WithLeadingTrivia(IndentationHelper.IndentLeadingTrivia(originalToken, rewrittenToken.LeadingTrivia, step.ToString(), default(SyntaxTriviaList)));
                });

            return indentedRoot.ReplaceNodes(
                indentedRoot.GetAnnotatedNodes(annotation),
                (originalNode, rewrittenNode) => SyntaxFactory.Block((StatementSyntax)rewrittenNode.WithoutAnnotations(annotation)));
        }

        private static bool NeedsShift(SyntaxNode node, IndentationSettings indentationSettings)
        {
            SyntaxToken firstToken = node.GetFirstToken();
            SyntaxToken previousToken = firstToken.GetPreviousToken();
            if (previousToken.IsKind(SyntaxKind.None) || previousToken.GetLine() == firstToken.GetLine())
            {
                // The statement does not start a line, so it is moved to a new line by the formatter.
                return false;
            }

            SyntaxToken parentLineToken = IndentationHelper.GetFirstTokenOnTextLine(previousToken);
            return IndentationHelper.GetIndentationSteps(indentationSettings, firstToken) <= IndentationHelper.GetIndentationSteps(indentationSettings, parentLineToken);
        }

        private static bool HasAnnotatedAncestor(SyntaxNode node, SyntaxAnnotation annotation)
        {
            for (SyntaxNode current = node.Parent; current != null; current = current.Parent)
            {
                if (current.HasAnnotation(annotation))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsConditionalDirectiveTrivia(SyntaxNode node)
        {
            for (var currentDirective = node.GetFirstDirective(); currentDirective != null && node.Contains(currentDirective); currentDirective = currentDirective.GetNextDirective())
            {
                switch (currentDirective.Kind())
                {
                case SyntaxKind.IfDirectiveTrivia:
                case SyntaxKind.ElseDirectiveTrivia:
                case SyntaxKind.ElifDirectiveTrivia:
                case SyntaxKind.EndIfDirectiveTrivia:
                    return true;
                }
            }

            return false;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                LayoutResources.SA1503CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                List<SyntaxNode> nodesNeedingBlocks = new List<SyntaxNode>(diagnostics.Length);

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan, false, true) is StatementSyntax node) || node.IsMissing)
                    {
                        continue;
                    }

                    // If the parent of the statement contains a conditional directive, stuff will be really hard to fix correctly, so don't offer a code fix.
                    if (ContainsConditionalDirectiveTrivia(node.Parent))
                    {
                        continue;
                    }

                    nodesNeedingBlocks.Add(node);
                }

                var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, fixAllContext.CancellationToken);
                return AddBraces(syntaxRoot, nodesNeedingBlocks, settings.Indentation);
            }
        }
    }
}
