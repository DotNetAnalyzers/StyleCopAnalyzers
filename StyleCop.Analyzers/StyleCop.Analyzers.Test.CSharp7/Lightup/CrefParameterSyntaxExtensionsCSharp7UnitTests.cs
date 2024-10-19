﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class CrefParameterSyntaxExtensionsCSharp7UnitTests
    {
        [Fact]
        public void TestRefKindKeyword()
        {
            var crefParameterSyntax = SyntaxFactory.CrefParameter(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
                .WithRefKindKeyword(SyntaxFactory.Token(SyntaxKind.InKeyword));
            Assert.Equal(crefParameterSyntax.RefKindKeyword, CrefParameterSyntaxExtensions.RefKindKeyword(crefParameterSyntax));
            Assert.Equal(crefParameterSyntax.RefOrOutKeyword, CrefParameterSyntaxExtensions.RefKindKeyword(crefParameterSyntax));
        }

        [Fact]
        public void TestWithRefKindKeyword()
        {
            var crefParameterSyntax = SyntaxFactory.CrefParameter(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));

            var refKindKeyword = SyntaxFactory.Token(SyntaxKind.InKeyword);
            Assert.Equal(default, CrefParameterSyntaxExtensions.RefKindKeyword(crefParameterSyntax));
            var crefParameterWithInKeyword = CrefParameterSyntaxExtensions.WithRefKindKeyword(crefParameterSyntax, refKindKeyword);
            Assert.NotNull(crefParameterWithInKeyword);
            Assert.True(refKindKeyword.IsEquivalentTo(crefParameterWithInKeyword.RefKindKeyword));
        }
    }
}
