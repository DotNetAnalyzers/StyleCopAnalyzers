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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1135CodeFixProvider))]
    [Shared]
    internal class SA1135CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1135UsingDirectivesMustBeQualified.DiagnosticId);

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
                        ReadabilityResources.SA1135CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1135CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (!(syntaxRoot.FindNode(diagnostic.Location.SourceSpan) is UsingDirectiveSyntax node))
            {
                return document;
            }

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var replacementNode = GetReplacementNode(semanticModel, node, cancellationToken);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, replacementNode);
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static SyntaxNode GetReplacementNode(SemanticModel semanticModel, UsingDirectiveSyntax node, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(node.Name, cancellationToken);
            var symbolNameSyntax = SyntaxFactory.ParseName(symbolInfo.Symbol.ToQualifiedString(node.Name));

            var newName = GetReplacementName(symbolNameSyntax, node.Name);
            return node.WithName((NameSyntax)newName);
        }

        private static TypeSyntax GetReplacementName(TypeSyntax symbolNameSyntax, TypeSyntax nameSyntax)
        {
            switch (nameSyntax.Kind())
            {
            case SyntaxKind.GenericName:
                return GetReplacementGenericName(symbolNameSyntax, (GenericNameSyntax)nameSyntax);

            case SyntaxKind.QualifiedName:
                return GetReplacementQualifiedName((QualifiedNameSyntax)symbolNameSyntax, (QualifiedNameSyntax)nameSyntax);

            default:
                return symbolNameSyntax;
            }
        }

        private static NameSyntax GetReplacementGenericName(TypeSyntax symbolNameSyntax, GenericNameSyntax genericNameSyntax)
        {
            var symbolQualifiedNameSyntax = symbolNameSyntax as QualifiedNameSyntax;
            var symbolGenericNameSyntax = (GenericNameSyntax)(symbolQualifiedNameSyntax?.Right ?? symbolNameSyntax);

            TypeArgumentListSyntax newTypeArgumentList = GetReplacementTypeArgumentList(symbolGenericNameSyntax, genericNameSyntax);

            if (symbolQualifiedNameSyntax != null)
            {
                var newRightPart = ((GenericNameSyntax)symbolQualifiedNameSyntax.Right).WithTypeArgumentList(newTypeArgumentList);
                return symbolQualifiedNameSyntax.WithRight(newRightPart);
            }

            return genericNameSyntax.WithTypeArgumentList(newTypeArgumentList);
        }

        private static TypeArgumentListSyntax GetReplacementTypeArgumentList(GenericNameSyntax symbolGenericNameSyntax, GenericNameSyntax genericNameSyntax)
        {
            var replacements = new Dictionary<TypeSyntax, TypeSyntax>();
            for (var i = 0; i < genericNameSyntax.TypeArgumentList.Arguments.Count; i++)
            {
                var argument = genericNameSyntax.TypeArgumentList.Arguments[i];

                if (!argument.IsKind(SyntaxKind.PredefinedType))
                {
                    var symbolArgument = symbolGenericNameSyntax.TypeArgumentList.Arguments[i];

                    var replacementArgument = GetReplacementName(symbolArgument, argument)
                        .WithLeadingTrivia(argument.GetLeadingTrivia())
                        .WithTrailingTrivia(argument.GetTrailingTrivia());

                    replacements.Add(argument, replacementArgument);
                }
            }

            var newTypeArgumentList = genericNameSyntax.TypeArgumentList.ReplaceNodes(replacements.Keys, (original, maybeRewritten) => replacements[original]);
            return newTypeArgumentList;
        }

        private static NameSyntax GetReplacementQualifiedName(QualifiedNameSyntax symbolNameSyntax, QualifiedNameSyntax nameSyntax)
        {
            if (nameSyntax.Right.IsKind(SyntaxKind.GenericName))
            {
                return GetReplacementGenericName(symbolNameSyntax, (GenericNameSyntax)nameSyntax.Right);
            }
            else
            {
                return symbolNameSyntax;
            }
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                   new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.SA1135CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                SyntaxNode syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                SemanticModel semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var nodes = diagnostics.Select(diagnostic => syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true).FirstAncestorOrSelf<UsingDirectiveSyntax>());

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => GetReplacementNode(semanticModel, rewrittenNode, fixAllContext.CancellationToken));
            }
        }
    }
}
