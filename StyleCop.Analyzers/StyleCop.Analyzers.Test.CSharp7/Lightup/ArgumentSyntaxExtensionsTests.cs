// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class ArgumentSyntaxExtensionsTests
    {
        [Fact]
        public void TestRefKindKeyword()
        {
            var argumentSyntax = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))
                .WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.InKeyword));
            Assert.Equal(argumentSyntax.RefKindKeyword, ArgumentSyntaxExtensions.RefKindKeyword(argumentSyntax));
            Assert.Equal(argumentSyntax.RefOrOutKeyword, ArgumentSyntaxExtensions.RefKindKeyword(argumentSyntax));
        }

        [Fact]
        public void TestWithRefKindKeyword()
        {
            var argumentSyntax = SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

            var refKindKeyword = SyntaxFactory.Token(SyntaxKind.InKeyword);
            Assert.Equal(default, ArgumentSyntaxExtensions.RefKindKeyword(argumentSyntax));
            var argumentWithInKeyword = ArgumentSyntaxExtensions.WithRefKindKeyword(argumentSyntax, refKindKeyword);
            Assert.NotNull(argumentWithInKeyword);
            Assert.True(refKindKeyword.IsEquivalentTo(argumentWithInKeyword.RefKindKeyword));
        }
    }
}
