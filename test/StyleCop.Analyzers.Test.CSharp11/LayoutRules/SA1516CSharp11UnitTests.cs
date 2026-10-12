// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.LayoutRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.LayoutRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public partial class SA1516CSharp11UnitTests : SA1516CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestUsingAndGlobalStatementSpacingInTopLevelProgram()
        {
            // NOTE: Roslyn bug fix. Earlier versions made diagnostics be reported twice.
            return new[]
            {
                // /0/Test0.cs(3,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestGlobalStatementAndRecordSpacingInTopLevelProgram()
        {
            // NOTE: Roslyn bug fix. Earlier versions made diagnostics be reported twice.
            return new[]
            {
                // /0/Test0.cs(2,1): warning SA1516: Elements should be separated by blank line
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTopLevelStatementsFollowedByType()
        {
            // NOTE: Roslyn bug fix. Earlier versions made diagnostics be reported twice.
            return new[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };
        }
    }
}
