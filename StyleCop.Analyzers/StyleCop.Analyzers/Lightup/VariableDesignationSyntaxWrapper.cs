// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct VariableDesignationSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        static VariableDesignationSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(VariableDesignationSyntaxWrapper));
        }

        internal static VariableDesignationSyntaxWrapper FromUpcast(CSharpSyntaxNode node)
        {
            return new VariableDesignationSyntaxWrapper(node);
        }
    }
}
