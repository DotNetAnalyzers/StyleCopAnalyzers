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
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                if (!(await CanFixAsync(context, diagnostic).ConfigureAwait(false)))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1130CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1134CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<bool> CanFixAsync(CodeFixContext context, Diagnostic diagnostic)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            var anonymousMethod = (AnonymousMethodExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

            var newNode = ReplaceWithLambda(semanticModel, anonymousMethod);

            return newNode != null;
        }

        private static SyntaxNode ReplaceWithLambda(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            var parameterList = anonymousMethod.ParameterList;
            SyntaxNode lambdaExpression;
            SyntaxToken arrowToken;

            if (parameterList == null)
            {
                ImmutableArray<string> argumentList = ImmutableArray<string>.Empty;

                switch (anonymousMethod.Parent.Kind())
                {
                case SyntaxKind.Argument:
                    argumentList = GetMethodInvocationArgumentList(semanticModel, anonymousMethod);
                    break;

                case SyntaxKind.EqualsValueClause:
                    argumentList = GetEqualsArgumentList(semanticModel, anonymousMethod);
                    break;

                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    var list = GetAssignmentArgumentList(semanticModel, anonymousMethod);

                    if (list == null)
                    {
                        return null;
                    }

                    argumentList = list.Value;
                    break;

                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.ReturnStatement:
                    argumentList = GetMemberReturnTypeArgumentList(semanticModel, anonymousMethod);
                    if (argumentList.IsEmpty)
                    {
                        return null;
                    }

                    break;
                }

                List<ParameterSyntax> parameters = GenerateUniqueParameterNames(semanticModel, anonymousMethod, argumentList);

                var newList = (parameters.Count > 0)
                    ? SyntaxFactory.SeparatedList(parameters, Enumerable.Repeat(ParameterListSeparator, parameters.Count - 1))
                    : SyntaxFactory.SeparatedList<ParameterSyntax>();

                parameterList = SyntaxFactory.ParameterList(newList)
                    .WithLeadingTrivia(anonymousMethod.DelegateKeyword.LeadingTrivia);

                arrowToken = SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)
                    .WithTrailingTrivia(anonymousMethod.DelegateKeyword.TrailingTrivia);
            }
            else
            {
                parameterList = parameterList.WithLeadingTrivia(anonymousMethod.DelegateKeyword.TrailingTrivia);

                arrowToken = SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken)
                    .WithTrailingTrivia(SyntaxFactory.ElasticSpace);
            }

            foreach (var parameter in parameterList.Parameters)
            {
                if (!IsValid(parameter))
                {
                    return anonymousMethod;
                }
            }

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

        private static ImmutableArray<string> GetMethodInvocationArgumentList(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            var argumentSyntax = (ArgumentSyntax)anonymousMethod.Parent;
            var argumentListSyntax = (BaseArgumentListSyntax)argumentSyntax.Parent;
            var originalInvocableExpression = argumentListSyntax.Parent;

            var originalSymbolInfo = semanticModel.GetSymbolInfo(originalInvocableExpression);
            var argumentIndex = argumentListSyntax.Arguments.IndexOf(argumentSyntax);
            var parameterList = SA1130UseLambdaSyntax.GetDelegateParameterList(originalSymbolInfo.Symbol, argumentIndex);
            return parameterList.Parameters.Select(p => p.Identifier.ToString()).ToImmutableArray();
        }

        private static ImmutableArray<string> GetEqualsArgumentList(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            var equalsValueClauseSyntax = (EqualsValueClauseSyntax)anonymousMethod.Parent;
            var variableDeclaration = (VariableDeclarationSyntax)equalsValueClauseSyntax.Parent.Parent;

            var symbol = semanticModel.GetSymbolInfo(variableDeclaration.Type);
            var namedTypeSymbol = (INamedTypeSymbol)symbol.Symbol;
            return namedTypeSymbol.DelegateInvokeMethod.Parameters.Select(ps => ps.Name).ToImmutableArray();
        }

        private static ImmutableArray<string>? GetAssignmentArgumentList(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            var assignmentExpressionSyntax = (AssignmentExpressionSyntax)anonymousMethod.Parent;

            var symbol = semanticModel.GetSymbolInfo(assignmentExpressionSyntax.Left);

            if (symbol.Symbol == null)
            {
                return null;
            }

            var eventSymbol = (IEventSymbol)symbol.Symbol;
            var namedTypeSymbol = (INamedTypeSymbol)eventSymbol.Type;
            return namedTypeSymbol.DelegateInvokeMethod.Parameters.Select(ps => ps.Name).ToImmutableArray();
        }

        private static ImmutableArray<string> GetMemberReturnTypeArgumentList(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod)
        {
            var enclosingSymbol = semanticModel.GetEnclosingSymbol(anonymousMethod.Parent.SpanStart);
            return !(((IMethodSymbol)enclosingSymbol).ReturnType is INamedTypeSymbol returnType) ? ImmutableArray<string>.Empty : returnType.DelegateInvokeMethod.Parameters.Select(ps => ps.Name).ToImmutableArray();
        }

        private static List<ParameterSyntax> GenerateUniqueParameterNames(SemanticModel semanticModel, AnonymousMethodExpressionSyntax anonymousMethod, ImmutableArray<string> argumentNames)
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

            var newNode = ReplaceWithLambda(semanticModel, anonymousMethod);

            var newSyntaxRoot = syntaxRoot.ReplaceNode(anonymousMethod, newNode);
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

                foreach (var diagnostic in diagnostics)
                {
                    var node = (AnonymousMethodExpressionSyntax)syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                    nodes.Add(node);
                }

                return syntaxRoot.ReplaceNodes(nodes, (originalNode, rewrittenNode) =>
                {
                    var newNode = ReplaceWithLambda(semanticModel, rewrittenNode);

                    if (newNode == null)
                    {
                        return rewrittenNode;
                    }

                    return newNode;
                });
            }
        }
    }
}
