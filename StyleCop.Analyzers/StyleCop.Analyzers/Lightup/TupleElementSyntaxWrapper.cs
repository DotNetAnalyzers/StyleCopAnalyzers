// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct TupleElementSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public SyntaxToken Identifier
        {
            get
            {
                return IdentifierAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public TupleElementSyntaxWrapper WithIdentifier(SyntaxToken identifier)
        {
            return new TupleElementSyntaxWrapper(WithIdentifierAccessor(this.SyntaxNode, identifier));
        }

        public TupleElementSyntaxWrapper WithType(TypeSyntax type)
        {
            return new TupleElementSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
