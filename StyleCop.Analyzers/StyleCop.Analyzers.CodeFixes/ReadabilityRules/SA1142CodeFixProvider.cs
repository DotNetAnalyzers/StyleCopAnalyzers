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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1142CodeFixProvider))]
    [Shared]
    internal class SA1142CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1142ReferToTupleElementsByName.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => FixAll.Instance;

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1142CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1141CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var replacementNode = GetReplacementNode(semanticModel, node);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GetReplacementNode(SemanticModel semanticModel, SyntaxNode fieldName)
        {
            var fieldSymbol = (IFieldSymbol)semanticModel.GetSymbolInfo(fieldName.Parent).Symbol;
            var fieldNameSymbol = fieldSymbol.ContainingType.GetMembers().OfType<IFieldSymbol>().Single(fs => !Equals(fs, fieldSymbol) && Equals(fs.CorrespondingTupleField(), fieldSymbol));

            return SyntaxFactory.IdentifierName(fieldNameSymbol.Name).WithTriviaFrom(fieldName);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            /// <inheritdoc/>
            protected override string CodeActionTitle => ReadabilityResources.SA1142CodeFix;

            /// <inheritdoc/>
            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var replaceMap = new Dictionary<SyntaxNode, SyntaxNode>();

                foreach (var diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                    replaceMap[node] = GetReplacementNode(semanticModel, node);
                }

                return syntaxRoot.ReplaceNodes(replaceMap.Keys, (original, rewritten) => replaceMap[original]);
            }
        }
    }
}
