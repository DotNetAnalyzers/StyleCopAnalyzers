// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class UsingStatementSyntaxExtensions
    {
        private static readonly Func<UsingStatementSyntax, SyntaxToken> AwaitKeywordAccessor;
        private static readonly Func<UsingStatementSyntax, SyntaxToken, UsingStatementSyntax> WithAwaitKeywordAccessor;

        static UsingStatementSyntaxExtensions()
        {
            AwaitKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<UsingStatementSyntax, SyntaxToken>(typeof(UsingStatementSyntax), nameof(AwaitKeyword));
            WithAwaitKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<UsingStatementSyntax, SyntaxToken>(typeof(UsingStatementSyntax), nameof(AwaitKeyword));
        }

        public static SyntaxToken AwaitKeyword(this UsingStatementSyntax syntax)
        {
            return AwaitKeywordAccessor(syntax);
        }

        public static UsingStatementSyntax WithAwaitKeyword(this UsingStatementSyntax syntax, SyntaxToken awaitKeyword)
        {
            return WithAwaitKeywordAccessor(syntax, awaitKeyword);
        }
    }
}
