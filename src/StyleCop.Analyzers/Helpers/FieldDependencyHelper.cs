// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class FieldDependencyHelper
{
    internal static Dictionary<string, ImmutableHashSet<string>> ComputeFieldDependencies(
        SemanticModel semanticModel,
        SyntaxList<MemberDeclarationSyntax> members,
        CancellationToken cancellationToken)
    {
        var fieldDeclarations = new List<FieldDeclarationSyntax>();
        var fieldNames = new HashSet<string>();

        foreach (var member in members)
        {
            if (member is FieldDeclarationSyntax field)
            {
                fieldDeclarations.Add(field);

                foreach (var variable in field.Declaration.Variables)
                {
                    fieldNames.Add(variable.Identifier.ValueText);
                }
            }
        }

        if (fieldDeclarations.Count < 2 || fieldNames.Count == 0)
        {
            return new Dictionary<string, ImmutableHashSet<string>>();
        }

        var fieldsWithCandidateInitializers = new List<FieldDeclarationSyntax>();
        foreach (var field in fieldDeclarations)
        {
            bool hasCandidate = false;
            foreach (var variable in field.Declaration.Variables)
            {
                if (variable.Initializer != null && HasCandidateIdentifier(variable.Initializer.Value, fieldNames))
                {
                    hasCandidate = true;
                    break;
                }
            }

            if (hasCandidate)
            {
                fieldsWithCandidateInitializers.Add(field);
            }
        }

        if (fieldsWithCandidateInitializers.Count == 0)
        {
            return new Dictionary<string, ImmutableHashSet<string>>();
        }

        var fieldSymbolMap = new Dictionary<ISymbol, string>();
        INamedTypeSymbol enclosingType = null;

        foreach (var field in fieldDeclarations)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                var fieldSymbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);
                if (fieldSymbol != null)
                {
                    fieldSymbolMap[fieldSymbol] = variable.Identifier.ValueText;
                    enclosingType ??= fieldSymbol.ContainingType;
                }
            }
        }

        if (enclosingType == null)
        {
            return new Dictionary<string, ImmutableHashSet<string>>();
        }

        var directDependencies = new Dictionary<string, HashSet<string>>();

        foreach (var field in fieldsWithCandidateInitializers)
        {
            foreach (var variable in field.Declaration.Variables)
            {
                if (variable.Initializer == null)
                {
                    continue;
                }

                HashSet<string> deps = null;
                var currentVarName = variable.Identifier.ValueText;

                foreach (var node in variable.Initializer.Value.DescendantNodesAndSelf())
                {
                    if (node is IdentifierNameSyntax identifierName
                        && fieldNames.Contains(identifierName.Identifier.ValueText)
                        && !IsInNameof(identifierName))
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(identifierName, cancellationToken);
                        var symbol = symbolInfo.Symbol;

                        if (symbol is IFieldSymbol fieldSymbol
                            && !fieldSymbol.IsConst
                            && fieldSymbol.IsStatic == field.Modifiers.Any(SyntaxKind.StaticKeyword)
                            && Equals(fieldSymbol.ContainingType?.OriginalDefinition, enclosingType.OriginalDefinition))
                        {
                            if (fieldSymbolMap.TryGetValue(fieldSymbol, out var targetVarName) && targetVarName != currentVarName)
                            {
                                deps ??= new HashSet<string>();
                                deps.Add(targetVarName);
                            }
                        }
                    }
                }

                if (deps != null)
                {
                    directDependencies[currentVarName] = deps;
                }
            }
        }

        if (directDependencies.Count == 0)
        {
            return new Dictionary<string, ImmutableHashSet<string>>();
        }

        var result = new Dictionary<string, ImmutableHashSet<string>>();

        foreach (var kvp in directDependencies)
        {
            var varName = kvp.Key;
            var transitiveDeps = new HashSet<string>();
            var queue = new Queue<string>(kvp.Value);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (transitiveDeps.Add(current))
                {
                    if (directDependencies.TryGetValue(current, out var nextDeps))
                    {
                        foreach (var next in nextDeps)
                        {
                            if (!transitiveDeps.Contains(next))
                            {
                                queue.Enqueue(next);
                            }
                        }
                    }
                }
            }

            result[varName] = transitiveDeps.ToImmutableHashSet();
        }

        return result;
    }

    internal static bool DependsOn(
        MemberDeclarationSyntax currentMember,
        MemberDeclarationSyntax targetMember,
        Dictionary<string, ImmutableHashSet<string>> fieldDependencies)
    {
        if (fieldDependencies == null || fieldDependencies.Count == 0)
        {
            return false;
        }

        if (!(currentMember is FieldDeclarationSyntax currentField) || !(targetMember is FieldDeclarationSyntax targetField))
        {
            return false;
        }

        foreach (var currentVar in currentField.Declaration.Variables)
        {
            if (fieldDependencies.TryGetValue(currentVar.Identifier.ValueText, out var deps))
            {
                foreach (var targetVar in targetField.Declaration.Variables)
                {
                    if (deps.Contains(targetVar.Identifier.ValueText))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool HasCandidateIdentifier(SyntaxNode expression, HashSet<string> fieldNames)
    {
        foreach (var node in expression.DescendantNodesAndSelf())
        {
            if (node is IdentifierNameSyntax id
                && fieldNames.Contains(id.Identifier.ValueText)
                && !IsInNameof(id))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInNameof(SyntaxNode node)
    {
        for (SyntaxNode current = node.Parent; current != null; current = current.Parent)
        {
            if (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is IdentifierNameSyntax nameSyntax
                    && nameSyntax.Identifier.ValueText == "nameof")
                {
                    return true;
                }

                break;
            }

            if (current is ExpressionSyntax || current is ArgumentSyntax || current is ArgumentListSyntax)
            {
                continue;
            }

            break;
        }

        return false;
    }
}
