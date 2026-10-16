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

    internal MemberKindOrder(ImmutableArray<string> names, ImmutableArray<OrderingTrait> elementOrder)
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
        var kindIndex = elementOrder.IndexOf(OrderingTrait.Kind);
        this.precedingTraits = kindIndex < 0 ? elementOrder : elementOrder.RemoveRange(kindIndex, elementOrder.Length - kindIndex);
        this.followingTraits = kindIndex < 0 ? ImmutableArray<OrderingTrait>.Empty : elementOrder.RemoveRange(0, kindIndex + 1);
    }

    internal static SyntaxKind Normalize(SyntaxKind kind) => kind switch
    {
        SyntaxKind.EventFieldDeclaration => SyntaxKind.EventDeclaration,
        SyntaxKindEx.RecordDeclaration => SyntaxKind.ClassDeclaration,
        SyntaxKindEx.RecordStructDeclaration => SyntaxKind.StructDeclaration,
        SyntaxKindEx.UnionDeclaration => SyntaxKind.StructDeclaration,
        _ => kind,
    };

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

    internal int GetRank(MemberDeclarationSyntax member) => this.kinds.IndexOf(Normalize(member.Kind()));

    internal int GetPrecedingPriority(MemberDeclarationSyntax member) => this.precedingTraits.IsEmpty ? 0 : new MemberOrderHelper(member, this.precedingTraits).Priority;

    internal int GetFollowingPriority(MemberDeclarationSyntax member) => this.followingTraits.IsEmpty ? 0 : new MemberOrderHelper(member, this.followingTraits).Priority;
}
