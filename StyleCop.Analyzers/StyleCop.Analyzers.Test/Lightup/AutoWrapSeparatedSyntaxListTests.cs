// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    public class AutoWrapSeparatedSyntaxListTests : SeparatedSyntaxListWrapperTestBase
    {
        internal override SeparatedSyntaxListWrapper<SyntaxNode> CreateList()
        {
            return new SeparatedSyntaxListWrapper<SyntaxNode>.AutoWrapSeparatedSyntaxList<SyntaxNode>(default);
        }

        internal override bool TryCreateNonEmptyList(out SeparatedSyntaxListWrapper<SyntaxNode> list)
        {
            list = new SeparatedSyntaxListWrapper<SyntaxNode>.AutoWrapSeparatedSyntaxList<SyntaxNode>(
                SyntaxFactory.SingletonSeparatedList<SyntaxNode>(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
            return true;
        }
    }
}
