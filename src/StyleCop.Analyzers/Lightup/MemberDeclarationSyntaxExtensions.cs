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
            AttributeListsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>>(typeof(MemberDeclarationSyntax), nameof(AttributeLists));
            ModifiersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<MemberDeclarationSyntax, SyntaxTokenList>(typeof(MemberDeclarationSyntax), nameof(Modifiers));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxList<AttributeListSyntax>>(typeof(MemberDeclarationSyntax), nameof(AttributeLists));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<MemberDeclarationSyntax, SyntaxTokenList>(typeof(MemberDeclarationSyntax), nameof(Modifiers));
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
    }
}
