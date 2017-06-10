// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct DeclarationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        private const string DeclarationExpressionSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax";
        private static readonly Type DeclarationExpressionSyntaxType;

        private static readonly Func<ExpressionSyntax, TypeSyntax> TypeAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode> DesignationAccessor;
        private static readonly Func<ExpressionSyntax, TypeSyntax, ExpressionSyntax> WithTypeAccessor;
        private static readonly Func<ExpressionSyntax, CSharpSyntaxNode, ExpressionSyntax> WithDesignationAccessor;

        private readonly ExpressionSyntax node;

        static DeclarationExpressionSyntaxWrapper()
        {
            DeclarationExpressionSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(DeclarationExpressionSyntaxTypeName);
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, TypeSyntax>(DeclarationExpressionSyntaxType, nameof(Type));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(DeclarationExpressionSyntaxType, nameof(Designation));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, TypeSyntax>(DeclarationExpressionSyntaxType, nameof(Type));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<ExpressionSyntax, CSharpSyntaxNode>(DeclarationExpressionSyntaxType, nameof(Designation));
        }

        private DeclarationExpressionSyntaxWrapper(ExpressionSyntax node)
        {
            this.node = node;
        }

        public ExpressionSyntax SyntaxNode => this.node;

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

        public static explicit operator DeclarationExpressionSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(DeclarationExpressionSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{DeclarationExpressionSyntaxTypeName}'");
            }

            return new DeclarationExpressionSyntaxWrapper((ExpressionSyntax)node);
        }

        public static implicit operator ExpressionSyntax(DeclarationExpressionSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, DeclarationExpressionSyntaxType);
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
