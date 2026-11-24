// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Lightup;
using StyleCop.Analyzers.Settings.ObjectModel;

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

    internal static int GetGroupPriority(MemberDeclarationSyntax member, OrderingSettings settings)
    {
        var elementOrder = settings.ElementOrder;
        var priority = MemberKindOrder.GetPriority(member, elementOrder, settings.AccessibilityOrder);
        var staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
        if (settings.MemberKindOrder == null && staticIndex >= 0 && staticIndex < elementOrder.IndexOf(OrderingTrait.Kind))
        {
            // The default kind analyzer partitions by the explicit static modifier before comparing kinds.
            // Preserve that boundary as well as the effective static group used by the other ordering rules.
            priority = (priority << 1) | (member.GetModifiers().Any(SyntaxKind.StaticKeyword) ? 1 : 0);
        }

        return priority;
    }

    internal static SyntaxKind GetGroupKind(MemberDeclarationSyntax member, OrderingSettings settings)
    {
        var kind = member.Kind();
        if (kind == SyntaxKindEx.RecordDeclaration && settings.MemberKindOrder == null)
        {
            var kindIndex = settings.ElementOrder.IndexOf(OrderingTrait.Kind);
            if (kindIndex >= 0 && kindIndex < settings.ElementOrder.IndexOf(OrderingTrait.Static))
            {
                // The default static rule distinguishes adjacent classes and records when kind precedes static.
                return kind;
            }
        }

        return MemberKindOrder.Normalize(kind);
    }

    internal static bool StartsOutOfOrderGroup(SyntaxList<MemberDeclarationSyntax> members, int index, OrderingSettings settings)
    {
        if (index == 0 || members[index - 1].IsKind(SyntaxKind.IncompleteMember))
        {
            return false;
        }

        var member = members[index];
        var previous = members[index - 1];
        var sameKind = MemberKindOrder.Normalize(member.Kind()) == MemberKindOrder.Normalize(previous.Kind());
        var elementOrder = settings.ElementOrder;
        var accessibilityOrder = settings.AccessibilityOrder;
        if (sameKind)
        {
            var priority = MemberKindOrder.GetPriority(member, elementOrder, accessibilityOrder);
            var previousPriority = MemberKindOrder.GetPriority(previous, elementOrder, accessibilityOrder);
            return priority > previousPriority || HasEarlierAccessibilityInversion(members, index, settings);
        }

        var kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
        var kindInversion = kindIndex >= 0
            && (settings.MemberKindOrder is { } order
                ? StartsConfiguredKindViolation(members, index, order)
                : MemberOrderTraits.IsDefaultKindOutOfOrder(member, previous, elementOrder, kindIndex, MemberKindOrder.DefaultTypeOrder));

        return kindInversion
            || HasStaticInversion(members, index, settings, kindIndex)
            || HasEarlierFieldInversion(members, index, settings, kindIndex)
            || HasEarlierAccessibilityInversion(members, index, settings);
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

        // An attribute can occur in another declaration of a partial type.
        var symbol = semanticModel?.GetDeclaredSymbol(type, cancellationToken);
        if (symbol == null)
        {
            return true;
        }

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is not { } attributeClass)
            {
                continue;
            }

            if (attributeClass.Name is "XmlRootAttribute" or "XmlTypeAttribute"
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Xml.Serialization")
            {
                // XmlSerializer can emit elements in declaration order when no order is specified.
                return true;
            }

            // LayoutKind.Auto has the value 3. The attribute also has a short-valued overload.
            if (attributeClass.Name == "StructLayoutAttribute"
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Runtime.InteropServices"
                && (attribute.ConstructorArguments.IsEmpty
                    || (attribute.ConstructorArguments[0].Value is not 3 && attribute.ConstructorArguments[0].Value is not (short)3)))
            {
                return true;
            }
        }

        return false;
    }

    internal static bool RequiresStableMemberOrder(TypeDeclarationSyntax type, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        for (var ancestor = type.Parent as TypeDeclarationSyntax; ancestor != null; ancestor = ancestor.Parent as TypeDeclarationSyntax)
        {
            if (IsGeneratedDeclaration(ancestor, semanticModel, cancellationToken))
            {
                return true;
            }
        }

        var isPartial = type.Modifiers.Any(SyntaxKind.PartialKeyword);

        // A different declaration can supply the public modifier or base type.
        var canBePublic = isPartial || type.Modifiers.Any(SyntaxKind.PublicKeyword) || type.Parent is InterfaceDeclarationSyntax;
        var canDeriveFromEventSource = type.IsKind(SyntaxKind.ClassDeclaration) && (isPartial || type.BaseList != null);
        var assemblyIsComVisible = canBePublic
            && semanticModel != null
            && IsComVisibleAssembly(semanticModel.Compilation.Assembly.GetAttributes());
        if (!assemblyIsComVisible && !canDeriveFromEventSource && type.AttributeLists.Count == 0 && !isPartial)
        {
            return false;
        }

        var symbol = semanticModel?.GetDeclaredSymbol(type, cancellationToken);
        if (symbol == null)
        {
            return true;
        }

        if (type.AttributeLists.Count > 0 && HasGeneratedCodeAttribute(type, symbol))
        {
            return true;
        }

        if (canDeriveFromEventSource && DerivesFromEventSource(symbol))
        {
            // EventSource assigns implicit event IDs in declaration order.
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

    internal static bool IsInHiddenRegion(SyntaxToken identifier, CancellationToken cancellationToken)
    {
        return identifier.SyntaxTree.GetLineVisibility(identifier.SpanStart, cancellationToken) == LineVisibility.Hidden;
    }

    internal static bool IsGeneratedDeclaration(MemberDeclarationSyntax member, SemanticModel? semanticModel, CancellationToken cancellationToken)
    {
        if (!member.GetFirstToken().IsKind(SyntaxKind.OpenBracketToken))
        {
            return false;
        }

        var declaration = member is BaseFieldDeclarationSyntax field && field.Declaration.Variables.Count == 1
            ? (SyntaxNode)field.Declaration.Variables[0]
            : member;
        var symbol = semanticModel?.GetDeclaredSymbol(declaration, cancellationToken);
        return symbol != null && HasGeneratedCodeAttribute(member, symbol);
    }

    internal static bool HasOrderSensitiveInitializers(MemberDeclarationSyntax first, MemberDeclarationSyntax second)
    {
        var firstHasInitializer = HasRuntimeInitializer(first);
        var secondHasInitializer = HasRuntimeInitializer(second);
        return (firstHasInitializer && (secondHasInitializer || IsPotentialDecimalConstant(second)))
            || (secondHasInitializer && IsPotentialDecimalConstant(first));
    }

    internal static bool CouldExposeFieldAccessibilityInversion(
        SyntaxList<MemberDeclarationSyntax> members,
        int index,
        int targetIndex,
        OrderingSettings settings,
        Dictionary<string, ImmutableHashSet<string>> fieldDependencies)
    {
        var elementOrder = settings.ElementOrder;
        var accessibilityIndex = elementOrder.IndexOf(OrderingTrait.Accessibility);
        if (accessibilityIndex < 0)
        {
            return false;
        }

        var current = (FieldDeclarationSyntax)members[index];
        var previousTraits = new MemberOrderTraits(members[targetIndex]);
        var priority = MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, previousTraits.AccessLevel);
        Dictionary<string, FieldDependencyHelper.FieldOrderingInfo>? originalOrder = null;
        Dictionary<string, FieldDependencyHelper.FieldOrderingInfo>? newOrder = null;
        for (var i = index + 1; i < members.Count; i++)
        {
            if (members[i] is not FieldDeclarationSyntax dependent || !FieldDependencyHelper.DependsOn(dependent, current, fieldDependencies))
            {
                continue;
            }

            var dependentTraits = new MemberOrderTraits(dependent);
            var dependentPriority = MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, dependentTraits.AccessLevel);
            if (dependentPriority <= priority || !dependentTraits.MatchesAccessibilityPrefix(previousTraits, elementOrder, accessibilityIndex))
            {
                continue;
            }

            // Evaluate the dependent in both orders. Another dependency or a nearer member may
            // still keep SA1202 from reporting an inversion after the move.
            newOrder ??= GetFieldOrderingInfo(members, settings.AccessibilityOrder, index, targetIndex);
            if (HasFieldAccessibilityInversion(i, targetIndex, newOrder))
            {
                originalOrder ??= GetFieldOrderingInfo(members, settings.AccessibilityOrder, index, index);
                if (!HasFieldAccessibilityInversion(i, index, originalOrder))
                {
                    return true;
                }
            }
        }

        return false;

        bool HasFieldAccessibilityInversion(int dependentIndex, int insertionIndex, Dictionary<string, FieldDependencyHelper.FieldOrderingInfo> positions)
        {
            var dependent = (FieldDeclarationSyntax)members[dependentIndex];
            var dependentTraits = new MemberOrderTraits(dependent);
            var dependentPriority = MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, dependentTraits.AccessLevel);
            var hasExcusedInversion = false;
            for (var i = dependentIndex - 1; i >= 0; i--)
            {
                var previous = members[GetOriginalMemberIndex(i, index, insertionIndex)];
                if (!MemberOrderTraits.SupportsAccessibilityOrdering(previous))
                {
                    continue;
                }

                var traits = new MemberOrderTraits(previous);
                if (!dependentTraits.MatchesAccessibilityPrefix(traits, elementOrder, accessibilityIndex))
                {
                    continue;
                }

                var previousPriority = MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, traits.AccessLevel);
                if (dependentPriority > previousPriority)
                {
                    if (previous is not FieldDeclarationSyntax previousField
                        || !FieldDependencyHelper.IsAccessibilityInversionExcused(dependent, previousField, previousPriority, i, fieldDependencies, positions))
                    {
                        return true;
                    }

                    hasExcusedInversion = true;
                }
                else if (!hasExcusedInversion)
                {
                    break;
                }
            }

            return false;
        }
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

    private static Dictionary<string, FieldDependencyHelper.FieldOrderingInfo> GetFieldOrderingInfo(
        SyntaxList<MemberDeclarationSyntax> members, ImmutableArray<AccessLevel> accessibilityOrder, int index, int insertionIndex)
    {
        var result = new Dictionary<string, FieldDependencyHelper.FieldOrderingInfo>();
        for (var i = 0; i < members.Count; i++)
        {
            if (members[GetOriginalMemberIndex(i, index, insertionIndex)] is FieldDeclarationSyntax field)
            {
                var accessLevel = MemberOrderHelper.GetAccessLevelForOrdering(field, field.Modifiers);
                var priority = MemberOrderHelper.GetAccessibilityPriority(accessibilityOrder, accessLevel);
                foreach (var variable in field.Declaration.Variables)
                {
                    result[variable.Identifier.ValueText] = new FieldDependencyHelper.FieldOrderingInfo(priority, i);
                }
            }
        }

        return result;
    }

    private static int GetOriginalMemberIndex(int position, int index, int insertionIndex)
    {
        if (position == insertionIndex)
        {
            return index;
        }

        return position > insertionIndex && position <= index ? position - 1 : position;
    }

    private static bool StartsConfiguredKindViolation(SyntaxList<MemberDeclarationSyntax> members, int index, MemberKindOrder order)
    {
        var member = members[index];
        var priority = order.GetPrecedingPriority(member);
        var rank = order.GetRank(member);
        if (rank < 0)
        {
            // An interrupted configured group reports the first unranked declaration.
            var previous = members[index - 1];
            var previousRank = order.GetRank(previous);
            return previousRank >= 0 && !MemberKindOrder.StartsNewGroup(previous, order.GetPrecedingPriority(previous), priority)
                && FindConfiguredRank(members, index + 1, 1, order, priority) == previousRank;
        }

        return FindConfiguredRank(members, index - 1, -1, order, priority) > rank;
    }

    private static int FindConfiguredRank(SyntaxList<MemberDeclarationSyntax> members, int start, int step, MemberKindOrder order, int priority)
    {
        for (var i = start; i >= 0 && i < members.Count; i += step)
        {
            var member = members[i];
            if (MemberKindOrder.StartsNewGroup(member, order.GetPrecedingPriority(member), priority))
            {
                break;
            }

            var rank = order.GetRank(member);
            if (rank >= 0)
            {
                return rank;
            }
        }

        return -1;
    }

    private static bool HasStaticInversion(SyntaxList<MemberDeclarationSyntax> members, int index, OrderingSettings settings, int kindIndex)
    {
        var elementOrder = settings.ElementOrder;
        var staticIndex = elementOrder.IndexOf(OrderingTrait.Static);
        if (staticIndex < 0)
        {
            return false;
        }

        if (kindIndex >= 0 && kindIndex < staticIndex && settings.MemberKindOrder is { } order)
        {
            return HasConfiguredTraitInversion(members, index, order, OrderingTrait.Static, SyntaxKind.StaticKeyword);
        }

        return new MemberOrderTraits(members[index])
            .IsModifierOutOfOrder(new MemberOrderTraits(members[index - 1]), elementOrder, staticIndex, OrderingTrait.Static);
    }

    private static bool HasEarlierFieldInversion(SyntaxList<MemberDeclarationSyntax> members, int index, OrderingSettings settings, int kindIndex)
    {
        if (members[index] is not FieldDeclarationSyntax field)
        {
            return false;
        }

        var traits = settings.ElementOrder;
        var constantIndex = traits.IndexOf(OrderingTrait.Constant);
        if (constantIndex >= 0 && field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            // Constant ordering compares consecutive fields even when another member lies between them.
            for (var i = index - 1; i >= 0; i--)
            {
                if (members[i] is FieldDeclarationSyntax previous)
                {
                    if (new MemberOrderTraits(field).IsModifierOutOfOrder(new MemberOrderTraits(previous), traits, constantIndex, OrderingTrait.Constant))
                    {
                        return true;
                    }

                    break;
                }
            }
        }

        return kindIndex >= 0 && kindIndex < traits.IndexOf(OrderingTrait.Readonly)
            && settings.MemberKindOrder is { } order
            && HasConfiguredTraitInversion(members, index, order, OrderingTrait.Readonly, SyntaxKind.ReadOnlyKeyword);
    }

    private static bool HasConfiguredTraitInversion(SyntaxList<MemberDeclarationSyntax> members, int index, MemberKindOrder order, OrderingTrait trait, SyntaxKind modifier)
    {
        var member = members[index];
        var modifierPriority = order.GetModifierPriority(member, trait, modifier);
        if ((modifierPriority & 1) == 0)
        {
            return false;
        }

        var kind = MemberKindOrder.Normalize(member.Kind());
        var precedingPriority = order.GetPrecedingPriority(member);
        for (var i = index - 1; i >= 0; i--)
        {
            var previous = members[i];
            if (MemberKindOrder.StartsNewGroup(previous, order.GetPrecedingPriority(previous), precedingPriority))
            {
                break;
            }

            if (MemberKindOrder.Normalize(previous.Kind()) == kind)
            {
                return MemberKindOrder.IsModifierOutOfOrder(modifierPriority, order.GetModifierPriority(previous, trait, modifier));
            }
        }

        return false;
    }

    private static bool HasEarlierAccessibilityInversion(SyntaxList<MemberDeclarationSyntax> members, int index, OrderingSettings settings)
    {
        var traits = settings.ElementOrder;
        var accessibilityIndex = traits.IndexOf(OrderingTrait.Accessibility);
        if (accessibilityIndex < 0 || !MemberOrderTraits.SupportsAccessibilityOrdering(members[index]))
        {
            return false;
        }

        var current = new MemberOrderTraits(members[index]);
        var priority = MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, current.AccessLevel);
        for (var i = index - 1; i >= 0; i--)
        {
            var previous = members[i];
            if (!MemberOrderTraits.SupportsAccessibilityOrdering(previous))
            {
                continue;
            }

            var previousTraits = new MemberOrderTraits(previous);
            if (current.MatchesAccessibilityPrefix(previousTraits, traits, accessibilityIndex))
            {
                return priority > MemberOrderHelper.GetAccessibilityPriority(settings.AccessibilityOrder, previousTraits.AccessLevel);
            }
        }

        return false;
    }

    private static bool HasGeneratedCodeAttribute(MemberDeclarationSyntax member, ISymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is { Name: "GeneratedCodeAttribute" } attributeClass
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.CodeDom.Compiler"
                && attribute.ApplicationSyntaxReference is { } syntax
                && syntax.SyntaxTree == member.SyntaxTree
                && member.Span.Contains(syntax.Span))
            {
                // A partial symbol may have an attribute that belongs to a different declaration.
                return true;
            }
        }

        return false;
    }

    private static bool IsPotentialDecimalConstant(MemberDeclarationSyntax member)
    {
        // Decimal constants emit a runtime assignment. A named constant type could be an alias for decimal.
        return member is FieldDeclarationSyntax field
            && field.Modifiers.Any(SyntaxKind.ConstKeyword)
            && (field.Declaration.Type is not PredefinedTypeSyntax predefined || predefined.Keyword.IsKind(SyntaxKind.DecimalKeyword));
    }

    private static bool IsComVisibleAssembly(ImmutableArray<AttributeData> attributes)
    {
        if (GetComVisibility(attributes) is bool visible)
        {
            return visible;
        }

        foreach (var attribute in attributes)
        {
            if (attribute.AttributeClass is { Name: "ClassInterfaceAttribute" } attributeClass
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.Runtime.InteropServices")
            {
                return true;
            }
        }

        return false;
    }

    private static bool DerivesFromEventSource(INamedTypeSymbol symbol)
    {
        for (var current = symbol.BaseType; current != null; current = current.BaseType)
        {
            if (current.Name == "EventSource" && current.ContainingNamespace.ToDisplayString() == "System.Diagnostics.Tracing")
            {
                return true;
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
