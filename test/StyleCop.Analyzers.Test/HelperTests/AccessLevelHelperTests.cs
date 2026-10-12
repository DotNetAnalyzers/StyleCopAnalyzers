// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.HelperTests
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;
    using Xunit;

    public class AccessLevelHelperTests
    {
        [Fact]
        public void TestNotSpecifiedToAccessibility()
        {
            Assert.Throws<ArgumentException>(() => AccessLevel.NotSpecified.ToAccessibility());
        }

        [Fact]
        public void TestCombineEffectiveAccessibility()
        {
            // If enclosing is private, declared does not matter
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.NotApplicable, Accessibility.Private));
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.Private));
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Internal, Accessibility.Private));
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Protected, Accessibility.Private));
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.Private));

            // Test enclosing is ProtectedAndInternal
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.ProtectedAndInternal));
            Assert.Equal(Accessibility.ProtectedAndInternal, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.ProtectedAndInternal));

            // Test enclosing is Protected
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.Protected));
            Assert.Equal(Accessibility.ProtectedAndInternal, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Internal, Accessibility.Protected));
            Assert.Equal(Accessibility.Protected, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.Protected));

            // Test enclosing is Internal
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.Internal));
            Assert.Equal(Accessibility.ProtectedAndInternal, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Protected, Accessibility.Internal));
            Assert.Equal(Accessibility.Internal, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.Internal));

            // Test enclosing is ProtectedOrInternal
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.ProtectedOrInternal));
            Assert.Equal(Accessibility.ProtectedOrInternal, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.ProtectedOrInternal));

            // Test enclosing is Public
            Assert.Equal(Accessibility.Private, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Private, Accessibility.Public));
            Assert.Equal(Accessibility.Public, AccessLevelHelper.CombineEffectiveAccessibility(Accessibility.Public, Accessibility.Public));
        }

        [Fact]
        public void TestGetAccessLevel()
        {
            Check(AccessLevel.NotSpecified);
            Check(AccessLevel.NotSpecified, SyntaxKind.PartialKeyword);

            Check(AccessLevel.Private, SyntaxKind.PrivateKeyword);
            Check(AccessLevel.Private, SyntaxKind.OverrideKeyword, SyntaxKind.PrivateKeyword);

            Check(AccessLevel.PrivateProtected, SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword);
            Check(AccessLevel.PrivateProtected, SyntaxKind.ProtectedKeyword, SyntaxKind.PrivateKeyword);

            Check(AccessLevel.Protected, SyntaxKind.ProtectedKeyword);
            Check(AccessLevel.Protected, SyntaxKind.VirtualKeyword, SyntaxKind.ProtectedKeyword);

            Check(AccessLevel.ProtectedInternal, SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword);
            Check(AccessLevel.ProtectedInternal, SyntaxKind.InternalKeyword, SyntaxKind.ProtectedKeyword);

            Check(AccessLevel.Internal, SyntaxKind.InternalKeyword);
            Check(AccessLevel.Internal, SyntaxKind.AbstractKeyword, SyntaxKind.InternalKeyword);

            Check(AccessLevel.Public, SyntaxKind.PublicKeyword);
            Check(AccessLevel.Public, SyntaxKind.AsyncKeyword, SyntaxKind.PublicKeyword);

            static void Check(AccessLevel expectedAccessLevel, params SyntaxKind[] tokenKinds)
            {
                var tokenList = SyntaxFactory.TokenList(tokenKinds.Select(SyntaxFactory.Token));
                Assert.Equal(expectedAccessLevel, AccessLevelHelper.GetAccessLevel(tokenList));
            }
        }
    }
}
