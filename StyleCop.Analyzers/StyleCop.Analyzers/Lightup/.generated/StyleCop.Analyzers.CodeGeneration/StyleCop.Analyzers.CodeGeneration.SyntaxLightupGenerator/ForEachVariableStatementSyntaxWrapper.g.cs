// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct ForEachVariableStatementSyntaxWrapper : ISyntaxWrapper<StatementSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax";
        private static readonly Type WrappedType;
        private readonly StatementSyntax node;
        private ForEachVariableStatementSyntaxWrapper(StatementSyntax node)
        {
            this.node = node;
        }

        public StatementSyntax SyntaxNode => this.node;
        public static implicit operator StatementSyntax(ForEachVariableStatementSyntaxWrapper wrapper)
        {
            return wrapper.node;
        }

        public static bool IsInstance(SyntaxNode node)
        {
            return node != null && LightupHelpers.CanWrapNode(node, WrappedType);
        }
    }
}
