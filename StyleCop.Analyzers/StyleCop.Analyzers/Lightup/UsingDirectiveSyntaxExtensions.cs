// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class UsingDirectiveSyntaxExtensions
    {
        private static readonly Func<UsingDirectiveSyntax, SyntaxToken> GlobalKeywordAccessor;
        private static readonly Func<UsingDirectiveSyntax, SyntaxToken> UnsafeKeywordAccessor;
        private static readonly Func<UsingDirectiveSyntax, TypeSyntax> NamespaceOrTypeAccessor;
        private static readonly Func<UsingDirectiveSyntax, SyntaxToken, UsingDirectiveSyntax> WithGlobalKeywordAccessor;
        private static readonly Func<UsingDirectiveSyntax, SyntaxToken, UsingDirectiveSyntax> WithUnsafeKeywordAccessor;
        private static readonly Func<UsingDirectiveSyntax, TypeSyntax, UsingDirectiveSyntax> WithNamespaceOrTypeAccessor;

        static UsingDirectiveSyntaxExtensions()
        {
            GlobalKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(GlobalKeyword));
            UnsafeKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(UnsafeKeyword));
            NamespaceOrTypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<UsingDirectiveSyntax, TypeSyntax>(typeof(UsingDirectiveSyntax), nameof(NamespaceOrType));
            WithGlobalKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(GlobalKeyword));
            WithUnsafeKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(UnsafeKeyword));
            WithNamespaceOrTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<UsingDirectiveSyntax, TypeSyntax>(typeof(UsingDirectiveSyntax), nameof(NamespaceOrType));
        }

        public static SyntaxToken GlobalKeyword(this UsingDirectiveSyntax syntax)
        {
            return GlobalKeywordAccessor(syntax);
        }

        public static SyntaxToken UnsafeKeyword(this UsingDirectiveSyntax syntax)
        {
            return UnsafeKeywordAccessor(syntax);
        }

        public static TypeSyntax NamespaceOrType(this UsingDirectiveSyntax syntax)
        {
            return NamespaceOrTypeAccessor(syntax);
        }

        public static UsingDirectiveSyntax WithGlobalKeyword(this UsingDirectiveSyntax syntax, SyntaxToken globalKeyword)
        {
            return WithGlobalKeywordAccessor(syntax, globalKeyword);
        }

        public static UsingDirectiveSyntax WithUnsafeKeyword(this UsingDirectiveSyntax syntax, SyntaxToken unsafeKeyword)
        {
            return WithUnsafeKeywordAccessor(syntax, unsafeKeyword);
        }

        public static UsingDirectiveSyntax WithNamespaceOrType(this UsingDirectiveSyntax syntax, TypeSyntax namespaceOrType)
        {
            return WithNamespaceOrTypeAccessor(syntax, namespaceOrType);
        }
    }
}
