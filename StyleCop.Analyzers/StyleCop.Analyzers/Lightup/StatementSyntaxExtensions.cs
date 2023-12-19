// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class StatementSyntaxExtensions
    {
        private static readonly Func<StatementSyntax, SyntaxList<AttributeListSyntax>> AttributeListsAccessor;
        private static readonly Func<StatementSyntax, SyntaxList<AttributeListSyntax>, StatementSyntax> WithAttributeListsAccessor;

        static StatementSyntaxExtensions()
        {
            AttributeListsAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxList<AttributeListSyntax>>(typeof(StatementSyntax), nameof(AttributeLists));
            WithAttributeListsAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxList<AttributeListSyntax>>(typeof(StatementSyntax), nameof(AttributeLists));
        }

        public static SyntaxList<AttributeListSyntax> AttributeLists(this StatementSyntax syntax)
        {
            return AttributeListsAccessor(syntax);
        }

        public static StatementSyntax WithAttributeLists(this StatementSyntax syntax, SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeListsAccessor(syntax, attributeLists);
        }

        public static StatementSyntax AddAttributeLists(this StatementSyntax syntax, params AttributeListSyntax[] items)
        {
            return syntax.WithAttributeLists(syntax.AttributeLists().AddRange(items));
        }
    }
}
