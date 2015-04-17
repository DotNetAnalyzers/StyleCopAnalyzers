namespace StyleCop.Analyzers.DocumentationRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using System.Collections.Generic;

    [ExportCodeFixProvider(nameof(SA1617CodeFixProvider), LanguageNames.CSharp)]
    [Shared]
    public class SA1617CodeFixProvider : CodeFixProvider
    {
        private static readonly ImmutableArray<string> FixableDiagnostics =
          ImmutableArray.Create(SA1617VoidReturnValueMustNotBeDocumented.DiagnosticId);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds => FixableDiagnostics;

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (!diagnostic.Id.Equals(SA1617VoidReturnValueMustNotBeDocumented.DiagnosticId))
                {
                    continue;
                }

                var node = root.FindNode(diagnostic.Location.SourceSpan);
                if (node.IsMissing)
                {
                    continue;
                }

                var documentation = XmlCommentHelper.GetDocumentationStructure(node);
                // Check if the return value is documented
                var returnsElement = XmlCommentHelper.GetTopLevelElement(documentation, XmlCommentHelper.ReturnsXmlTag);

                if (returnsElement != null)
                {
                    SyntaxNode previous = null;
                    foreach (var item in documentation.ChildNodes())
                    {
                        if (item.Equals(returnsElement))
                            break;
                                        
                        previous = item;
                    }

                    List<SyntaxNode> nodesToFix = new List<SyntaxNode>();
                    nodesToFix.Add(returnsElement);
                    if (previous?.ToFullString().Trim() == "///")
                        nodesToFix.Add(previous);

                    context.RegisterCodeFix(CodeAction.Create($"Remove <returns> XML comment.", cancellationToken => RemoveHelper.RemoveSymbolsAsync(document, root, nodesToFix, SyntaxRemoveOptions.KeepLeadingTrivia, cancellationToken)), diagnostic);
                }
            }
        }
    }
}