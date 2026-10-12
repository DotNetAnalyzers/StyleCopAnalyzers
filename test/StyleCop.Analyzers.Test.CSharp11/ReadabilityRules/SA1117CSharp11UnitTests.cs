// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public partial class SA1117CSharp11UnitTests : SA1117CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestInvalidPrimaryConstructor()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
