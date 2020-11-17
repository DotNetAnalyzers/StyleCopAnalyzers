// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal partial struct SubpatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        private static readonly Func<CSharpSyntaxNode, NameColonSyntax> NameColonAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode> PatternAccessor;

        private static readonly Func<CSharpSyntaxNode, NameColonSyntax, CSharpSyntaxNode> WithNameColonAccessor;
        private static readonly Func<CSharpSyntaxNode, CSharpSyntaxNode, CSharpSyntaxNode> WithPatternAccessor;

        static SubpatternSyntaxWrapper()
        {
            WrappedType = SyntaxWrapperHelper.GetWrappedType(typeof(SubpatternSyntaxWrapper));
            NameColonAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, NameColonSyntax>(WrappedType, nameof(NameColon));
            PatternAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));

            WithNameColonAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, NameColonSyntax>(WrappedType, nameof(NameColon));
            WithPatternAccessor = LightupHelpers.CreateSyntaxWithPropertyAccessor<CSharpSyntaxNode, CSharpSyntaxNode>(WrappedType, nameof(Pattern));
        }

        public NameColonSyntax NameColon
        {
            get
            {
                return NameColonAccessor(this.SyntaxNode);
            }
        }

        public PatternSyntaxWrapper Pattern
        {
            get
            {
                return (PatternSyntaxWrapper)PatternAccessor(this.SyntaxNode);
            }
        }

        public SubpatternSyntaxWrapper WithNameColon(NameColonSyntax nameColon)
        {
            return new SubpatternSyntaxWrapper(WithNameColonAccessor(this.SyntaxNode, nameColon));
        }

        public SubpatternSyntaxWrapper WithPattern(PatternSyntaxWrapper pattern)
        {
            return new SubpatternSyntaxWrapper(WithPatternAccessor(this.SyntaxNode, pattern));
        }
    }
}
