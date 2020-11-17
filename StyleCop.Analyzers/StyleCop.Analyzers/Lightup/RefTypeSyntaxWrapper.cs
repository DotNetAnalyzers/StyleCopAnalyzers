// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct RefTypeSyntaxWrapper : ISyntaxWrapper<TypeSyntax>
    {
        static RefTypeSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(RefTypeSyntaxWrapper));
            RefKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(RefKeyword));
            ReadOnlyKeywordAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ReadOnlyKeyword));
            TypeAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
            WithRefKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(RefKeyword));
            WithReadOnlyKeywordAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, SyntaxToken>(WrappedType, nameof(ReadOnlyKeyword));
            WithTypeAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeSyntax, TypeSyntax>(WrappedType, nameof(Type));
        }

        public SyntaxToken RefKeyword
        {
            get
            {
                return RefKeywordAccessor(this.SyntaxNode);
            }
        }

        public SyntaxToken ReadOnlyKeyword
        {
            get
            {
                return ReadOnlyKeywordAccessor(this.SyntaxNode);
            }
        }

        public TypeSyntax Type
        {
            get
            {
                return TypeAccessor(this.SyntaxNode);
            }
        }

        public RefTypeSyntaxWrapper WithRefKeyword(SyntaxToken refKeyword)
        {
            return new RefTypeSyntaxWrapper(WithRefKeywordAccessor(this.SyntaxNode, refKeyword));
        }

        public RefTypeSyntaxWrapper WithReadOnlyKeyword(SyntaxToken readOnlyKeyword)
        {
            return new RefTypeSyntaxWrapper(WithReadOnlyKeywordAccessor(this.SyntaxNode, readOnlyKeyword));
        }

        public RefTypeSyntaxWrapper WithType(TypeSyntax type)
        {
            return new RefTypeSyntaxWrapper(WithTypeAccessor(this.SyntaxNode, type));
        }
    }
}
