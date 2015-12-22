// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1129DoNotUseDefaultValueTypeConstructor"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1129CodeFixProvider))]
    [Shared]
    internal class SA1129CodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1129DoNotUseDefaultValueTypeConstructor.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1129CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1129CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var newExpression = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(newExpression, GetReplacementNode(newExpression, semanticModel, cancellationToken));

            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GetReplacementNode(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var newExpression = (ObjectCreationExpressionSyntax)node;

            SyntaxNode replacement = null;
            if (IsType<CancellationToken>(newExpression.Type, semanticModel, cancellationToken))
            {
                replacement = GetCancellationTokenNoneSyntax(newExpression.Type);
            }
            else
            {
                replacement = SyntaxFactory.DefaultExpression(newExpression.Type);
            }

            return replacement
                .WithLeadingTrivia(newExpression.GetLeadingTrivia())
                .WithTrailingTrivia(newExpression.GetTrailingTrivia());
        }

        private static bool IsType<T>(TypeSyntax typeSyntax, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var expectedType = typeof(T);
            var symbolInfo = semanticModel.GetSymbolInfo(typeSyntax, cancellationToken);
            var namedTypeSymbol = symbolInfo.Symbol as INamedTypeSymbol;

            if (namedTypeSymbol == null)
            {
                return false;
            }

            if (!string.Equals(expectedType.Name, namedTypeSymbol.Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(expectedType.Namespace, namedTypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)), StringComparison.Ordinal))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Gets a qualified member access expression for <c>CancellationToken.None</c>.
        /// </summary>
        /// <param name="typeSyntax">The type syntax from the original constructor.</param>
        /// <returns>A new member access expression.</returns>
        private static SyntaxNode GetCancellationTokenNoneSyntax(TypeSyntax typeSyntax)
        {
            return SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                typeSyntax,
                SyntaxFactory.IdentifierName(nameof(CancellationToken.None)));
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1129CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true));

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => GetReplacementNode(rewrittenNode, semanticModel, CancellationToken.None));
            }
        }
    }
}
