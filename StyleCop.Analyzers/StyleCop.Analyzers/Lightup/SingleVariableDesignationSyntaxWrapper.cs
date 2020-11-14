// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct SingleVariableDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> IdentifierAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithIdentifierAccessor;

        static SingleVariableDesignationSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(SingleVariableDesignationSyntaxWrapper));
            IdentifierAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Identifier));
            WithIdentifierAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(Identifier));
        }

        public SyntaxToken Identifier
        {
            get
            {
                return IdentifierAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator SingleVariableDesignationSyntaxWrapper(VariableDesignationSyntaxWrapper node)
        {
            return (SingleVariableDesignationSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator SingleVariableDesignationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new SingleVariableDesignationSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator VariableDesignationSyntaxWrapper(SingleVariableDesignationSyntaxWrapper wrapper)
        {
            return VariableDesignationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public SingleVariableDesignationSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new SingleVariableDesignationSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }
    }
}
