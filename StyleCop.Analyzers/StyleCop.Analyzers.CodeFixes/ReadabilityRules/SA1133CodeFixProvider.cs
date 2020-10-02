// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1133DoNotCombineAttributes"/>.
    /// </summary>
    /// <remarks>
    /// <para>The SA1133 code fix adds the new lines to make sure that it doesn't immediately introduces a SA1134 after
    /// code fixing, but it will not / should not attempt to fix any preexisting SA1134 cases.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1133CodeFixProvider))]
    [Shared]
    internal class SA1133CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1133DoNotCombineAttributes.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1133CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1133CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var nodeInSourceSpan = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            AttributeListSyntax attributeList = nodeInSourceSpan.FirstAncestorOrSelf<AttributeListSyntax>();

            var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);
            var indentationSteps = IndentationHelper.GetIndentationSteps(settings.Indentation, attributeList);
            var indentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, indentationSteps);

            List<AttributeListSyntax> newAttributeLists = GetNewAttributeList(attributeList, indentationTrivia);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(attributeList, newAttributeLists);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }

        private static List<AttributeListSyntax> GetNewAttributeList(AttributeListSyntax attributeList, SyntaxTrivia indentationTrivia)
        {
            var newAttributeLists = new List<AttributeListSyntax>();

            for (var i = 0; i < attributeList.Attributes.Count; i++)
            {
                var newAttributes = SyntaxFactory.SingletonSeparatedList(
                    attributeList.Attributes[i].WithLeadingTrivia(
                        attributeList.Attributes[i].GetLeadingTrivia().WithoutLeadingWhitespace()));
                var newAttributeList = SyntaxFactory.AttributeList(attributeList.Target, newAttributes);

                newAttributeList = (i == 0)
                    ? newAttributeList.WithLeadingTrivia(attributeList.GetLeadingTrivia())
                    : newAttributeList.WithLeadingTrivia(indentationTrivia);

                newAttributeList = (i == (attributeList.Attributes.Count - 1))
                    ? newAttributeList.WithTrailingTrivia(attributeList.GetTrailingTrivia())
                    : newAttributeList.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

                newAttributeLists.Add(newAttributeList);
            }

            return newAttributeLists;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1133CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettings(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, fixAllContext.CancellationToken);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true).FirstAncestorOrSelf<AttributeListSyntax>());

                var newRoot = syntaxRoot.TrackNodes(nodes);

                foreach (var attributeList in nodes)
                {
                    var indentationSteps = IndentationHelper.GetIndentationSteps(settings.Indentation, attributeList);
                    var indentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(settings.Indentation, indentationSteps);
                    newRoot = newRoot.ReplaceNode(newRoot.GetCurrentNode(attributeList), GetNewAttributeList(attributeList, indentationTrivia));
                }

                return newRoot;
            }
        }
    }
}
