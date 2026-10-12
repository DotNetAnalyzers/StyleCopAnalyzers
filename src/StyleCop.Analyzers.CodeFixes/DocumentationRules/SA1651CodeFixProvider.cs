// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
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
    /// Implements a code fix for <see cref="SA1651DoNotUsePlaceholderElements"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, review the placeholder documentation for accuracy and remove the
    /// &lt;placeholder&gt; tags.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1651CodeFixProvider))]
    [Shared]
    internal class SA1651CodeFixProvider : CodeFixProvider
    {
        private static readonly SyntaxAnnotation NodeToReplaceAnnotation = new SyntaxAnnotation(nameof(NodeToReplaceAnnotation));

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1651DoNotUsePlaceholderElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (diagnostic.Properties.ContainsKey(SA1651DoNotUsePlaceholderElements.NoCodeFixKey))
                {
                    // skip diagnostics that should not offer a code fix.
                    continue;
                }

                var documentRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
                SyntaxNode syntax = documentRoot.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);
                if (syntax == null)
                {
                    continue;
                }

                if (!(syntax is XmlElementSyntax xmlElementSyntax))
                {
                    // We continue even for placeholders if they are empty elements (XmlEmptyElementSyntax)
                    continue;
                }

                if (string.IsNullOrWhiteSpace(xmlElementSyntax.Content.ToString()))
                {
                    // The placeholder hasn't been updated yet.
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1651CodeFix,
                        cancellationToken => this.GetTransformedDocumentAsync(context.Document, xmlElementSyntax, cancellationToken),
                        nameof(SA1651CodeFixProvider)),
                    diagnostic);
            }
        }

        private static IEnumerable<XmlNodeSyntax> RemovePlaceHolder(XmlElementSyntax elementSyntax)
        {
            SyntaxList<XmlNodeSyntax> content = elementSyntax.Content;

            var leadingTrivia = elementSyntax.StartTag.GetLeadingTrivia();
            leadingTrivia = leadingTrivia.AddRange(elementSyntax.StartTag.GetTrailingTrivia());
            leadingTrivia = leadingTrivia.AddRange(content[0].GetLeadingTrivia());
            content = content.Replace(content[0], content[0].WithLeadingTrivia(leadingTrivia));

            var trailingTrivia = content[content.Count - 1].GetTrailingTrivia();
            trailingTrivia = trailingTrivia.AddRange(elementSyntax.EndTag.GetLeadingTrivia());
            trailingTrivia = trailingTrivia.AddRange(elementSyntax.EndTag.GetTrailingTrivia());
            content = content.Replace(content[content.Count - 1], content[content.Count - 1].WithTrailingTrivia(trailingTrivia));

            return content;
        }

        private async Task<Document> GetTransformedDocumentAsync(Document document, XmlElementSyntax elementSyntax, CancellationToken cancellationToken)
        {
            if (elementSyntax.Content.Count == 0)
            {
                return document;
            }

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = root.ReplaceNode(elementSyntax, RemovePlaceHolder(elementSyntax));
            return document.WithSyntaxRoot(newRoot);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAll Instance { get; } = new FixAll();

            protected override string CodeActionTitle { get; } = DocumentationResources.SA1651CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var elements = new List<XmlElementSyntax>();

                foreach (var diagnostic in diagnostics)
                {
                    if ((syntaxRoot.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true) is XmlElementSyntax xmlElement)
                        && (xmlElement.Content.Count > 0)
                        && !string.IsNullOrWhiteSpace(xmlElement.Content.ToString()))
                    {
                        elements.Add(xmlElement);
                    }
                }

                var newSyntaxRoot = syntaxRoot.ReplaceNodes(elements, (original, rewritten) => rewritten.WithAdditionalAnnotations(NodeToReplaceAnnotation));
                newSyntaxRoot = new FixAllVisitor().Visit(newSyntaxRoot);
                return newSyntaxRoot;
            }
        }

        private class FixAllVisitor : CSharpSyntaxRewriter
        {
            public FixAllVisitor()
                : base(true)
            {
            }

            public override SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list)
            {
                list = base.VisitList(list);

                var index = 0;
                while (index < list.Count)
                {
                    var element = list[index];
                    if (element.HasAnnotation(NodeToReplaceAnnotation))
                    {
                        list = list.ReplaceRange(element, RemovePlaceHolder(element as XmlElementSyntax).Cast<TNode>());
                    }
                    else
                    {
                        index++;
                    }
                }

                return list;
            }
        }
    }
}
