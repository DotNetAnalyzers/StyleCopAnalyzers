// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct ImplicitStackAllocArrayCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithStackAllocKeyword(SyntaxToken stackAllocKeyword)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithStackAllocKeywordAccessor(this.SyntaxNode, stackAllocKeyword));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithOpenBracketToken(SyntaxToken openBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithOpenBracketTokenAccessor(this.SyntaxNode, openBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithCloseBracketToken(SyntaxToken closeBracketToken)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithCloseBracketTokenAccessor(this.SyntaxNode, closeBracketToken));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper WithInitializer(InitializerExpressionSyntax initializer)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(WithInitializerAccessor(this.SyntaxNode, initializer));
        }

        public ImplicitStackAllocArrayCreationExpressionSyntaxWrapper AddInitializerExpressions(params ExpressionSyntax[] items)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntaxWrapper(this.WithInitializer(this.Initializer.AddExpressions(items)));
        }
    }
}
