// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1112CSharp11UnitTests : SA1112CSharp10UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultTestPrimaryConstructorWithNoParametersClosingParenthesisOnTheNextLine()
        {
            return new[]
            {
                Diagnostic().WithLocation(0),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestMethodWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 17, 4, 27).WithArguments("TestClass.TestMethod<T>()"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 30, 4, 31).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 30, 4, 31),
            };
        }
    }
}
