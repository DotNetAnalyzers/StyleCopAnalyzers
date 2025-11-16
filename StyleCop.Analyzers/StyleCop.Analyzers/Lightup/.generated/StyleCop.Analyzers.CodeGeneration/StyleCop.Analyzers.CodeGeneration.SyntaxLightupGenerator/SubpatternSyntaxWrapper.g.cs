// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct SubpatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> ExpressionColonAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> PatternAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithExpressionColonAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithPatternAccessor;

        private readonly CSharpSyntaxNode node;

        static SubpatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SubpatternSyntaxWrapper));
            ExpressionColonAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(ExpressionColon));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
            WithExpressionColonAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(ExpressionColon));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
        }

        private SubpatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;


        public BaseExpressionColonSyntaxWrapper ExpressionColon
        {
            get
            {
                return (BaseExpressionColonSyntaxWrapper)ExpressionColonAccessor(this.SyntaxNode);
            }
        }

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator SubpatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new SubpatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator CSharpSyntaxNode(SubpatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public SubpatternSyntaxWrapper WithExpressionColon(BaseExpressionColonSyntaxWrapper expressionColon)
        {
            return new SubpatternSyntaxWrapper(WithExpressionColonAccessor(this.SyntaxNode, expressionColon));
        }

        public SubpatternSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new SubpatternSyntaxWrapper(WithPatternAccessor(this.SyntaxNode, pattern));
        }
    }
}
