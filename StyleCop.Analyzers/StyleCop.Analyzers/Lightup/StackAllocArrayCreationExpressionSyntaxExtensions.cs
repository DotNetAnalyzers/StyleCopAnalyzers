// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class StackAllocArrayCreationExpressionSyntaxExtensions
    {
        private static readonly Func<StackAllocArrayCreationExpressionSyntax, InitializerExpressionSyntax> InitializerAccessor;
        private static readonly Func<StackAllocArrayCreationExpressionSyntax, InitializerExpressionSyntax, StackAllocArrayCreationExpressionSyntax> WithInitializerAccessor;

        static StackAllocArrayCreationExpressionSyntaxExtensions()
        {
            InitializerAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StackAllocArrayCreationExpressionSyntax, InitializerExpressionSyntax>(typeof(StackAllocArrayCreationExpressionSyntax), nameof(Initializer));
            WithInitializerAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StackAllocArrayCreationExpressionSyntax, InitializerExpressionSyntax>(typeof(StackAllocArrayCreationExpressionSyntax), nameof(Initializer));
        }

        public static InitializerExpressionSyntax Initializer(this StackAllocArrayCreationExpressionSyntax syntax)
        {
            return InitializerAccessor(syntax);
        }

        public static StackAllocArrayCreationExpressionSyntax WithInitializer(this StackAllocArrayCreationExpressionSyntax syntax, InitializerExpressionSyntax initializer)
        {
            return WithInitializerAccessor(syntax, initializer);
        }
    }
}
