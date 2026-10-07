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

        /// <summary>
        /// Finds the nearest ancestor of the specified syntax node that is the first token on its line.
        /// </summary>
        /// <remarks><para>This method is useful for scenarios such as handling 'else if' statements, where the
        /// initial node may not be the first on its line. The search ascends the syntax tree until a suitable ancestor
        /// is found.</para></remarks>
        /// <param name="parent">The syntax node from which to begin searching for an ancestor that starts a line.</param>
        /// <returns>A syntax node that is the first token on its line. If the specified node is already the first on its line,
        /// returns the node itself.</returns>
        public static SyntaxNode GetFirstOnLineAncestorOrSelf(this SyntaxNode parent)
        {
            // if the parent is not the first on a line, find the parent that is.
            // This mainly happens for 'else if' statements.
            while (!parent.GetFirstToken().IsFirstInLine())
            {
                parent = parent.Parent;
            }

            return parent;
        }
    }
}
