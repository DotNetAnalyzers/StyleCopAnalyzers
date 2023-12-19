// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxNodeExtensions
    {
        public static bool IsInExpressionTree(
            this SyntaxNode? node,
            SemanticModel semanticModel,
            INamedTypeSymbol? expressionType,
            CancellationToken cancellationToken)
        {
            if (expressionType != null)
            {
                for (var current = node; current != null; current = current.Parent)
                {
                    if (current.IsAnyLambda())
                    {
                        var typeInfo = semanticModel.GetTypeInfo(current, cancellationToken);
                        if (expressionType.Equals(typeInfo.ConvertedType?.OriginalDefinition))
                        {
                            return true;
                        }
                    }
                    else if (current is SelectOrGroupClauseSyntax or OrderingSyntax)
                    {
                        var info = semanticModel.GetSymbolInfo(current, cancellationToken);
                        if (AnyTakesExpressionTree(info, expressionType))
                        {
                            return true;
                        }
                    }
                    else if (current is QueryClauseSyntax queryClause)
                    {
                        var info = semanticModel.GetQueryClauseInfo(queryClause, cancellationToken);
                        if (AnyTakesExpressionTree(info.CastInfo, expressionType)
                            || AnyTakesExpressionTree(info.OperationInfo, expressionType))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

            static bool AnyTakesExpressionTree(SymbolInfo info, INamedTypeSymbol expressionType)
            {
                if (TakesExpressionTree(info.Symbol, expressionType))
                {
                    return true;
                }

                foreach (var symbol in info.CandidateSymbols)
                {
                    if (TakesExpressionTree(symbol, expressionType))
                    {
                        return true;
                    }
                }

                return false;
            }

            static bool TakesExpressionTree(ISymbol symbol, INamedTypeSymbol expressionType)
            {
                if (symbol is IMethodSymbol method
                    && method.Parameters.Length > 0
                    && expressionType.Equals(method.Parameters[0].Type?.OriginalDefinition))
                {
                    return true;
                }

                return false;
            }
        }

        public static bool IsAnyLambda(this SyntaxNode? node)
        {
            return node.IsKind(SyntaxKind.ParenthesizedLambdaExpression)
                || node.IsKind(SyntaxKind.SimpleLambdaExpression);
        }
    }
}
