// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct DeclarationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        static DeclarationExpressionSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(DeclarationExpressionSyntaxWrapper));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, TypeSyntax>(WrappedType, nameof(Type));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Designation));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(WrappedType, nameof(Designation));
        }

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
