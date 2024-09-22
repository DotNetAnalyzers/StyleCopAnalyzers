// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.HelperTests
{
    using Microsoft.CodeAnalysis;
    using StyleCop.Analyzers.Helpers;
    using Xunit;

    public class SyntaxTreeHelpersTests
    {
        [Fact]
        public void TestContainsUsingAliasArgumentIsNull()
        {
            SyntaxTree tree = null;
            Assert.False(tree.ContainsUsingAlias(null));
        }
    }
}
