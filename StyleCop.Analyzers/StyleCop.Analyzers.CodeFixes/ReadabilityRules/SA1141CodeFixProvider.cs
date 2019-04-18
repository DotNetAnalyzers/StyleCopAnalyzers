// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Simplification;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Lightup;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1141CodeFixProvider))]
    [Shared]
    internal class SA1141CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1141UseTupleSyntax.DiagnosticId);

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1141CodeFix,
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
            if (node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                // get the invocation node when processing ValueTuple.Create, as that needs to be replaced.
                node = node.Parent;
            }

            var newNode = GetReplacementNode(semanticModel, node);

            // doing our own formatting, as the default formatting for operators is incompatible with StyleCop.Analyzers.
            var separatorRewriter = new SeparatorRewriter();
            newNode = separatorRewriter.Visit(newNode);

            switch (node.Parent.Kind())
            {
            case SyntaxKind.MethodDeclaration:
            case SyntaxKind.Parameter:
            case SyntaxKind.PropertyDeclaration:
            case SyntaxKind.IndexerDeclaration:
            case SyntaxKind.DelegateDeclaration:
                newNode = newNode.WithTrailingTrivia(SyntaxFactory.Space);
                break;
            }

            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode).WithAdditionalAnnotations(Simplifier.Annotation);
            return document.WithSyntaxRoot(newSyntaxRoot.WithoutFormatting());
        }

        private static SyntaxNode GetReplacementNode(SemanticModel semanticModel, SyntaxNode node)
        {
            switch (node)
            {
            case QualifiedNameSyntax qualifiedNameSyntax:
                return TransformGenericNameToTuple(semanticModel, (GenericNameSyntax)qualifiedNameSyntax.Right);

            case GenericNameSyntax genericNameSyntax:
                return TransformGenericNameToTuple(semanticModel, genericNameSyntax);

            case ObjectCreationExpressionSyntax objectCreationExpression:
                return TransformArgumentListToTuple(semanticModel, objectCreationExpression.ArgumentList.Arguments);

            case InvocationExpressionSyntax invocationExpressionSyntax:
                return TransformArgumentListToTuple(semanticModel, invocationExpressionSyntax.ArgumentList.Arguments);

            default:
                return node;
            }
        }

        private static SyntaxNode TransformGenericNameToTuple(SemanticModel semanticModel, GenericNameSyntax genericName)
        {
            var implementationType = typeof(SeparatedSyntaxListWrapper<>.AutoWrapSeparatedSyntaxList<>).MakeGenericType(typeof(TupleElementSyntaxWrapper), WrapperHelper.GetWrappedType(typeof(TupleElementSyntaxWrapper)));
            var tupleElements = (SeparatedSyntaxListWrapper<TupleElementSyntaxWrapper>)Activator.CreateInstance(implementationType);

            foreach (var typeArgument in genericName.TypeArgumentList.Arguments)
            {
                if (IsValueTuple(semanticModel, typeArgument))
                {
                    var tupleTypeSyntax = (TypeSyntax)GetReplacementNode(semanticModel, typeArgument);
                    tupleElements = tupleElements.Add(SyntaxFactoryEx.TupleElement(tupleTypeSyntax));
                }
                else
                {
                    tupleElements = tupleElements.Add(SyntaxFactoryEx.TupleElement(typeArgument));
                }
            }

            return SyntaxFactoryEx.TupleType(tupleElements);
        }

        private static SyntaxNode TransformArgumentListToTuple(SemanticModel semanticModel, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            SeparatedSyntaxList<ArgumentSyntax> processedArguments = default;

            for (var i = 0; i < arguments.Count; i++)
            {
                var argument = arguments[i];

                var argumentTypeInfo = semanticModel.GetTypeInfo(argument.Expression);
                if (argumentTypeInfo.Type != argumentTypeInfo.ConvertedType)
                {
                    var expectedType = SyntaxFactory.ParseTypeName(argumentTypeInfo.ConvertedType.ToDisplayString());
                    argument = argument.WithExpression(SyntaxFactory.CastExpression(expectedType, argument.Expression));
                }

                processedArguments = processedArguments.Add(argument);
            }

            return SyntaxFactoryEx.TupleExpression(processedArguments);
        }

        private static bool IsValueTuple(SemanticModel semanticModel, TypeSyntax typeSyntax)
        {
            if (typeSyntax.IsKind(SyntaxKindEx.TupleType))
            {
                return false;
            }

            var symbolInfo = semanticModel.GetSymbolInfo(typeSyntax);
            return (symbolInfo.Symbol is ITypeSymbol typeSymbol) && typeSymbol.IsTupleType();
        }

        private class SeparatorRewriter : CSharpSyntaxRewriter
        {
            public SeparatorRewriter()
                : base(false)
            {
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (token.IsKind(SyntaxKind.CommaToken))
                {
                    token = token.WithTrailingTrivia(SyntaxFactory.Space);
                }

                return base.VisitToken(token);
            }
        }
    }
}
