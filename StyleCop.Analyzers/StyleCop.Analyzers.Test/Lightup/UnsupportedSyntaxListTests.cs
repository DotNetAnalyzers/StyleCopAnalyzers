// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Lightup;

    public class UnsupportedSyntaxListTests : SeparatedSyntaxListWrapperTestBase
    {
        internal override SeparatedSyntaxListWrapper<SyntaxNode> CreateList()
        {
            return SeparatedSyntaxListWrapper<SyntaxNode>.UnsupportedEmpty;
        }

        internal override bool TryCreateNonEmptyList(out SeparatedSyntaxListWrapper<SyntaxNode> list)
        {
            list = null;
            return false;
        }
    }
}
