﻿namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Editing;
    using StyleCop.Analyzers.Helpers;

    internal class SA1107FixAllProvider : DocumentBasedFixAllProvider
    {
        protected override string CodeActionTitle => ReadabilityResources.SA1107CodeFix;

        protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
        {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
            if (diagnostics.IsEmpty)
            {
                return null;
            }

            DocumentEditor editor = await DocumentEditor.CreateAsync(document, fixAllContext.CancellationToken).ConfigureAwait(false);

            SyntaxNode root = editor.GetChangedRoot();

            ImmutableList<SyntaxNode> nodesToChange = ImmutableList.Create<SyntaxNode>();

            // Make sure all nodes we care about are tracked
            foreach (var diagnostic in diagnostics)
            {
                var location = diagnostic.Location;
                var syntaxNode = root.FindNode(location.SourceSpan);
                if (syntaxNode != null)
                {
                    editor.TrackNode(syntaxNode);
                    nodesToChange = nodesToChange.Add(syntaxNode);
                }
            }

            foreach (var node in nodesToChange)
            {
                editor.ReplaceNode(node, node.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
            }

            return editor.GetChangedRoot();
        }
    }
}
