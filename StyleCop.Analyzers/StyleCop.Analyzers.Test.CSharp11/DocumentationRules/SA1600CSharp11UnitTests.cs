// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.DocumentationRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.DocumentationRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp11UnitTests : SA1600CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestTypeWithPrimaryConstructorWithoutDocumentation()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }
    }
}
