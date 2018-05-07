// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class TaskHelper
    {
        public static bool IsTaskReturningMethod(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclarationSyntax, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, methodDeclarationSyntax.ReturnType, cancellationToken);
        }

        public static bool IsTaskReturningMethod(SemanticModel semanticModel, DelegateDeclarationSyntax delegateDeclarationSyntax, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, delegateDeclarationSyntax.ReturnType, cancellationToken);
        }

        public static bool IsTaskType(SemanticModel semanticModel, TypeSyntax typeSyntax, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(typeSyntax, cancellationToken);
            INamedTypeSymbol namedTypeSymbol = symbolInfo.Symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
            {
                return false;
            }

            if (!string.Equals(nameof(Task), namedTypeSymbol.Name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(typeof(Task).Namespace, namedTypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)), StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
