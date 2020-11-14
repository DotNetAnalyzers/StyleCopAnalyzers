// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct VarPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> VarKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> DesignationAccessor;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithVarKeywordAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithDesignationAccessor;

        static VarPatternSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(VarPatternSyntaxWrapper));
            VarKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(VarKeyword));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));

            WithVarKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(VarKeyword));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));
        }

        public SyntaxToken VarKeyword
        {
            get
            {
                return VarKeywordAccessor(this.SyntaxNode);
            }
        }

        public VariableDesignationSyntaxWrapper Designation
        {
            get
            {
                return (VariableDesignationSyntaxWrapper)DesignationAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator VarPatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (VarPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static implicit operator PatternSyntaxWrapper(VarPatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public VarPatternSyntaxWrapper WithVarKeyword(SyntaxToken varKeyword)
        {
            return new VarPatternSyntaxWrapper(WithVarKeywordAccessor(this.SyntaxNode, varKeyword));
        }

        public VarPatternSyntaxWrapper WithDesignation(VariableDesignationSyntaxWrapper designation)
        {
            return new VarPatternSyntaxWrapper(WithDesignationAccessor(this.SyntaxNode, designation));
        }
    }
}
