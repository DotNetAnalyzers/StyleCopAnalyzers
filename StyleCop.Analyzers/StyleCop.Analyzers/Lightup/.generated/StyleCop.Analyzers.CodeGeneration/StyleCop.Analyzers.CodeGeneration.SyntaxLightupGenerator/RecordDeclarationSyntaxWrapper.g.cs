// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct RecordDeclarationSyntaxWrapper : ISyntaxWrapper<TypeDeclarationSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax";
        private static readonly Type WrappedType;
        private readonly TypeDeclarationSyntax node;
        private RecordDeclarationSyntaxWrapper(TypeDeclarationSyntax node)
        {
            this.node = node;
        }

        public TypeDeclarationSyntax SyntaxNode => this.node;
    }
}
