// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ForEachStatementSyntaxExtensions
    {
        private static readonly Func<ForEachStatementSyntax, SyntaxToken> AwaitKeywordAccessor;
        private static readonly Func<ForEachStatementSyntax, SyntaxToken, ForEachStatementSyntax> WithAwaitKeywordAccessor;

        static ForEachStatementSyntaxExtensions()
        {
            AwaitKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ForEachStatementSyntax, SyntaxToken>(typeof(ForEachStatementSyntax), nameof(AwaitKeyword));
            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ForEachStatementSyntax, SyntaxToken>(typeof(ForEachStatementSyntax), nameof(AwaitKeyword));
        }

        public static SyntaxToken AwaitKeyword(this ForEachStatementSyntax syntax)
        {
            return AwaitKeywordAccessor(syntax);
        }

        public static ForEachStatementSyntax WithAwaitKeyword(this ForEachStatementSyntax syntax, SyntaxToken awaitKeyword)
        {
            return WithAwaitKeywordAccessor(syntax, awaitKeyword);
        }
    }
}
