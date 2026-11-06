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

        protected override DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 37, 4, 46).WithArguments("TestClass.implicit operator TestClass()"),
                DiagnosticResult.CompilerError("CS1019").WithSpan(4, 46, 4, 46),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 46, 4, 47).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 46, 4, 47),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestLocalFunctionWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithSpan(6, 20, 6, 20).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(6, 20, 6, 20),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestOperatorWithMissingOperatorToken()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1019").WithSpan(4, 38, 4, 39),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 39, 4, 48).WithArguments("("),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestOperatorWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 38, 4, 39).WithArguments("TestClass.operator +()"),
                DiagnosticResult.CompilerError("CS1534").WithSpan(4, 38, 4, 39).WithArguments("+"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 39, 4, 40).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 39, 4, 40),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestDelegateWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 38, 4, 39).WithArguments("("),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 38, 4, 39),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestIndexerWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1551"),
                DiagnosticResult.CompilerError("CS0548").WithSpan(4, 16, 4, 20).WithArguments("TestClass.this"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("["),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("]"),
                DiagnosticResult.CompilerError("CS1014").WithSpan(4, 20, 4, 21),
                DiagnosticResult.CompilerError("CS1514").WithSpan(4, 20, 4, 21),
                DiagnosticResult.CompilerError("CS1513").WithSpan(5, 2, 5, 2),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestIndexerWithMissingOpenBracket()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 21, 4, 24).WithArguments("["),
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
