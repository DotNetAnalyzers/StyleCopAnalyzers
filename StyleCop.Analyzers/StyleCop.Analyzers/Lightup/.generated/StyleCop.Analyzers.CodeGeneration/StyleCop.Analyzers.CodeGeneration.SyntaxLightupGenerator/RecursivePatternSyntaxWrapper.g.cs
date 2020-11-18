// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RecursivePatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax";
        private static readonly Type WrappedType;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax> TypeAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> PositionalPatternClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> PropertyPatternClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> DesignationAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithPositionalPatternClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithPropertyPatternClauseAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithDesignationAccessor;
        private readonly CSharpSyntaxNode node;
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

        private RecursivePatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;
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

        public static explicit operator RecursivePatternSyntaxWrapper(ExpressionOrPatternSyntaxWrapper node)
        {
            return (RecursivePatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator RecursivePatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new RecursivePatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator PatternSyntaxWrapper(RecursivePatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator ExpressionOrPatternSyntaxWrapper(RecursivePatternSyntaxWrapper wrapper)
        {
            return ExpressionOrPatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(RecursivePatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
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
