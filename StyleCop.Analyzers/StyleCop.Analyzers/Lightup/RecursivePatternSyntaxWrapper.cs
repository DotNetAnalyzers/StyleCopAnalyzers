// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using Microsoft.CodeAnalysis.CSharp;

    internal partial struct RecursivePatternSyntaxWrapper : ISyntaxWrapper<CSharpSyntaxNode>
    {
        public RecursivePatternSyntaxWrapper AddPositionalPatternClauseSubpatterns(params SubpatternSyntaxWrapper[] items)
        {
            var positionalPatternClause = this.PositionalPatternClause;
            if (positionalPatternClause.SyntaxNode is null)
            {
                positionalPatternClause = SyntaxFactoryEx.PositionalPatternClause();
            }

            return this.WithPositionalPatternClause(positionalPatternClause.WithSubpatterns(positionalPatternClause.Subpatterns.AddRange(items)));
        }

        public RecursivePatternSyntaxWrapper AddPropertyPatternClauseSubpatterns(params SubpatternSyntaxWrapper[] items)
        {
            var propertyPatternClause = this.PropertyPatternClause;
            if (propertyPatternClause.SyntaxNode is null)
            {
                propertyPatternClause = SyntaxFactoryEx.PropertyPatternClause();
            }

            return this.WithPropertyPatternClause(propertyPatternClause.WithSubpatterns(propertyPatternClause.Subpatterns.AddRange(items)));
        }
    }
}
