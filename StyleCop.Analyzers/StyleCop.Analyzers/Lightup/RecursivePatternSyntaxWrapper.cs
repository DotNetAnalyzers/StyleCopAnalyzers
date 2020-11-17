// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct RecursivePatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        static RecursivePatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RecursivePatternSyntaxWrapper));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
            PositionalPatternClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(PositionalPatternClause));
            PropertyPatternClauseAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(PropertyPatternClause));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));

            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(WrappedType, nameof(Type));
            WithPositionalPatternClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(PositionalPatternClause));
            WithPropertyPatternClauseAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(PropertyPatternClause));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public PositionalPatternClauseSyntaxWrapper PositionalPatternClause
        {
            get
            {
                return (PositionalPatternClauseSyntaxWrapper)PositionalPatternClauseAccessor(this.SyntaxNode);
            }
        }

        public PropertyPatternClauseSyntaxWrapper PropertyPatternClause
        {
            get
            {
                return (PropertyPatternClauseSyntaxWrapper)PropertyPatternClauseAccessor(this.SyntaxNode);
            }
        }

        public VariableDesignationSyntaxWrapper Designation
        {
            get
            {
                return (VariableDesignationSyntaxWrapper)DesignationAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator RecursivePatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (RecursivePatternSyntaxWrapper)node.SyntaxNode;
        }

        public static implicit operator PatternSyntaxWrapper(RecursivePatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public RecursivePatternSyntaxWrapper AddPositionalPatternClauseSubpatterns(params SubpatternSyntaxWrapper[] items)
        {
            var positionalPatternClause = this.PositionalPatternClause;
            if (positionalPatternClause.SyntaxNode is null)
            {
                positionalPatternClause = SyntaxFactoryEx.PositionalPatternClause();
            }

            return this.WithPositionalPatternClause(positionalPatternClause.WithSubpatterns(positionalPatternClause.Subpatterns.AddRange(items)));
        }

        public RecursivePatternSyntaxWrapper AddPropertyPatternClauseSubpatterns(params SubpatternSyntaxWrapper[] items)
        {
            var propertyPatternClause = this.PropertyPatternClause;
            if (propertyPatternClause.SyntaxNode is null)
            {
                propertyPatternClause = SyntaxFactoryEx.PropertyPatternClause();
            }

            return this.WithPropertyPatternClause(propertyPatternClause.WithSubpatterns(propertyPatternClause.Subpatterns.AddRange(items)));
        }

        public RecursivePatternSyntaxWrapper WithType(TypeSyntax type)
        {
            return new RecursivePatternSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }

        public RecursivePatternSyntaxWrapper WithPositionalPatternClause(PositionalPatternClauseSyntaxWrapper positionalPatternClause)
        {
            return new RecursivePatternSyntaxWrapper(WithPositionalPatternClauseAccessor(this.SyntaxNode, positionalPatternClause));
        }

        public RecursivePatternSyntaxWrapper WithPropertyPatternClause(PropertyPatternClauseSyntaxWrapper propertyPatternClause)
        {
            return new RecursivePatternSyntaxWrapper(WithPropertyPatternClauseAccessor(this.SyntaxNode, propertyPatternClause));
        }

        public RecursivePatternSyntaxWrapper WithDesignation(VariableDesignationSyntaxWrapper designation)
        {
            return new RecursivePatternSyntaxWrapper(WithDesignationAccessor(this.SyntaxNode, designation));
        }
    }
}
