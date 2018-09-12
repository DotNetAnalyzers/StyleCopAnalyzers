// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;

    public class SA1022UnitTests : NumberSignSpacingTestBase
    {
        protected override string Sign
        {
            get
            {
                return "+";
            }
        }

        protected override DiagnosticAnalyzer Analyzer => new SA1022PositiveSignsMustBeSpacedCorrectly();

        protected override CodeFixProvider CodeFix => new TokenSpacingCodeFixProvider();
    }
}
