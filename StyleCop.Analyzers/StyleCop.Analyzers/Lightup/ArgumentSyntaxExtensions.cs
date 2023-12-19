// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ArgumentSyntaxExtensions
    {
        private static readonly Func<ArgumentSyntax, SyntaxToken> RefKindKeywordAccessor;
        private static readonly Func<ArgumentSyntax, SyntaxToken, ArgumentSyntax> WithRefKindKeywordAccessor;

        static ArgumentSyntaxExtensions()
        {
            RefKindKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ArgumentSyntax, SyntaxToken>(typeof(ArgumentSyntax), nameof(RefKindKeyword));
            WithRefKindKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ArgumentSyntax, SyntaxToken>(typeof(ArgumentSyntax), nameof(RefKindKeyword));
        }

        public static SyntaxToken RefKindKeyword(this ArgumentSyntax syntax)
        {
            return RefKindKeywordAccessor(syntax);
        }

        public static ArgumentSyntax WithRefKindKeyword(this ArgumentSyntax syntax, SyntaxToken refKindKeyword)
        {
            return WithRefKindKeywordAccessor(syntax, refKindKeyword);
        }
    }
}
