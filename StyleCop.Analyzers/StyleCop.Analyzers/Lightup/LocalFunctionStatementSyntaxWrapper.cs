// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct LocalFunctionStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax";
        private static readonly Type WrappedType;

        private static readonly Func<StatementSyntax, SyntaxTokenList> ModifiersAccessor;
        private static readonly Func<StatementSyntax, TypeSyntax> ReturnTypeAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> IdentifierAccessor;
        private static readonly Func<StatementSyntax, TypeParameterListSyntax> TypeParameterListAccessor;
        private static readonly Func<StatementSyntax, ParameterListSyntax> ParameterListAccessor;
        private static readonly Func<StatementSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>> ConstraintClausesAccessor;
        private static readonly Func<StatementSyntax, BlockSyntax> BodyAccessor;
        private static readonly Func<StatementSyntax, ArrowExpressionClauseSyntax> ExpressionBodyAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> SemicolonTokenAccessor;
        private static readonly Func<StatementSyntax, SyntaxTokenList, StatementSyntax> WithModifiersAccessor;
        private static readonly Func<StatementSyntax, TypeSyntax, StatementSyntax> WithReturnTypeAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithIdentifierAccessor;
        private static readonly Func<StatementSyntax, TypeParameterListSyntax, StatementSyntax> WithTypeParameterListAccessor;
        private static readonly Func<StatementSyntax, ParameterListSyntax, StatementSyntax> WithParameterListAccessor;
        private static readonly Func<StatementSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>, StatementSyntax> WithConstraintClausesAccessor;
        private static readonly Func<StatementSyntax, BlockSyntax, StatementSyntax> WithBodyAccessor;
        private static readonly Func<StatementSyntax, ArrowExpressionClauseSyntax, StatementSyntax> WithExpressionBodyAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken, StatementSyntax> WithSemicolonTokenAccessor;

        private readonly StatementSyntax node;

        static LocalFunctionStatementSyntaxWrapper()
        {
            WrappedType = WrapperHelper.GetWrappedType(typeof(LocalFunctionStatementSyntaxWrapper));
            ModifiersAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            ReturnTypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, TypeSyntax>(WrappedType, nameof(ReturnType));
            IdentifierAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(Identifier));
            TypeParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, TypeParameterListSyntax>(WrappedType, nameof(TypeParameterList));
            ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ParameterListSyntax>(WrappedType, nameof(ParameterList));
            ConstraintClausesAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>>(WrappedType, nameof(ConstraintClauses));
            BodyAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, BlockSyntax>(WrappedType, nameof(Body));
            ExpressionBodyAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ArrowExpressionClauseSyntax>(WrappedType, nameof(ExpressionBody));
            SemicolonTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
            WithModifiersAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxTokenList>(WrappedType, nameof(Modifiers));
            WithReturnTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, TypeSyntax>(WrappedType, nameof(ReturnType));
            WithIdentifierAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(Identifier));
            WithTypeParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, TypeParameterListSyntax>(WrappedType, nameof(TypeParameterList));
            WithParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ParameterListSyntax>(WrappedType, nameof(ParameterList));
            WithConstraintClausesAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxList<TypeParameterConstraintClauseSyntax>>(WrappedType, nameof(ConstraintClauses));
            WithBodyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, BlockSyntax>(WrappedType, nameof(Body));
            WithExpressionBodyAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, ArrowExpressionClauseSyntax>(WrappedType, nameof(ExpressionBody));
            WithSemicolonTokenAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<StatementSyntax, SyntaxToken>(WrappedType, nameof(SemicolonToken));
        }

        private LocalFunctionStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;

        public SyntaxTokenList Modifiers
        {
            get
            {
                return ModifiersAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax ReturnType
        {
            get
            {
                return ReturnTypeAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken Identifier
        {
            get
            {
                return IdentifierAccessor(this.SyntaxNode);
            }
        }

        public TypeParameterListSyntax TypeParameterList
        {
            get
            {
                return TypeParameterListAccessor(this.SyntaxNode);
            }
        }

        public ParameterListSyntax ParameterList
        {
            get
            {
                return ParameterListAccessor(this.SyntaxNode);
            }
        }

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses
        {
            get
            {
                return ConstraintClausesAccessor(this.SyntaxNode);
            }
        }

        public BlockSyntax Body
        {
            get
            {
                return BodyAccessor(this.SyntaxNode);
            }
        }

        public ArrowExpressionClauseSyntax ExpressionBody
        {
            get
            {
                return ExpressionBodyAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken SemicolonToken
        {
            get
            {
                return SemicolonTokenAccessor(this.SyntaxNode);
            }
        }

        public static explicit operator LocalFunctionStatementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default;
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{WrappedTypeName}'");
            }

            return new LocalFunctionStatementSyntaxWrapper((StatementSyntax)node);
        }

        public static implicit operator StatementSyntax(LocalFunctionStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }

        public LocalFunctionStatementSyntaxWrapper WithModifiers(SyntaxTokenList modifiers)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithModifiersAccessor(this.SyntaxNode, modifiers));
        }

        public LocalFunctionStatementSyntaxWrapper WithReturnType(TypeSyntax returnType)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithReturnTypeAccessor(this.SyntaxNode, returnType));
        }

        public LocalFunctionStatementSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }

        public LocalFunctionStatementSyntaxWrapper WithTypeParameterList(TypeParameterListSyntax typeParameterList)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithTypeParameterListAccessor(this.SyntaxNode, typeParameterList));
        }

        public LocalFunctionStatementSyntaxWrapper WithParameterList(ParameterListSyntax parameterList)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithParameterListAccessor(this.SyntaxNode, parameterList));
        }

        public LocalFunctionStatementSyntaxWrapper WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithConstraintClausesAccessor(this.SyntaxNode, constraintClauses));
        }

        public LocalFunctionStatementSyntaxWrapper WithBody(BlockSyntax body)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithBodyAccessor(this.SyntaxNode, body));
        }

        public LocalFunctionStatementSyntaxWrapper WithExpressionBody(ArrowExpressionClauseSyntax expressionBody)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithExpressionBodyAccessor(this.SyntaxNode, expressionBody));
        }

        public LocalFunctionStatementSyntaxWrapper WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return new LocalFunctionStatementSyntaxWrapper(WithSemicolonTokenAccessor(this.SyntaxNode, semicolonToken));
        }

        public LocalFunctionStatementSyntaxWrapper AddModifiers(params SyntaxToken[] items)
        {
            return this.WithModifiers(this.Modifiers.AddRange(items));
        }

        public LocalFunctionStatementSyntaxWrapper AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            var typeParameterList = this.TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return this.WithTypeParameterList(typeParameterList.WithParameters(typeParameterList.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntaxWrapper AddParameterListParameters(params ParameterSyntax[] items)
        {
            return this.WithParameterList(this.ParameterList.WithParameters(this.ParameterList.Parameters.AddRange(items)));
        }

        public LocalFunctionStatementSyntaxWrapper AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return this.WithConstraintClauses(this.ConstraintClauses.AddRange(items));
        }

        public LocalFunctionStatementSyntaxWrapper AddBodyStatements(params StatementSyntax[] items)
        {
            var body = this.Body ?? SyntaxFactory.Block();
            return this.WithBody(body.WithStatements(body.Statements.AddRange(items)));
        }
    }
}
