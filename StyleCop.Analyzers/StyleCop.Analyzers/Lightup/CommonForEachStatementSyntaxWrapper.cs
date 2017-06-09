// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal struct CommonForEachStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        private const string CommonForEachStatementSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax";

        /// <summary>
        /// Prior to C# 7, <see cref="ForEachStatementSyntax"/> was the base type for all <c>foreach</c> statements. If
        /// the <c>CommonForEachStatementSyntax</c> type isn't found at runtime, we fall back to using this type
        /// instead.
        /// </summary>
        private const string ForEachStatementSyntaxTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax";

        private static readonly Type CommonForEachStatementSyntaxType;

        private static readonly Func<StatementSyntax, SyntaxToken> ForEachKeywordAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> OpenParenTokenAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> InKeywordAccessor;
        private static readonly Func<StatementSyntax, ExpressionSyntax> ExpressionAccessor;
        private static readonly Func<StatementSyntax, SyntaxToken> CloseParenTokenAccessor;
        private static readonly Func<StatementSyntax, StatementSyntax> StatementAccessor;

        private readonly StatementSyntax node;

        static CommonForEachStatementSyntaxWrapper()
        {
            CommonForEachStatementSyntaxType = typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(CommonForEachStatementSyntaxTypeName)
                ?? typeof(CSharpSyntaxNode).GetTypeInfo().Assembly.GetType(ForEachStatementSyntaxTypeName);
            ForEachKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(CommonForEachStatementSyntaxType, nameof(ForEachKeyword));
            OpenParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(CommonForEachStatementSyntaxType, nameof(OpenParenToken));
            InKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(CommonForEachStatementSyntaxType, nameof(InKeyword));
            ExpressionAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, ExpressionSyntax>(CommonForEachStatementSyntaxType, nameof(Expression));
            CloseParenTokenAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, SyntaxToken>(CommonForEachStatementSyntaxType, nameof(CloseParenToken));
            StatementAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<StatementSyntax, StatementSyntax>(CommonForEachStatementSyntaxType, nameof(Statement));
        }

        private CommonForEachStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;

        public SyntaxToken ForEachKeyword
        {
            get
            {
                return ForEachKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken OpenParenToken
        {
            get
            {
                return OpenParenTokenAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken InKeyword
        {
            get
            {
                return InKeywordAccessor(this.SyntaxNode);
            }
        }

        public ExpressionSyntax Expression
        {
            get
            {
                return ExpressionAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken CloseParenToken
        {
            get
            {
                return CloseParenTokenAccessor(this.SyntaxNode);
            }
        }

        public StatementSyntax Statement
        {
            get
            {
                return StatementAccessor(this.SyntaxNode);
            }
        }

        public static implicit operator CommonForEachStatementSyntaxWrapper(ForEachStatementSyntax node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }

        public static explicit operator CommonForEachStatementSyntaxWrapper(SyntaxNode node)
        {
            if (node == null)
            {
                return default(CommonForEachStatementSyntaxWrapper);
            }

            if (!IsInstance(node))
            {
                throw new InvalidCastException($"Cannot cast '{node.GetType().FullName}' to '{CommonForEachStatementSyntaxTypeName}'");
            }

            return new CommonForEachStatementSyntaxWrapper((StatementSyntax)node);
        }

        public static implicit operator StatementSyntax(CommonForEachStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, CommonForEachStatementSyntaxType);
        }

        internal static CommonForEachStatementSyntaxWrapper FromUpcast(StatementSyntax node)
        {
            return new CommonForEachStatementSyntaxWrapper(node);
        }
    }
}
