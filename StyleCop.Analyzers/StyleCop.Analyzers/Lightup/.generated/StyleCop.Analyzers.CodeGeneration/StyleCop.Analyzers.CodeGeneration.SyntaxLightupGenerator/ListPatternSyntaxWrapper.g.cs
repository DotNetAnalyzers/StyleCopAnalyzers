// Copyright (c) Contributors to the New StyleCop Analyzers project.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ListPatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ListPatternSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<CSharpSyntaxNode, SyntaxToken> OpenBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<PatternSyntaxWrapper>> PatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken> CloseBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> DesignationAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithOpenBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, SeparatedSyntaxListWrapper<PatternSyntaxWrapper>, CSharpSyntaxNode> WithPatternsAccessor;
        private static readonly Func<CSharpSyntaxNode, SyntaxToken, CSharpSyntaxNode> WithCloseBracketTokenAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithDesignationAccessor;

        private readonly CSharpSyntaxNode node;

        static ListPatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(ListPatternSyntaxWrapper));
            OpenBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            PatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListPropertyAccessor<CSharpSyntaxNode, PatternSyntaxWrapper>(WrappedType, nameof(Patterns));
            CloseBracketTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            DesignationAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));
            WithOpenBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(OpenBracketToken));
            WithPatternsAccessor = LightupHelpers.CreateSeparatedSyntaxListWithPropertyAccessor<CSharpSyntaxNode, PatternSyntaxWrapper>(WrappedType, nameof(Patterns));
            WithCloseBracketTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, SyntaxToken>(WrappedType, nameof(CloseBracketToken));
            WithDesignationAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Designation));
        }

        private ListPatternSyntaxWrapper(CSharpSyntaxNode node)
        {
            this.node = node;
        }

        public CSharpSyntaxNode SyntaxNode => this.node;

        public SyntaxToken OpenBracketToken
        {
            get
            {
                return OpenBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public SeparatedSyntaxListWrapper<PatternSyntaxWrapper> Patterns
        {
            get
            {
                return PatternsAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseBracketToken
        {
            get
            {
                return CloseBracketTokenAccessor(this.SyntaxNode);
            }
        }

        public VariableDesignationSyntaxWrapper Designation
        {
            get
            {
                return (VariableDesignationSyntaxWrapper)DesignationAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator ListPatternSyntaxWrapper(PatternSyntaxWrapper node)
        {
            return (ListPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ListPatternSyntaxWrapper(ExpressionOrPatternSyntaxWrapper node)
        {
            return (ListPatternSyntaxWrapper)node.SyntaxNode;
        }

        public static explicit operator ListPatternSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new ListPatternSyntaxWrapper((CSharpSyntaxNode)node);
        }

        public static implicit operator PatternSyntaxWrapper(ListPatternSyntaxWrapper wrapper)
        {
            return PatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator ExpressionOrPatternSyntaxWrapper(ListPatternSyntaxWrapper wrapper)
        {
            return ExpressionOrPatternSyntaxWrapper.FromUpcast(wrapper.node);
        }

        public static implicit operator CSharpSyntaxNode(ListPatternSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public ListPatternSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new ListPatternSyntaxWrapper(WithOpenBracketTokenAccessor(this.SyntaxNode, openBracketToken));
        }

        public ListPatternSyntaxWrapper WithPatterns(SeparatedSyntaxListWrapper<PatternSyntaxWrapper> patterns)
        {
            return new ListPatternSyntaxWrapper(WithPatternsAccessor(this.SyntaxNode, patterns));
        }

        public ListPatternSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new ListPatternSyntaxWrapper(WithCloseBracketTokenAccessor(this.SyntaxNode, closeBracketToken));
        }

        public ListPatternSyntaxWrapper WithDesignation(VariableDesignationSyntaxWrapper designation)
        {
            return new ListPatternSyntaxWrapper(WithDesignationAccessor(this.SyntaxNode, designation));
        }
    }
}
