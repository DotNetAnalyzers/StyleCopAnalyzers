// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct DeclarationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public VariableDesignationSyntaxWrapper Designation
        {
            get
            {
                return (VariableDesignationSyntaxWrapper)DesignationAccessor(this.SyntaxNode);
            }
        }

        public DeclarationExpressionSyntaxWrapper WithType(TypeSyntax type)
        {
            return new DeclarationExpressionSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }

        public DeclarationExpressionSyntaxWrapper WithDesignation(VariableDesignationSyntaxWrapper designation)
        {
            return new DeclarationExpressionSyntaxWrapper(WithDesignationAccessor(this.SyntaxNode, designation.SyntaxNode));
        }
    }
}
