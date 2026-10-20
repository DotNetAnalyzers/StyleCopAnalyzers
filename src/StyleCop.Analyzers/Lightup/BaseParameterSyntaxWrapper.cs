// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup;

using Microsoft.CodeAnalysis.CSharp.Syntax;

internal partial struct BaseParameterSyntaxWrapper
{
    internal const string FallbackWrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax";

    public static implicit operator BaseParameterSyntaxWrapper(ParameterSyntax? node)
    {
        return new BaseParameterSyntaxWrapper(node);
    }
}
