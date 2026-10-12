// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct BaseObjectCreationExpressionSyntaxWrapper : ISyntaxWrapper<ExpressionSyntax>
    {
        internal const string FallbackWrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax";

        public static implicit operator BaseObjectCreationExpressionSyntaxWrapper(ObjectCreationExpressionSyntax node)
        {
            return new BaseObjectCreationExpressionSyntaxWrapper(node);
        }
    }
}
