// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.LayoutRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Test.CSharp10.LayoutRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1516ElementsMustBeSeparatedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1516CodeFixProvider>;

    public partial class SA1516UsingGroupsCSharp11UnitTests : SA1516UsingGroupsCSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestBlankLineRequiredBetweenGlobalAndLocalUsingGroups()
        {
            // NOTE: Roslyn bug fix. Earlier versions made diagnostics be reported twice.
            return new[]
            {
                // /0/Test0.cs(4,1): warning SA1516: Using directives should be separated by blank line
                Diagnostic(SA1516ElementsMustBeSeparatedByBlankLine.DescriptorRequire).WithLocation(0),
            };
        }
    }
}
