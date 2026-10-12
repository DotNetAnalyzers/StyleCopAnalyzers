// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.LayoutRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.LayoutRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1505OpeningBracesMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1505CodeFixProvider>;

    public partial class SA1505CSharp11UnitTests : SA1505CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestBlankLineAfterOpeningBraceInTypeWithPrimaryConstructor()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
