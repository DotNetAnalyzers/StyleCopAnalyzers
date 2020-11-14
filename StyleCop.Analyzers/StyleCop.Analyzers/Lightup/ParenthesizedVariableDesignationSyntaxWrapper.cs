// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct ParenthesizedVariableDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper>> VariablesAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper>, CSharpSyntaxNode> WithVariablesAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseParenTokenAccessor;

        static ParenthesizedVariableDesignationSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(ParenthesizedVariableDesignationSyntaxWrapper));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            VariablesAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, VariableDesignationSyntaxWrapper>(WrappedType, nameof(Variables));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenParenToken));
            WithVariablesAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, VariableDesignationSyntaxWrapper>(WrappedType, nameof(Variables));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseParenToken));
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper> Variables
        {
            get
            {
                return VariablesAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ParenthesizedVariableDesignationSyntaxWrapper(VariableDesignationSyntaxWrapper node)
        {
            return (ParenthesizedVariableDesignationSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ParenthesizedVariableDesignationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ParenthesizedVariableDesignationSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator VariableDesignationSyntaxWrapper(ParenthesizedVariableDesignationSyntaxWrapper wrapper)
        {
            return VariableDesignationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(ParenthesizedVariableDesignationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ParenthesizedVariableDesignationSyntaxWrapper AddVariables(params VariableDesignationSyntaxWrapper[] items)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(this.WithVariables(this.Variables.AddRange(items)));
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithOpenParenToken(SyntaxToken identifier)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, identifier));
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithVariables(SeparatedSyntaxListWrapper<VariableDesignationSyntaxWrapper> variables)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(WithVariablesAccessor(this.SyntaxNode, variables));
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithCloseParenToken(SyntaxToken identifier)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, identifier));
        }
    }
}
