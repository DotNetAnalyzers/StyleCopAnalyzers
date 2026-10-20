// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class MemberDeclarationSyntaxExtensions
    {
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>> AttributeListsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxTokenList> ModifiersAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>, MemberDeclarationSyntax> WithAttributeListsAccessor;
        private static readonly Func<MemberDeclarationSyntax, SyntaxTokenList, MemberDeclarationSyntax> WithModifiersAccessor;

        static MemberDeclarationSyntaxExtensions()
        {
            AttributeListsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>>(typeof(MemberDeclarationSyntax), nameof(AttributeLists), AttributeListsFallback);
            ModifiersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxTokenList>(typeof(MemberDeclarationSyntax), nameof(Modifiers), ModifiersFallback);
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>>(typeof(MemberDeclarationSyntax), nameof(AttributeLists), WithAttributeListsFallback);
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxTokenList>(typeof(MemberDeclarationSyntax), nameof(Modifiers), WithModifiersFallback);
        }

        public static SyntaxList<AttributeListSyntax> AttributeLists(this MemberDeclarationSyntax syntax)
        {
            return AttributeListsAccessor(syntax);
        }

        public static SyntaxTokenList Modifiers(this MemberDeclarationSyntax syntax)
        {
            return ModifiersAccessor(syntax);
        }

        public static MemberDeclarationSyntax WithAttributeLists(this MemberDeclarationSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsAccessor(syntax, attributeLists);
        }

        public static MemberDeclarationSyntax WithModifiers(this MemberDeclarationSyntax syntax, SyntaxTokenList modifiers)
        {
            return WithModifiersAccessor(syntax, modifiers);
        }

        public static MemberDeclarationSyntax AddAttributeLists(this MemberDeclarationSyntax syntax, params AttributeListSyntax[] items)
        {
            return syntax.WithAttributeLists(syntax.AttributeLists().AddRange(items));
        }

        public static MemberDeclarationSyntax AddModifiers(this MemberDeclarationSyntax syntax, params SyntaxToken[] items)
        {
            return syntax.WithModifiers(syntax.Modifiers().AddRange(items));
        }

        private static SyntaxList<AttributeListSyntax> AttributeListsFallback(MemberDeclarationSyntax syntax)
        {
            return syntax switch
            {
                BaseMethodDeclarationSyntax declaration => declaration.AttributeLists,
                BasePropertyDeclarationSyntax declaration => declaration.AttributeLists,
                BaseTypeDeclarationSyntax declaration => declaration.AttributeLists,
                BaseFieldDeclarationSyntax declaration => declaration.AttributeLists,
                DelegateDeclarationSyntax declaration => declaration.AttributeLists,
                EnumMemberDeclarationSyntax declaration => declaration.AttributeLists,
                IncompleteMemberSyntax declaration => declaration.AttributeLists,
                null => throw new NullReferenceException(),
                _ => default,
            };
        }

        private static MemberDeclarationSyntax WithAttributeListsFallback(MemberDeclarationSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists)
        {
            return syntax switch
            {
                MethodDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                ConstructorDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                DestructorDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                OperatorDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                ConversionOperatorDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                PropertyDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                EventDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                IndexerDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                ClassDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                StructDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                InterfaceDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                EnumDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                DelegateDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                EnumMemberDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                FieldDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                EventFieldDeclarationSyntax declaration => declaration.WithAttributeLists(attributeLists),
                IncompleteMemberSyntax declaration => declaration.WithAttributeLists(attributeLists),
                null => throw new NullReferenceException(),
                _ when attributeLists.Count == 0 => syntax,
                _ => throw new NotSupportedException(),
            };
        }

        private static SyntaxTokenList ModifiersFallback(MemberDeclarationSyntax syntax)
        {
            return syntax switch
            {
                BaseMethodDeclarationSyntax declaration => declaration.Modifiers,
                BasePropertyDeclarationSyntax declaration => declaration.Modifiers,
                BaseTypeDeclarationSyntax declaration => declaration.Modifiers,
                BaseFieldDeclarationSyntax declaration => declaration.Modifiers,
                DelegateDeclarationSyntax declaration => declaration.Modifiers,
                IncompleteMemberSyntax declaration => declaration.Modifiers,
                null => throw new NullReferenceException(),
                _ => default,
            };
        }

        private static MemberDeclarationSyntax WithModifiersFallback(MemberDeclarationSyntax syntax, SyntaxTokenList modifiers)
        {
            return syntax switch
            {
                MethodDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                ConstructorDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                DestructorDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                OperatorDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                ConversionOperatorDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                PropertyDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                EventDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                IndexerDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                ClassDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                StructDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                InterfaceDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                EnumDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                DelegateDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                FieldDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                EventFieldDeclarationSyntax declaration => declaration.WithModifiers(modifiers),
                IncompleteMemberSyntax declaration => declaration.WithModifiers(modifiers),
                null => throw new NullReferenceException(),
                _ when modifiers.Count == 0 => syntax,
                _ => throw new NotSupportedException(),
            };
        }
    }
}
