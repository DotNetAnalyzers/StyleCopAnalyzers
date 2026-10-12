// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.OrderingRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.OrderingRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1202CSharp11UnitTests : SA1202CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestMemberOrderInTypeWithPrimaryConstructor()
        {
            return new[]
            {
                Diagnostic().WithLocation(0).WithArguments("public", "private"),
            };
        }
    }
}
