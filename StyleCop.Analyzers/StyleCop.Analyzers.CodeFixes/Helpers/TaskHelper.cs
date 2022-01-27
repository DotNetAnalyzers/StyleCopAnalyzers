﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class TaskHelper
    {
        public static bool IsTaskReturningType(SemanticModel semanticModel, TypeSyntax returnType, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, returnType, cancellationToken);
        }

        public static bool IsTaskReturningMethod(SemanticModel semanticModel, DelegateDeclarationSyntax delegateDeclarationSyntax, CancellationToken cancellationToken)
        {
            return IsTaskType(semanticModel, delegateDeclarationSyntax.ReturnType, cancellationToken);
        }

        public static bool IsTaskType(SemanticModel semanticModel, TypeSyntax typeSyntax, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(typeSyntax, cancellationToken);
            if (!(symbolInfo.Symbol is INamedTypeSymbol namedTypeSymbol))
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
