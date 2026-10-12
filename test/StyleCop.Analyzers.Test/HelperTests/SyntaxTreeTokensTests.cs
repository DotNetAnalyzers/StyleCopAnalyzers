// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;
    using Xunit;

    public class SyntaxTreeTokensTests
    {
        private const string TestCode = @"#define DEBUG
using System;

/// <summary>
/// The <see cref=""Foo""/> class.
/// </summary>
public class Foo
{
    #region Members
#if DEBUG
    private int i = (1);
#else
    private int j = (2);
#endif
    #pragma warning disable CS0169 // comment
    private int k; ) ]
    #endregion

    /// <param name=""x"">The value.</param>
    public void M(int x)
    {
        var s = ""text"" + 'c'; // trailing
    }
}
#if DEBUG
#endif
";

        [Fact]
        public void TestTokensMatchDescendantTokens()
        {
            var root = CSharpSyntaxTree.ParseText(TestCode).GetRoot();
            var tokens = new SyntaxTreeTokens(root.SyntaxTree);

            Assert.Equal(root.DescendantTokens(), tokens.GetTokens(CancellationToken.None));
        }

        [Fact]
        public void TestTokensIncludingStructuredTriviaMatchDescendantTokens()
        {
            var root = CSharpSyntaxTree.ParseText(TestCode).GetRoot();
            var tokens = new SyntaxTreeTokens(root.SyntaxTree);
            var expected = root.DescendantTokens(descendIntoTrivia: true).ToArray();

            Assert.Contains(expected, token => token.Parent.IsPartOfStructuredTrivia());
            Assert.Equal(expected, tokens.GetTokensIncludingStructuredTrivia(CancellationToken.None));
        }
    }
}
