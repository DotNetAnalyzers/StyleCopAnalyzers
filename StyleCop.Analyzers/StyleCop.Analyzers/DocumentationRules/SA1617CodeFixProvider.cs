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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1617VoidReturnValueMustNotBeDocumented"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the <c>&lt;returns&gt;</c> tag from the element.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1617CodeFixProvider))]
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
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics.Where(d => FixableDiagnostics.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(DocumentationResources.SA1617CodeFix, token => GetTransformedDocumentAsync(context.Document, diagnostic, token), equivalenceKey: nameof(SA1617CodeFixProvider)), diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            var documentation = node.GetDocumentationCommentTriviaSyntax();

            // Check if the return value is documented
            var returnsElement = documentation.Content.GetFirstXmlElement(XmlCommentHelper.ReturnsXmlTag);

            if (returnsElement == null)
            {
                return document;
            }

            // Find the node previous to the <returns> node to determine if it is an XML comment indicator. If so, we
            // will remove that node from the syntax tree as well.
            SyntaxNode previous = null;
            foreach (var item in documentation.ChildNodes())
            {
                if (item.Equals(returnsElement))
                {
                    break;
                }

                previous = item;
            }

            List<SyntaxNode> nodesToFix = new List<SyntaxNode>();
            nodesToFix.Add(returnsElement);

            var previousAsTextSyntax = previous as XmlTextSyntax;
            if (previousAsTextSyntax != null && XmlCommentHelper.IsConsideredEmpty(previousAsTextSyntax))
            {
                nodesToFix.Add(previous);
            }

            var newSyntaxRoot = root.RemoveNodes(nodesToFix, SyntaxRemoveOptions.KeepLeadingTrivia);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }
    }
}
