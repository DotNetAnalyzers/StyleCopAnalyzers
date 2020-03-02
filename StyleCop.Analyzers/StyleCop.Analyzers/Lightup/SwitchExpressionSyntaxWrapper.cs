// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct SwitchExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax";

        private static readonly Type WrappedType;
        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>> ArmsAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax> GoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> OpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> SwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken> CloseBraceTokenAccessor;

        private static readonly Func<ExpressionSyntax, SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper>, ExpressionSyntax> WithArmsAccessor;
        private static readonly Func<ExpressionSyntax, ExpressionSyntax, ExpressionSyntax> WithGoverningExpressionAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithOpenBraceTokenAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithSwitchKeywordAccessor;
        private static readonly Func<ExpressionSyntax, SyntaxToken, ExpressionSyntax> WithCloseBraceTokenAccessor;

        private readonly ExpressionSyntax node;

        static SwitchExpressionSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(SwitchExpressionSyntaxWrapper));
            ArmsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<ExpressionSyntax, SwitchExpressionArmSyntaxWrapper>(WrappedType, nameof(Arms));
            GoverningExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(GoverningExpression));
            OpenBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            SwitchKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(SwitchKeyword));
            CloseBraceTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(CloseBraceToken));

            WithArmsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<ExpressionSyntax, SwitchExpressionArmSyntaxWrapper>(WrappedType, nameof(Arms));
            WithGoverningExpressionAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, ExpressionSyntax>(WrappedType, nameof(GoverningExpression));
            WithOpenBraceTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(OpenBraceToken));
            WithSwitchKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, SyntaxToken>(WrappedType, nameof(SwitchKeyword));
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

        public SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper> Arms
        {
            get
            {
                return ArmsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenBraceToken
        {
            get
            {
                return OpenBraceTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken SwitchKeyword
        {
            get
            {
                return SwitchKeywordAccessor(this.SyntaxNode);
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

        public SwitchExpressionSyntaxWrapper AddArms(params SwitchExpressionArmSyntaxWrapper[] arms)
        {
            return new SwitchExpressionSyntaxWrapper(this.WithArms(this.Arms.AddRange(arms)));
        }

        public SwitchExpressionSyntaxWrapper WithArms(SeparatedSyntaxListWrapper<SwitchExpressionArmSyntaxWrapper> arms)
        {
            return new SwitchExpressionSyntaxWrapper(WithArmsAccessor(this.SyntaxNode, arms));
        }

        public SwitchExpressionSyntaxWrapper WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return new SwitchExpressionSyntaxWrapper(WithCloseBraceTokenAccessor(this.SyntaxNode, closeBraceToken));
        }

        public SwitchExpressionSyntaxWrapper WithGoverningExpression(ExpressionSyntax expression)
        {
            return new SwitchExpressionSyntaxWrapper(WithGoverningExpressionAccessor(this.SyntaxNode, expression));
        }

        public SwitchExpressionSyntaxWrapper WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return new SwitchExpressionSyntaxWrapper(WithOpenBraceTokenAccessor(this.SyntaxNode, openBraceToken));
        }

        public SwitchExpressionSyntaxWrapper WithSwitchKeyword(SyntaxToken switchKeyword)
        {
            return new SwitchExpressionSyntaxWrapper(WithSwitchKeywordAccessor(this.SyntaxNode, switchKeyword));
        }
    }
}
