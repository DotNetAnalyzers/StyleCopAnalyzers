// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers;

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// Identifies named members and guards their relative order when moving them would change semantics.
/// </summary>
internal static class MemberNameOrder
{
    internal static SyntaxToken GetSortableIdentifier(MemberDeclarationSyntax member)
    {
        if (member.ContainsDirectives)
        {
            return default;
        }

        var identifier = member switch
        {
            MethodDeclarationSyntax method => method.Identifier,
            PropertyDeclarationSyntax property => property.Identifier,
            EventDeclarationSyntax eventDeclaration => eventDeclaration.Identifier,
            BaseFieldDeclarationSyntax field when field.Declaration.Variables.Count == 1 => field.Declaration.Variables[0].Identifier,
            BaseTypeDeclarationSyntax type => type.Identifier,
            DelegateDeclarationSyntax delegateDeclaration => delegateDeclaration.Identifier,
            _ => default,
        };

        return identifier.IsMissing ? default : identifier;
    }

    internal static bool RequiresAutoLayout(MemberDeclarationSyntax member)
    {
        // In addition to fields, properties and field-like events may create backing fields.
        return (member is PropertyDeclarationSyntax
                || (member is BaseFieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword)))
            && !member.GetModifiers().Any(SyntaxKind.StaticKeyword);
    }

    internal static bool RequiresStableInstanceOrder(TypeDeclarationSyntax type, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        if (type.Kind() is SyntaxKind.StructDeclaration or SyntaxKindEx.RecordDeclaration or SyntaxKindEx.RecordStructDeclaration or SyntaxKindEx.UnionDeclaration)
        {
            return true;
        }

        if (type.AttributeLists.Count == 0 && !type.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        // A layout attribute can occur in another declaration of a partial type.
        var symbol = semanticModel?.GetDeclaredSymbol(type, cancellationToken);
        if (symbol == null)
        {
            return true;
        }

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is { Name: "StructLayoutAttribute" } attributeClass
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Runtime.InteropServices")
            {
                // LayoutKind.Auto has the value 3. The attribute also has a short-valued overload.
                return attribute.ConstructorArguments.Length == 0
                    || (attribute.ConstructorArguments[0].Value is not 3 && attribute.ConstructorArguments[0].Value is not (short)3);
            }
        }

        return false;
    }

    internal static bool IsComType(TypeDeclarationSyntax type, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        var isPartial = type.Modifiers.Any(SyntaxKind.PartialKeyword);

        // A different declaration of this type can supply its public modifier.
        var canBePublic = isPartial || type.Modifiers.Any(SyntaxKind.PublicKeyword) || type.Parent is InterfaceDeclarationSyntax;
        var assemblyIsComVisible = canBePublic
            && semanticModel != null
            && GetComVisibility(semanticModel.Compilation.Assembly.GetAttributes()) == true;
        if (!assemblyIsComVisible && type.AttributeLists.Count == 0 && !isPartial)
        {
            return false;
        }

        var symbol = semanticModel?.GetDeclaredSymbol(type, cancellationToken);
        if (symbol == null)
        {
            return true;
        }

        var attributes = symbol.GetAttributes();
        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass is not { } attributeClass)
            {
                continue;
            }

            var attributeNamespace = attributeClass.ContainingNamespace.ToDisplayString();
            if (attributeNamespace == "System.Runtime.InteropServices.Marshalling"
                && attributeClass.Name is "GeneratedComInterfaceAttribute" or "GeneratedComClassAttribute")
            {
                return true;
            }

            if (attributeNamespace == "System.Runtime.InteropServices"
                && (attributeClass.Name is "ComImportAttribute" or "InterfaceTypeAttribute" or "ClassInterfaceAttribute" or "ComDefaultInterfaceAttribute" or "ComSourceInterfacesAttribute"
                    || (attributeClass.Name == "GuidAttribute" && type.IsKind(SyntaxKind.InterfaceDeclaration))))
            {
                return true;
            }
        }

        var visibility = GetComVisibility(attributes);
        return visibility ?? (assemblyIsComVisible && IsPubliclyVisible(symbol));
    }

    internal static bool ContainsModuleInitializer(MemberDeclarationSyntax member, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        if (member is MethodDeclarationSyntax method)
        {
            return IsModuleInitializer(method, semanticModel, cancellationToken);
        }

        if (member is TypeDeclarationSyntax type)
        {
            // Moving a nested type can also move its module initializers in the compiler's emitted order.
            foreach (var node in type.DescendantNodes(static node => node is TypeDeclarationSyntax))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (node is MethodDeclarationSyntax nestedMethod && IsModuleInitializer(nestedMethod, semanticModel, cancellationToken))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static bool HasRuntimeInitializer(MemberDeclarationSyntax member)
    {
        if (member is PropertyDeclarationSyntax property)
        {
            return property.Initializer != null;
        }

        if (member is BaseFieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            foreach (var variable in field.Declaration.Variables)
            {
                if (variable.Initializer != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool? GetComVisibility(ImmutableArray<AttributeData> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass is { Name: "ComVisibleAttribute" } attributeClass
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Runtime.InteropServices"
                && attribute.ConstructorArguments.Length == 1
                && attribute.ConstructorArguments[0].Value is bool visible)
            {
                return visible;
            }
        }

        return null;
    }

    private static bool IsPubliclyVisible(INamedTypeSymbol type)
    {
        for (var current = type; current != null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsModuleInitializer(MethodDeclarationSyntax method, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        if (method.ParameterList.Parameters.Count != 0
            || !method.Modifiers.Any(SyntaxKind.StaticKeyword)
            || (method.AttributeLists.Count == 0 && !method.Modifiers.Any(SyntaxKind.PartialKeyword)))
        {
            return false;
        }

        var symbol = semanticModel?.GetDeclaredSymbol(method, cancellationToken);
        if (symbol == null)
        {
            return true;
        }

        return HasModuleInitializerAttribute(symbol)
            || (symbol.PartialImplementationPart is { } implementation && HasModuleInitializerAttribute(implementation))
            || (symbol.PartialDefinitionPart is { } definition && HasModuleInitializerAttribute(definition));
    }

    private static bool HasModuleInitializerAttribute(IMethodSymbol method)
    {
        foreach (var attribute in method.GetAttributes())
        {
            if (attribute.AttributeClass is { Name: "ModuleInitializerAttribute" } attributeClass
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Runtime.CompilerServices")
            {
                return true;
            }
        }

        return false;
    }
}
