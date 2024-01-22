// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal partial struct RecordDeclarationSyntaxWrapper : ISyntaxWrapper<TypeDeclarationSyntax>
{
    public static implicit operator RecordDeclarationSyntaxWrapper(TypeDeclarationSyntax node)
    {
        return new RecordDeclarationSyntaxWrapper(node);
    }
}
