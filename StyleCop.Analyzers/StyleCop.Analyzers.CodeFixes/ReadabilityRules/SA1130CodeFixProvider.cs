// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
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
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1130UseLambdaSyntax"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1130CodeFixProvider))]
    [Shared]
    internal class SA1130CodeFixProvider : CodeFixProvider
    {
        private static readonly SyntaxToken ParameterListSeparator = SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(SyntaxFactory.Space);

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1130UseLambdaSyntax.DiagnosticId);

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
                        ReadabilityResources.SA1130CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1134CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static SyntaxNode ReplaceWithLambda(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod, string argumentsProperty)
        {
            var parameterList = anonymousMethod.ParameterList;
            SyntaxNode lambdaExpression;

            if (parameterList == null)
            {
                SeparatedSyntaxList<ParameterSyntax> newList = default(SeparatedSyntaxList<ParameterSyntax>);

                if (!string.IsNullOrEmpty(argumentsProperty))
                {
                    var argumentNames = argumentsProperty.Split(',');
                    List<ParameterSyntax> parameters = GenerateUniqueParameterNames(semanticModel, anonymousMethod, argumentNames);

                    newList = SyntaxFactory.SeparatedList(parameters, Enumerable.Repeat(ParameterListSeparator, parameters.Count - 1));
                }

                parameterList = SyntaxFactory.ParameterList(newList)
                    .WithLeadingTrivia(anonymousMethod.DelegateKeyword.LeadingTrivia)
                    .WithTrailingTrivia(anonymousMethod.DelegateKeyword.TrailingTrivia);
            }
            else
            {
                parameterList = parameterList.WithLeadingTrivia(anonymousMethod.DelegateKeyword.TrailingTrivia);
            }

            foreach (var parameter in parameterList.Parameters)
            {
                if (!IsValid(parameter))
                {
                    return anonymousMethod;
                }
            }

            var arrowToken = SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)
                .WithTrailingTrivia(SyntaxFactory.ElasticSpace);

            if (parameterList.Parameters.Count == 1)
            {
                var parameterSyntax = RemoveType(parameterList.Parameters[0]);

                var trailingTrivia = parameterSyntax.GetTrailingTrivia()
                    .Concat(parameterList.CloseParenToken.LeadingTrivia)
                    .Concat(parameterList.CloseParenToken.TrailingTrivia.WithoutTrailingWhitespace())
                    .Concat(new[] { SyntaxFactory.ElasticSpace });
                var leadingTrivia = parameterList.OpenParenToken.LeadingTrivia
                    .Concat(parameterList.OpenParenToken.TrailingTrivia)
                    .Concat(parameterSyntax.GetLeadingTrivia());

                parameterSyntax = parameterSyntax
                    .WithLeadingTrivia(leadingTrivia)
                    .WithTrailingTrivia(trailingTrivia);

                lambdaExpression = SyntaxFactory.SimpleLambdaExpression(anonymousMethod.AsyncKeyword, parameterSyntax, arrowToken, anonymousMethod.Body);
            }
            else
            {
                var parameterListSyntax = RemoveType(parameterList)
                    .WithTrailingTrivia(parameterList.GetTrailingTrivia().WithoutTrailingWhitespace().Add(SyntaxFactory.ElasticSpace));
                lambdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(anonymousMethod.AsyncKeyword, parameterListSyntax, arrowToken, anonymousMethod.Body);
            }

            return lambdaExpression
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private static List<ParameterSyntax> GenerateUniqueParameterNames(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod, string[] argumentNames)
        {
            var parameters = new List<ParameterSyntax>();

            foreach (var argumentName in argumentNames)
            {
                var baseName = argumentName;
                var newName = baseName;
                var index = 0;

                while (semanticModel.LookupSymbols(anonymousMethod.SpanStart, name: newName).Length > 0)
                {
                    index++;
                    newName = baseName + index;
                }

                parameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(newName)).WithType(null));
            }

            return parameters;
        }

        private static ParameterListSyntax RemoveType(ParameterListSyntax parameterList)
        {
            return parameterList.WithParameters(SyntaxFactory.SeparatedList(parameterList.Parameters.Select(x => RemoveType(x)), parameterList.Parameters.GetSeparators()));
        }

        private static ParameterSyntax RemoveType(ParameterSyntax parameterSyntax)
        {
            var syntax = parameterSyntax.WithType(null);

            if (parameterSyntax.Type != null)
            {
                syntax = syntax.WithLeadingTrivia(parameterSyntax.Type.GetLeadingTrivia().Concat(parameterSyntax.Type.GetTrailingTrivia()));
            }

            return syntax.WithTrailingTrivia(syntax.GetTrailingTrivia().WithoutTrailingWhitespace())
                .WithLeadingTrivia(syntax.GetLeadingTrivia().WithoutWhitespace());
        }

        private static bool IsValid(ParameterSyntax parameterSyntax)
        {
            // If one of the following conditions is false the code won't compile, but we want to check for it anyway and not make it worse by applying this code fix.
            return parameterSyntax.AttributeLists.Count == 0
                && parameterSyntax.Default == null
                && parameterSyntax.Modifiers.Count == 0
                && !parameterSyntax.Identifier.IsKind(SyntaxKind.ArgListKeyword);
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var anonymousMethod = (AnonymousMethodExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

            var argumentsProperty = diagnostic.Properties.ContainsKey(SA1130UseLambdaSyntax.DelegateArgumentNamesProperty) ? diagnostic.Properties[SA1130UseLambdaSyntax.DelegateArgumentNamesProperty] : string.Empty;
            var newSyntaxRoot = syntaxRoot.ReplaceNode(anonymousMethod, ReplaceWithLambda(semanticModel, anonymousMethod, argumentsProperty));
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());

            return newDocument;
        }

        private class FixAll : DocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle => ReadabilityResources.SA1130CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var semanticModel = await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var nodes = new List<AnonymousMethodExpressionSyntax>();
                var nodeProperties = new Dictionary<AnonymousMethodExpressionSyntax, string>();

                foreach (var diagnostic in diagnostics)
                {
                    var argumentsProperty = diagnostic.Properties.ContainsKey(SA1130UseLambdaSyntax.DelegateArgumentNamesProperty) ? diagnostic.Properties[SA1130UseLambdaSyntax.DelegateArgumentNamesProperty] : string.Empty;
                    var node = (AnonymousMethodExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

                    nodes.Add(node);
                    nodeProperties.Add(node, argumentsProperty);
                }

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) => ReplaceWithLambda(semanticModel, rewrittenNode, nodeProperties[rewrittenNode]));
            }
        }
    }
}
