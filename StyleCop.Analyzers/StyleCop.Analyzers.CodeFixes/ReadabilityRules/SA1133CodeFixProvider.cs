// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
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

    /// <summary>
    /// Implements a code fix for <see cref="SA1133DoNotCombineAttributes"/>.
    /// </summary>
    /// <remarks>
    /// The SA1133 code fix adds the new lines to make sure that it doesn't immediately introduces a SA1134 after code fixing,
    /// but it will not / should not attempt to fix any preexisting SA1134 cases.
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
            return CustomFixAllProviders.BatchFixer;
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
            var nodeInSourceSpan = syntaxRoot.FindNode(diagnostic.Location.SourceSpan);
            AttributeListSyntax attributeList;
            if (nodeInSourceSpan.Parent is AttributeListSyntax)
            {
                attributeList = (AttributeListSyntax)nodeInSourceSpan.Parent;
            }
            else if (nodeInSourceSpan.Parent is AttributeSyntax)
            {
                var violatingAttribute = (AttributeSyntax)nodeInSourceSpan.Parent;
                attributeList = (AttributeListSyntax)violatingAttribute.Parent;
            }
            else
            {
                return document;
            }

            var newAttributeLists = new List<AttributeListSyntax>();

            var indentationOptions = IndentationOptions.FromDocument(document);
            var indentationSteps = IndentationHelper.GetIndentationSteps(indentationOptions, attributeList);
            var indentationTrivia = IndentationHelper.GenerateWhitespaceTrivia(indentationOptions, indentationSteps);

            for (var i = 0; i < attributeList.Attributes.Count; i++)
            {
                var newAttributes = SyntaxFactory.SingletonSeparatedList(attributeList.Attributes[i]);
                var newAttributeList = SyntaxFactory.AttributeList(attributeList.Target, newAttributes);

                newAttributeList = (i == 0)
                    ? newAttributeList.WithLeadingTrivia(attributeList.GetLeadingTrivia())
                    : newAttributeList.WithLeadingTrivia(indentationTrivia);

                newAttributeList = (i == (attributeList.Attributes.Count - 1))
                    ? newAttributeList.WithTrailingTrivia(attributeList.GetTrailingTrivia())
                    : newAttributeList.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

                newAttributeLists.Add(newAttributeList);
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNode(attributeList, newAttributeLists);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }
    }
}
