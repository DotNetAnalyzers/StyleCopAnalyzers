// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal readonly partial struct CasePatternSwitchLabelSyntaxWrapper : ISyntaxWrapper<SwitchLabelSyntax>
    {
        internal const string WrappedTypeName = "Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax";
        private static readonly Type WrappedType;
        private readonly SwitchLabelSyntax node;
        private CasePatternSwitchLabelSyntaxWrapper(SwitchLabelSyntax node)
        {
            this.node = node;
        }

        public SwitchLabelSyntax SyntaxNode => this.node;
    }
}
