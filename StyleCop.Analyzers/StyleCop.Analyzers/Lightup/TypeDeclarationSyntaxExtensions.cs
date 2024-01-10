// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup;

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class TypeDeclarationSyntaxExtensions
{
    private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax?> ParameterListAccessor;
    private static readonly Func<TypeDeclarationSyntax, ParameterListSyntax?, TypeDeclarationSyntax> WithParameterListAccessor;

    static TypeDeclarationSyntaxExtensions()
    {
        ParameterListAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax>(typeof(TypeDeclarationSyntax), nameof(ParameterList));
        WithParameterListAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<TypeDeclarationSyntax, ParameterListSyntax?>(typeof(TypeDeclarationSyntax), nameof(ParameterList));
    }

    public static ParameterListSyntax? ParameterList(this TypeDeclarationSyntax syntax)
    {
        return ParameterListAccessor(syntax);
    }

    public static TypeDeclarationSyntax WithParameterList(this TypeDeclarationSyntax syntax, ParameterListSyntax? parameterList)
    {
        return WithParameterListAccessor(syntax, parameterList);
    }
}
