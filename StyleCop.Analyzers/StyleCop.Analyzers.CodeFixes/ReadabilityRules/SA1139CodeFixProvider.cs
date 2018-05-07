// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
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
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Implements a code fix for <see cref="SA1139UseLiteralSuffixNotationInsteadOfCasting"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1139CodeFixProvider))]
    [Shared]
    internal class SA1139CodeFixProvider : CodeFixProvider
    {
        private static readonly Dictionary<SyntaxKind, string> LiteralSyntaxKindToSuffix = new Dictionary<SyntaxKind, string>()
            {
                { SyntaxKind.IntKeyword, string.Empty },
                { SyntaxKind.LongKeyword, "L" },
                { SyntaxKind.ULongKeyword, "UL" },
                { SyntaxKind.UIntKeyword, "U" },
                { SyntaxKind.FloatKeyword, "F" },
                { SyntaxKind.DoubleKeyword, "D" },
                { SyntaxKind.DecimalKeyword, "M" },
            };

        private static readonly char[] LettersAllowedInLiteralSuffix = LiteralSyntaxKindToSuffix.Values
            .SelectMany(s => s.ToCharArray()).Distinct()
            .SelectMany(c => new[] { char.ToLowerInvariant(c), c })
            .ToArray();

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1139UseLiteralSuffixNotationInsteadOfCasting.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1139CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1139CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var oldSemanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var node = syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) as CastExpressionSyntax;
            if (node == null)
            {
                return document;
            }

            var replacementNode = GenerateReplacementNode(node);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, replacementNode);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);
            var newSemanticModel = await newDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var newNode = newSemanticModel.SyntaxTree.GetRoot().FindNode(
                span: new TextSpan(start: node.FullSpan.Start, length: replacementNode.FullSpan.Length),
                getInnermostNodeForTie: true);

            var oldConstantValue = oldSemanticModel.GetConstantValue(node).Value;
            var newConstantValueOption = newSemanticModel.GetConstantValue(newNode, cancellationToken);
            if (newConstantValueOption.HasValue && oldConstantValue.Equals(newConstantValueOption.Value))
            {
                return newDocument;
            }
            else
            {
                var newNodeBasedOnValue = GenerateReplacementNodeBasedOnValue(node, oldConstantValue);
                newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNodeBasedOnValue);
                return document.WithSyntaxRoot(newSyntaxRoot);
            }
        }

        private static SyntaxNode GenerateReplacementNode(CastExpressionSyntax node)
        {
            var plusMinusSyntax = node.Expression as PrefixUnaryExpressionSyntax;
            var literalExpressionSyntax =
                plusMinusSyntax == null ?
                (LiteralExpressionSyntax)node.Expression :
                (LiteralExpressionSyntax)plusMinusSyntax.Operand;
            var typeToken = node.Type.GetFirstToken();
            var prefix = plusMinusSyntax == null
                ? string.Empty
                : plusMinusSyntax.OperatorToken.Text;
            var literalWithoutSuffix = literalExpressionSyntax.StripLiteralSuffix();
            var correspondingSuffix = LiteralSyntaxKindToSuffix[typeToken.Kind()];
            var fixedCodePreservingText = SyntaxFactory.ParseExpression(prefix + literalWithoutSuffix + correspondingSuffix);

            return fixedCodePreservingText.WithTriviaFrom(node);
        }

        private static SyntaxNode GenerateReplacementNodeBasedOnValue(CastExpressionSyntax node, object desiredValue)
        {
            var typeToken = node.Type.GetFirstToken();
            var correspondingSuffix = LiteralSyntaxKindToSuffix[typeToken.Kind()];
            var fixedCodePreservingText = SyntaxFactory.ParseExpression(desiredValue + correspondingSuffix);

            return fixedCodePreservingText.WithTriviaFrom(node);
        }
    }
}
