// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct SingleVariableDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        static SingleVariableDesignationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SingleVariableDesignationSyntaxWrapper));
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
