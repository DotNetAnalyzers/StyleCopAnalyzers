// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class CrefParameterSyntaxExtensions
    {
        private static readonly Func<CrefParameterSyntax, SyntaxToken> RefKindKeywordAccessor;
        private static readonly Func<CrefParameterSyntax, SyntaxToken, CrefParameterSyntax> WithRefKindKeywordAccessor;

        static CrefParameterSyntaxExtensions()
        {
            RefKindKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CrefParameterSyntax, SyntaxToken>(typeof(CrefParameterSyntax), nameof(RefKindKeyword));
            WithRefKindKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CrefParameterSyntax, SyntaxToken>(typeof(CrefParameterSyntax), nameof(RefKindKeyword));
        }

        public static SyntaxToken RefKindKeyword(this CrefParameterSyntax syntax)
        {
            return RefKindKeywordAccessor(syntax);
        }

        public static CrefParameterSyntax WithRefKindKeyword(this CrefParameterSyntax syntax, SyntaxToken refKindKeyword)
        {
            return WithRefKindKeywordAccessor(syntax, refKindKeyword);
        }
    }
}
