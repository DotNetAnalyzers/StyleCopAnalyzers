// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct DiscardDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> UnderscoreTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithUnderscoreTokenAccessor;

        static DiscardDesignationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(DiscardDesignationSyntaxWrapper));
            UnderscoreTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(UnderscoreToken));
            WithUnderscoreTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(UnderscoreToken));
        }

        public SyntaxToken UnderscoreToken
        {
            get
            {
                return UnderscoreTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator DiscardDesignationSyntaxWrapper(VariableDesignationSyntaxWrapper node)
        {
            return (DiscardDesignationSyntaxWrapper)node.SyntaxNode;
        }

        public static implicit operator VariableDesignationSyntaxWrapper(DiscardDesignationSyntaxWrapper wrapper)
        {
            return VariableDesignationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public DiscardDesignationSyntaxWrapper WithUnderscoreToken(SyntaxToken identifier)
        {
            return new DiscardDesignationSyntaxWrapper(WithUnderscoreTokenAccessor(this.SyntaxNode, identifier));
        }
    }
}
