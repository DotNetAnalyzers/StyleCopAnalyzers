// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct BaseNamespaceDeclarationSyntaxWrapper : ISyntaxWrapper<MemberDeclarationSyntax>
    {
        internal const string FallbackWrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax";

        public static implicit operator BaseNamespaceDeclarationSyntaxWrapper(NamespaceDeclarationSyntax node)
        {
            return new BaseNamespaceDeclarationSyntaxWrapper(node);
        }
    }
}
