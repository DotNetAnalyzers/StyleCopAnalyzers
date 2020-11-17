// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct VarPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
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
