// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Helpers;
using StyleCop.Analyzers.Lightup;

/// <summary>
/// The configured relative order of contiguous member-kind groups.
/// </summary>
internal sealed class MemberKindOrder
{
    private readonly ImmutableArray<SyntaxKind> kinds;
    private readonly ImmutableArray<OrderingTrait> precedingTraits;
    private readonly ImmutableArray<OrderingTrait> followingTraits;
    private readonly ImmutableArray<OrderingTrait> elementOrder;
    private readonly ImmutableArray<AccessLevel> accessibilityOrder;

    internal MemberKindOrder(ImmutableArray<string> names, ImmutableArray<OrderingTrait> elementOrder, ImmutableArray<AccessLevel> accessibilityOrder)
    {
        var builder = ImmutableArray.CreateBuilder<SyntaxKind>();
        foreach (var name in names)
        {
            var kind = name.ToLowerInvariant() switch
            {
                "field" => SyntaxKind.FieldDeclaration,
                "constructor" => SyntaxKind.ConstructorDeclaration,
                "destructor" => SyntaxKind.DestructorDeclaration,
                "delegate" => SyntaxKind.DelegateDeclaration,
                "event" => SyntaxKind.EventDeclaration,
                "enum" => SyntaxKind.EnumDeclaration,
                "interface" => SyntaxKind.InterfaceDeclaration,
                "extension" => SyntaxKindEx.ExtensionBlockDeclaration,
                "property" => SyntaxKind.PropertyDeclaration,
                "indexer" => SyntaxKind.IndexerDeclaration,
                "conversionoperator" => SyntaxKind.ConversionOperatorDeclaration,
                "operator" => SyntaxKind.OperatorDeclaration,
                "method" => SyntaxKind.MethodDeclaration,
                "struct" => SyntaxKind.StructDeclaration,
                "class" => SyntaxKind.ClassDeclaration,
                _ => throw new InvalidSettingsException($"memberKindOrder cannot contain kind '{name}'"),
            };

            if (builder.Contains(kind))
            {
                throw new InvalidSettingsException($"memberKindOrder cannot contain duplicate kind '{name}'");
            }

            builder.Add(kind);
        }

        this.kinds = builder.ToImmutable();
        this.elementOrder = elementOrder;
        this.accessibilityOrder = accessibilityOrder;
        var kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
        this.precedingTraits = kindIndex < 0 ? elementOrder : elementOrder.RemoveRange(kindIndex, elementOrder.Length - kindIndex);
        this.followingTraits = kindIndex < 0 ? ImmutableArray<OrderingTrait>.Empty : elementOrder.RemoveRange(0, kindIndex + 1);
    }

    internal static ImmutableArray<SyntaxKind> DefaultTypeOrder { get; } = ImmutableArray.Create(
        SyntaxKind.FieldDeclaration,
        SyntaxKind.ConstructorDeclaration,
        SyntaxKind.DestructorDeclaration,
        SyntaxKind.DelegateDeclaration,
        SyntaxKind.EventDeclaration,
        SyntaxKind.EnumDeclaration,
        SyntaxKind.InterfaceDeclaration,
        SyntaxKindEx.ExtensionBlockDeclaration,
        SyntaxKind.PropertyDeclaration,
        SyntaxKind.IndexerDeclaration,
        SyntaxKind.ConversionOperatorDeclaration,
        SyntaxKind.OperatorDeclaration,
        SyntaxKind.MethodDeclaration,
        SyntaxKind.StructDeclaration,
        SyntaxKind.ClassDeclaration);

    internal static SyntaxKind Normalize(SyntaxKind kind) => kind switch
    {
        SyntaxKindEx.RecordDeclaration => SyntaxKind.ClassDeclaration,
        SyntaxKindEx.RecordStructDeclaration => SyntaxKind.StructDeclaration,
        SyntaxKindEx.UnionDeclaration => SyntaxKind.StructDeclaration,
        _ => NormalizeEventKind(kind),
    };

    internal static SyntaxKind NormalizeEventKind(SyntaxKind kind)
        => kind == SyntaxKind.EventFieldDeclaration ? SyntaxKind.EventDeclaration : kind;

    internal static string GetKindName(SyntaxKind kind) => Normalize(kind) switch
    {
        SyntaxKind.FieldDeclaration => "field",
        SyntaxKind.ConstructorDeclaration => "constructor",
        SyntaxKind.DestructorDeclaration => "destructor",
        SyntaxKind.DelegateDeclaration => "delegate",
        SyntaxKind.EventDeclaration => "event",
        SyntaxKind.EnumDeclaration => "enum",
        SyntaxKind.InterfaceDeclaration => "interface",
        SyntaxKindEx.ExtensionBlockDeclaration => "extension",
        SyntaxKind.PropertyDeclaration => "property",
        SyntaxKind.IndexerDeclaration => "indexer",
        SyntaxKind.ConversionOperatorDeclaration => "conversionOperator",
        SyntaxKind.OperatorDeclaration => "operator",
        SyntaxKind.MethodDeclaration => "method",
        SyntaxKind.StructDeclaration => "struct",
        SyntaxKind.ClassDeclaration => "class",
        _ => kind.ToString(),
    };

    internal static int GetPriority(MemberDeclarationSyntax member, ImmutableArray<OrderingTrait> traits, ImmutableArray<AccessLevel> accessibilityOrder, OrderingTrait? stopAt = null)
    {
        var modifiers = member.GetModifiers();
        var isConstant = modifiers.Any(SyntaxKind.ConstKeyword);
        var priority = 0;
        foreach (var trait in traits)
        {
            if (trait == stopAt)
            {
                break;
            }

            switch (trait)
            {
            case OrderingTrait.Accessibility:
                priority = (priority << 3) | MemberOrderHelper.GetAccessibilityPriority(accessibilityOrder, MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers));
                break;

            case OrderingTrait.Constant:
                priority = (priority << 1) | (isConstant ? 1 : 0);
                break;

            case OrderingTrait.Static:
                priority = (priority << 1) | (isConstant || modifiers.Any(SyntaxKind.StaticKeyword) ? 1 : 0);
                break;

            case OrderingTrait.Readonly:
                priority = (priority << 1) | (isConstant || modifiers.Any(SyntaxKind.ReadOnlyKeyword) ? 1 : 0);
                break;
            }
        }

        return priority;
    }

    internal static bool StartsNewGroup(MemberDeclarationSyntax member, int priority, int previousPriority)
        => priority != previousPriority || member.IsKind(SyntaxKind.IncompleteMember);

    internal static bool IsModifierOutOfOrder(int currentPriority, int previousPriority)
        => (currentPriority & 1) != 0 && previousPriority == currentPriority - 1;

    internal int GetModifierPriority(MemberDeclarationSyntax member, OrderingTrait trait, SyntaxKind modifier)
    {
        var modifiers = member.GetModifiers();
        var hasModifier = modifiers.Any(SyntaxKind.ConstKeyword) || modifiers.Any(modifier);

        // The low bit records the modifier. The remaining bits record the higher-priority traits.
        return (this.GetPriorityBefore(member, trait) << 1) | (hasModifier ? 1 : 0);
    }

    internal int GetRank(MemberDeclarationSyntax member) => this.kinds.IndexOf(Normalize(member.Kind()));

    internal int GetPrecedingPriority(MemberDeclarationSyntax member) => this.precedingTraits.IsEmpty ? 0 : GetPriority(member, this.precedingTraits, this.accessibilityOrder);

    internal int GetFollowingPriority(MemberDeclarationSyntax member) => this.followingTraits.IsEmpty ? 0 : GetPriority(member, this.followingTraits, this.accessibilityOrder);

    internal int GetPriorityBefore(MemberDeclarationSyntax member, OrderingTrait orderingTrait) => GetPriority(member, this.elementOrder, this.accessibilityOrder, orderingTrait);
}
