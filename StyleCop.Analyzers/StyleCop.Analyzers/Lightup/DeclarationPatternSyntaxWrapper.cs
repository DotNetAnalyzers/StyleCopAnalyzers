// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct DeclarationPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private const string DeclarationPatternSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax";
        private static readonly Type DeclarationPatternSyntaxType;

        private static readonly Func<CSharpSyntaxNode, TypeSyntax> TypeAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> DesignationAccessor;
        private static readonly Func<CSharpSyntaxNode, TypeSyntax, CSharpSyntaxNode> WithTypeAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithDesignationAccessor;

        private readonly CSharpSyntaxNode node;

        static DeclarationPatternSyntaxWrapper()
        {
            DeclarationPatternSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(DeclarationPatternSyntaxTypeName);
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(DeclarationPatternSyntaxType, nameof(Type));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(DeclarationPatternSyntaxType, nameof(Designation));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, TypeSyntax>(DeclarationPatternSyntaxType, nameof(Type));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(DeclarationPatternSyntaxType, nameof(Designation));
        }

        private DeclarationPatternSyntaxWrapper(CSharpSyntaxNode node)
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

        public VariableDesignationSyntaxWrapper Designation
        {
            get
            {
                return (VariableDesignationSyntaxWrapper)DesignationAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator DeclarationPatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (DeclarationPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator DeclarationPatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(DeclarationPatternSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{DeclarationPatternSyntaxTypeName}'");
            }

            return new DeclarationPatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator PatternSyntaxWrapper(DeclarationPatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(DeclarationPatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, DeclarationPatternSyntaxType);
        }

        public DeclarationPatternSyntaxWrapper WithType(TypeSyntax type)
        {
            return new DeclarationPatternSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }

        public DeclarationPatternSyntaxWrapper WithDesignation(VariableDesignationSyntaxWrapper designation)
        {
            return new DeclarationPatternSyntaxWrapper(WithDesignationAccessor(this.SyntaxNode, designation.SyntaxNode));
        }
    }
}
