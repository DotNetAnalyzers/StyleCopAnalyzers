// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class StackAllocArrayCreationExpressionSyntaxExtensionsTests
    {
        [Fact]
        public void TestInitializer()
        {
            var stackAllocSyntax = SyntaxFactory.StackAllocArrayCreationExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
                .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression));
            Assert.NotNull(StackAllocArrayCreationExpressionSyntaxExtensions.Initializer(stackAllocSyntax));
            Assert.Equal(SyntaxKind.ArrayInitializerExpression, StackAllocArrayCreationExpressionSyntaxExtensions.Initializer(stackAllocSyntax).Kind());
        }

        [Fact]
        public void TestWithInitializer()
        {
            var stackAllocSyntax = SyntaxFactory.StackAllocArrayCreationExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
            var stackAllocWithDefaultInitializer = StackAllocArrayCreationExpressionSyntaxExtensions.WithInitializer(stackAllocSyntax, null);
            Assert.Null(StackAllocArrayCreationExpressionSyntaxExtensions.Initializer(stackAllocWithDefaultInitializer));
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression);
            var stackAllocWithInitializer = StackAllocArrayCreationExpressionSyntaxExtensions.WithInitializer(stackAllocSyntax, initializer);
            Assert.NotNull(stackAllocWithInitializer.Initializer);
            Assert.Equal(SyntaxKind.ArrayInitializerExpression, stackAllocWithInitializer.Initializer.Kind());
            Assert.True(stackAllocWithInitializer.Initializer.IsEquivalentTo(initializer));
        }
    }
}
