// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1629DocumentationTextMustEndWithAPeriod,
        StyleCop.Analyzers.DocumentationRules.SA1629CodeFixProvider>;

    public partial class SA1629CSharp11UnitTests : SA1629CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestTypeWithPrimaryConstructorSummaryWithoutPeriod()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
