// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal struct ParenthesizedVariableDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private const string ParenthesizedVariableDesignationSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax";
        private static readonly Type ParenthesizedVariableDesignationSyntaxType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenParenTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseParenTokenAccessor;

        private readonly CSharpSyntaxNode node;

        static ParenthesizedVariableDesignationSyntaxWrapper()
        {
            ParenthesizedVariableDesignationSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(ParenthesizedVariableDesignationSyntaxTypeName);
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(ParenthesizedVariableDesignationSyntaxType, nameof(OpenParenToken));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(ParenthesizedVariableDesignationSyntaxType, nameof(CloseParenToken));
            WithOpenParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(ParenthesizedVariableDesignationSyntaxType, nameof(OpenParenToken));
            WithCloseParenTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(ParenthesizedVariableDesignationSyntaxType, nameof(CloseParenToken));
        }

        private ParenthesizedVariableDesignationSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxList<CSharpSyntaxNode> Variables
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ParenthesizedVariableDesignationSyntaxWrapper(VariableDesignationSyntaxWrapper node)
        {
            return (ParenthesizedVariableDesignationSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ParenthesizedVariableDesignationSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(ParenthesizedVariableDesignationSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{ParenthesizedVariableDesignationSyntaxTypeName}'");
            }

            return new ParenthesizedVariableDesignationSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator VariableDesignationSyntaxWrapper(ParenthesizedVariableDesignationSyntaxWrapper wrapper)
        {
            return VariableDesignationSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(ParenthesizedVariableDesignationSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, ParenthesizedVariableDesignationSyntaxType);
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithOpenParenToken(SyntaxToken identifier)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(WithOpenParenTokenAccessor(this.SyntaxNode, identifier));
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithVariables(SeparatedSyntaxList<CSharpSyntaxNode> variables)
        {
            throw new NotImplementedException();
        }

        public ParenthesizedVariableDesignationSyntaxWrapper WithCloseParenToken(SyntaxToken identifier)
        {
            return new ParenthesizedVariableDesignationSyntaxWrapper(WithCloseParenTokenAccessor(this.SyntaxNode, identifier));
        }
    }
}
