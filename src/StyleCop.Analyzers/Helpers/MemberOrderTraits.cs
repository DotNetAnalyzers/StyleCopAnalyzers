// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StyleCop.Analyzers.Settings.ObjectModel;

/// <summary>
/// Compares the syntax traits used by the member ordering rules.
/// </summary>
internal readonly struct MemberOrderTraits
{
    private readonly SyntaxKind kind;
    private readonly bool isConstant;
    private readonly bool isStatic;
    private readonly bool isReadonly;

    internal MemberOrderTraits(MemberDeclarationSyntax member)
    {
        var modifiers = member.GetModifiers();
        this.kind = MemberKindOrder.NormalizeEventKind(member.Kind());
        this.AccessLevel = MemberOrderHelper.GetAccessLevelForOrdering(member, modifiers);
        this.isConstant = modifiers.Any(SyntaxKind.ConstKeyword);
        this.isStatic = modifiers.Any(SyntaxKind.StaticKeyword);
        this.isReadonly = modifiers.Any(SyntaxKind.ReadOnlyKeyword);
    }

    internal AccessLevel AccessLevel { get; }

    internal static bool IsDefaultKindOutOfOrder(
        MemberDeclarationSyntax current,
        MemberDeclarationSyntax previous,
        ImmutableArray<OrderingTrait> elementOrder,
        int kindIndex,
        ImmutableArray<SyntaxKind> kindOrder)
    {
        for (var i = 0; i < kindIndex; i++)
        {
            switch (elementOrder[i])
            {
            case OrderingTrait.Accessibility:
                if (MemberOrderHelper.GetAccessLevelForOrdering(current, current.GetModifiers())
                    != MemberOrderHelper.GetAccessLevelForOrdering(previous, previous.GetModifiers()))
                {
                    return false;
                }

                break;

            case OrderingTrait.Static:
                // Kind ordering only groups by the explicit static modifier.
                if (current.GetModifiers().Any(SyntaxKind.StaticKeyword) != previous.GetModifiers().Any(SyntaxKind.StaticKeyword))
                {
                    return false;
                }

                break;
            }
        }

        return kindOrder.IndexOf(MemberKindOrder.Normalize(current.Kind()))
            < kindOrder.IndexOf(MemberKindOrder.Normalize(previous.Kind()));
    }

    internal bool IsModifierOutOfOrder(MemberOrderTraits previous, ImmutableArray<OrderingTrait> elementOrder, int traitIndex, OrderingTrait trait)
        => this.HasModifier(trait, includeImplicit: true)
            && !previous.HasModifier(trait, includeImplicit: true)
            && this.MatchesPrefix(previous, elementOrder, traitIndex, includeImplicit: true);

    internal bool MatchesAccessibilityPrefix(MemberOrderTraits previous, ImmutableArray<OrderingTrait> elementOrder, int accessibilityIndex)
        => this.MatchesPrefix(previous, elementOrder, accessibilityIndex, includeImplicit: false);

    private bool MatchesPrefix(MemberOrderTraits previous, ImmutableArray<OrderingTrait> elementOrder, int index, bool includeImplicit)
    {
        for (var i = 0; i < index; i++)
        {
            var trait = elementOrder[i];
            var matches = trait switch
            {
                OrderingTrait.Kind => this.kind == previous.kind,
                OrderingTrait.Accessibility => this.AccessLevel == previous.AccessLevel,
                OrderingTrait.Constant or OrderingTrait.Static or OrderingTrait.Readonly
                    => this.HasModifier(trait, includeImplicit) == previous.HasModifier(trait, includeImplicit),
                _ => true,
            };
            if (!matches)
            {
                return false;
            }
        }

        return true;
    }

    private bool HasModifier(OrderingTrait trait, bool includeImplicit)
        => trait switch
        {
            OrderingTrait.Constant => this.isConstant,
            OrderingTrait.Static => this.isStatic || (includeImplicit && this.isConstant),
            OrderingTrait.Readonly => this.isReadonly || (includeImplicit && this.isConstant),
            _ => false,
        };
}
