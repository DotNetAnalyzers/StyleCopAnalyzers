// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class CrefParameterSyntaxExtensionsTests
    {
        [Fact]
        public void TestRefKindKeyword()
        {
            var crefParameterSyntax = SyntaxFactory.CrefParameter(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
            Assert.Equal(default, CrefParameterSyntaxExtensions.RefKindKeyword(crefParameterSyntax));
        }

        [Fact]
        public void TestWithRefKindKeyword()
        {
            var crefParameterSyntax = SyntaxFactory.CrefParameter(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));

            // With default value is allowed
            var crefParameterWithDefaultRefKind = CrefParameterSyntaxExtensions.WithRefKindKeyword(crefParameterSyntax, default);
            Assert.Equal(default, CrefParameterSyntaxExtensions.RefKindKeyword(crefParameterWithDefaultRefKind));

            // Non-default throws an exception
            var refKind = SyntaxFactory.Token(SyntaxKind.RefKeyword);
            Assert.Throws<NotSupportedException>(() => CrefParameterSyntaxExtensions.WithRefKindKeyword(crefParameterSyntax, refKind));
        }
    }
}
