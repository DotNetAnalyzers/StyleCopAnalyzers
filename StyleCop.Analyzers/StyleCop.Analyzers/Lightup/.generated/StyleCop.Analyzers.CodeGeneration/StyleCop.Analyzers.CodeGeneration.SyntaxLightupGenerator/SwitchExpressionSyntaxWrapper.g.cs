// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct SwitchExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> GoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> SwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>> ArmsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithGoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithSwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>, ExpressionSyntax> WithArmsAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBraceTokenAccessor;
        private readonly ExpressionSyntax node;
        static SwitchExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SwitchExpressionSyntaxWrapper));
            GoverningExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(GoverningExpression));
            SwitchKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(SwitchKeyword));
            OpenBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            ArmsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<ExpressionSyntax, SwitchExpressionArmSyntaxWrapper>(WrappedType, nameof(Arms));
            CloseBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
            WithGoverningExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(GoverningExpression));
            WithSwitchKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(SwitchKeyword));
            WithOpenBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            WithArmsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<ExpressionSyntax, SwitchExpressionArmSyntaxWrapper>(WrappedType, nameof(Arms));
            WithCloseBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBraceToken));
        }

        private SwitchExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;
        public ExpressionSyntax GoverningExpression
        {
            get
            {
                return GoverningExpressionAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken SwitchKeyword
        {
            get
            {
                return SwitchKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenBraceToken
        {
            get
            {
                return OpenBraceTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper> Arms
        {
            get
            {
                return ArmsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBraceToken
        {
            get
            {
                return CloseBraceTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator SwitchExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new SwitchExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(SwitchExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
