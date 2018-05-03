// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using StyleCop.Analyzers.Lightup;
    using Xunit;

    public class LightupHelpersTests
    {
        [Fact]
        public void TestCanWrapNullNode()
        {
            Assert.True(LightupHelpers.CanWrapNode(null, typeof(PatternSyntaxWrapper)));
        }

        [Fact]
        public void TestCanAccessNonExistentProperty()
        {
            var propertyAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<SyntaxNode, object>(typeof(SyntaxNode), "NonExistentProperty");
            Assert.NotNull(propertyAccessor);
            Assert.Null(propertyAccessor(SyntaxFactory.AccessorList()));
            Assert.Throws<NullReferenceException>(() => propertyAccessor(null));

            var withPropertyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<SyntaxNode, object>(typeof(SyntaxNode), "NonExistentProperty");
            Assert.NotNull(withPropertyAccessor);
            Assert.NotNull(withPropertyAccessor(SyntaxFactory.AccessorList(), null));
            Assert.ThrowsAny<NotSupportedException>(() => withPropertyAccessor(SyntaxFactory.AccessorList(), new object()));
            Assert.Throws<NullReferenceException>(() => withPropertyAccessor(null, new object()));

            var separatedListPropertyAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<SyntaxNode, SyntaxNode>(typeof(SyntaxNode), "NonExistentProperty");
            Assert.NotNull(separatedListPropertyAccessor);
            Assert.NotNull(separatedListPropertyAccessor(SyntaxFactory.AccessorList()));
            Assert.Throws<NullReferenceException>(() => separatedListPropertyAccessor(null));

            var separatedListWithPropertyAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<SyntaxNode, SyntaxNode>(typeof(SyntaxNode), "NonExistentProperty");
            Assert.NotNull(separatedListWithPropertyAccessor);
            Assert.NotNull(separatedListWithPropertyAccessor(SyntaxFactory.AccessorList(), null));
            Assert.ThrowsAny<NotSupportedException>(() => separatedListWithPropertyAccessor(SyntaxFactory.AccessorList(), new SeparatedSyntaxListWrapper<SyntaxNode>.AutoWrapSeparatedSyntaxList<LiteralExpressionSyntax>(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)))));
            Assert.Throws<NullReferenceException>(() => separatedListWithPropertyAccessor(null, SeparatedSyntaxListWrapper<SyntaxNode>.UnsupportedEmpty));
        }

        [Fact]
        public void TestCreateSyntaxPropertyAccessor()
        {
            // The call *should* have been made with the first generic argument set to `BaseMethodDeclarationSyntax`
            // instead of `MethodDeclarationSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSyntaxPropertyAccessor<MethodDeclarationSyntax, BlockSyntax>(typeof(BaseMethodDeclarationSyntax), nameof(BaseMethodDeclarationSyntax.Body)));

            // The call *should* have been made with the second generic argument set to `ArrowExpressionClauseSyntax`
            // instead of `BlockSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSyntaxPropertyAccessor<MethodDeclarationSyntax, BlockSyntax>(typeof(MethodDeclarationSyntax), nameof(MethodDeclarationSyntax.ExpressionBody)));
        }

        [Fact]
        public void TestCreateSeparatedSyntaxListPropertyAccessor()
        {
            // The call works for `SeparatedSyntaxList<T>`, not work for `SyntaxList<T>`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<BlockSyntax, StatementSyntax>(typeof(BlockSyntax), nameof(BlockSyntax.Statements)));

            // The call *should* have been made with the first generic argument set to `BaseParameterListSyntax`
            // instead of `ParameterListSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<ParameterListSyntax, ParameterSyntax>(typeof(BaseParameterListSyntax), nameof(BaseParameterListSyntax.Parameters)));
        }

        [Fact]
        public void TestCreateSeparatedSyntaxListPropertyAccessorValidateElementType()
        {
            // The call *should* have been made with the second generic argument set to `ParameterSyntax`
            // instead of `ArgumentSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<BaseParameterListSyntax, ArgumentSyntax>(typeof(BaseParameterListSyntax), nameof(BaseParameterListSyntax.Parameters)));
        }

        [Fact]
        public void TestCreateSyntaxWithPropertyAccessor()
        {
            // The call *should* have been made with the first generic argument set to `BaseMethodDeclarationSyntax`
            // instead of `MethodDeclarationSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSyntaxWithPropertyAccessor<MethodDeclarationSyntax, BlockSyntax>(typeof(BaseMethodDeclarationSyntax), nameof(BaseMethodDeclarationSyntax.Body)));

            // The call *should* have been made with the second generic argument set to `ArrowExpressionClauseSyntax`
            // instead of `BlockSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSyntaxWithPropertyAccessor<MethodDeclarationSyntax, BlockSyntax>(typeof(MethodDeclarationSyntax), nameof(MethodDeclarationSyntax.ExpressionBody)));
        }

        [Fact]
        public void TestCreateSeparatedSyntaxListWithPropertyAccessor()
        {
            // The call works for `SeparatedSyntaxList<T>`, not work for `SyntaxList<T>`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<BlockSyntax, StatementSyntax>(typeof(BlockSyntax), nameof(BlockSyntax.Statements)));

            // The call *should* have been made with the first generic argument set to `BaseParameterListSyntax`
            // instead of `ParameterListSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<ParameterListSyntax, ParameterSyntax>(typeof(BaseParameterListSyntax), nameof(BaseParameterListSyntax.Parameters)));
        }

        [Fact]
        public void TestCreateSeparatedSyntaxListWithPropertyAccessorValidateElementType()
        {
            // The call *should* have been made with the second generic argument set to `ParameterSyntax`
            // instead of `ArgumentSyntax`.
            Assert.ThrowsAny<InvalidOperationException>(() => LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<BaseParameterListSyntax, ArgumentSyntax>(typeof(BaseParameterListSyntax), nameof(BaseParameterListSyntax.Parameters)));
        }
    }
}
