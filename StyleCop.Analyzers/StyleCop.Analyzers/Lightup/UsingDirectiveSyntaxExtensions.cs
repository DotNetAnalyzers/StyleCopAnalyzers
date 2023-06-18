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
        private static readonly Func<UsingDirectiveSyntax, SyntaxToken, UsingDirectiveSyntax> WithGlobalKeywordAccessor;

        static UsingDirectiveSyntaxExtensions()
        {
            GlobalKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(GlobalKeyword));
            WithGlobalKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<UsingDirectiveSyntax, SyntaxToken>(typeof(UsingDirectiveSyntax), nameof(GlobalKeyword));
        }

        public static SyntaxToken GlobalKeyword(this UsingDirectiveSyntax syntax)
        {
            return GlobalKeywordAccessor(syntax);
        }

        public static UsingDirectiveSyntax WithGlobalKeyword(this UsingDirectiveSyntax syntax, SyntaxToken globalKeyword)
        {
            return WithGlobalKeywordAccessor(syntax, globalKeyword);
        }
    }
}
