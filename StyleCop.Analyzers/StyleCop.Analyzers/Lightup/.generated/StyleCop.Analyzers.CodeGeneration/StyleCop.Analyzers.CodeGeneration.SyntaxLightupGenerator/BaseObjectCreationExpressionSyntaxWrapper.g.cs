// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct BaseObjectCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.BaseObjectCreationExpressionSyntax";
        private static readonly Type WrappedType;
        private readonly ExpressionSyntax node;
        private BaseObjectCreationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public static implicit operator ExpressionSyntax(BaseObjectCreationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
