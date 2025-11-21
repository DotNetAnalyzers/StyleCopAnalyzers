// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp11.NamingRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.NamingRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    public partial class SA1300CSharp11UnitTests : SA1300CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPositionalRecord1()
        {
            // NOTE: Roslyn bug fix. Earlier versions made diagnostics be reported twice.
            return new[]
            {
                // /0/Test0.cs(2,15): warning SA1300: Element 'r' should begin with an uppercase letter
                Diagnostic().WithLocation(0).WithArguments("r"),
            };
        }
    }
}
