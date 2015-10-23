// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// Implements a code fix for <see cref="SA1404CodeAnalysisSuppressionMustHaveJustification"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, add a justification to your suppression.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1404CodeFixProvider))]
    [Shared]
    internal class SA1404CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1404CodeAnalysisSuppressionMustHaveJustification.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var node = root.FindNode(diagnostic.Location.SourceSpan);

                var attribute = node as AttributeSyntax;
                if (attribute != null)
                {
                    // In this case there is no justification at all
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            MaintainabilityResources.SA1404CodeFix,
                            token => AddJustificationToAttributeAsync(context.Document, root, attribute),
                            nameof(SA1404CodeFixProvider) + "-Add"), diagnostic);
                    return;
                }

                var argument = node as AttributeArgumentSyntax;
                if (argument != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            MaintainabilityResources.SA1404CodeFix,
                            token => UpdateValueOfArgumentAsync(context.Document, root, argument),
                            nameof(SA1404CodeFixProvider) + "-Update"), diagnostic);
                    return;
                }
            }
        }

        private static Task<Document> UpdateValueOfArgumentAsync(Document document, SyntaxNode root, AttributeArgumentSyntax argument)
        {
            var newArgument = argument.WithExpression(GetNewAttributeValue());
            return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(argument, newArgument)));
        }

        private static Task<Document> AddJustificationToAttributeAsync(Document document, SyntaxNode syntaxRoot, AttributeSyntax attribute)
        {
            var attributeName = SyntaxFactory.IdentifierName(nameof(SuppressMessageAttribute.Justification));
            var newArgument = SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(attributeName), null, GetNewAttributeValue());

            var newArgumentList = attribute.ArgumentList.AddArguments(newArgument);
            return Task.FromResult(document.WithSyntaxRoot(syntaxRoot.ReplaceNode(attribute.ArgumentList, newArgumentList)));
        }

        private static LiteralExpressionSyntax GetNewAttributeValue()
        {
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder));
        }
    }
}
