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
    /// Implements a code fix for <see cref="SA1121UseBuiltInTypeAlias"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, ensure that the comma is followed by a single space, and is not preceded
    /// by any space.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1121CodeFixProvider))]
    [Shared]
    internal class SA1121CodeFixProvider : CodeFixProvider
    {
        private static readonly Dictionary<SpecialType, PredefinedTypeSyntax> PredefinedSpecialTypes = new Dictionary<SpecialType, PredefinedTypeSyntax>
        {
            [SpecialType.System_Boolean] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
            [SpecialType.System_Byte] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
            [SpecialType.System_Char] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword)),
            [SpecialType.System_Decimal] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
            [SpecialType.System_Double] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),
            [SpecialType.System_Int16] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword)),
            [SpecialType.System_Int32] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            [SpecialType.System_Int64] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
            [SpecialType.System_Object] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
            [SpecialType.System_SByte] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)),
            [SpecialType.System_Single] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
            [SpecialType.System_String] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
            [SpecialType.System_UInt16] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
            [SpecialType.System_UInt32] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword)),
            [SpecialType.System_UInt64] = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword))
        };

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1121UseBuiltInTypeAlias.DiagnosticId);

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
                        ReadabilityResources.SA1121CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1121CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static SyntaxNode ComputeReplacement(SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
        {
            var memberAccess = node.Parent as MemberAccessExpressionSyntax;
            if (memberAccess != null)
            {
                if (node == memberAccess.Name)
                {
                    node = memberAccess;
                }
            }

            var type = semanticModel.GetSymbolInfo(node, cancellationToken).Symbol as INamedTypeSymbol;

            PredefinedTypeSyntax typeSyntax;
            if (!PredefinedSpecialTypes.TryGetValue(type.SpecialType, out typeSyntax))
            {
                return node;
            }

            SyntaxNode newNode;
            if (node is CrefSyntax)
            {
                newNode = SyntaxFactory.TypeCref(typeSyntax);
            }
            else
            {
                newNode = typeSyntax;
            }

            return newNode.WithTriviaFrom(node).WithoutFormatting();
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (semanticModel == null)
            {
                return document;
            }

            var node = root.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

            var newNode = ComputeReplacement(semanticModel, node, cancellationToken);

            var newRoot = root.ReplaceNode(node, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle
                => ReadabilityResources.SA1121CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document)
            {
                var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                List<SyntaxNode> nodesToReplace = new List<SyntaxNode>(diagnostics.Length);
                foreach (var diagnostic in diagnostics)
                {
                    var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, findInsideTrivia: true, getInnermostNodeForTie: true);

                    nodesToReplace.Add(node);
                }

                return syntaxRoot.ReplaceNodes(nodesToReplace, (originalNode, rewrittenNode) => ComputeReplacement(semanticModel, originalNode, fixAllContext.CancellationToken));
            }
        }
    }
}
