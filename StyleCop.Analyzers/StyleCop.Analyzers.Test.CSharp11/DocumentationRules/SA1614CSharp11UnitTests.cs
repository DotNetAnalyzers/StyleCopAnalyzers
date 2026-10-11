// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1614ElementParameterDocumentationMustHaveText>;

    public partial class SA1614CSharp11UnitTests : SA1614CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorParameterWithoutText()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
