// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1110OpeningParenthesisMustBeOnDeclarationLine,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1110CSharp11UnitTests : SA1110CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructor()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
