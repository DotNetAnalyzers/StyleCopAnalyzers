// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1111ClosingParenthesisMustBeOnLineOfLastParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1111CSharp7UnitTests : SA1111UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationWithOneParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s
)
        {

        }
    }
}";
            var fixedCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s)
        {

        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithThreeParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s,
                        int i,
                        object o
)
        {

        }
    }
}";
            var fixedCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s,
                        int i,
                        object o)
        {

        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithNoParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(
)
        {

        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithOneParameterClosingParenthesisOnTheSameLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s)
        {

        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultTestIndexerWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1551"),
                DiagnosticResult.CompilerError("CS0548").WithSpan(4, 16, 4, 20).WithArguments("TestClass.this"),
                DiagnosticResult.CompilerError("CS1001").WithSpan(4, 20, 4, 21),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("[", ";"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 21, 4, 21).WithArguments("]", "}"),
                DiagnosticResult.CompilerError("CS1514").WithSpan(4, 21, 4, 21),
                DiagnosticResult.CompilerError("CS1513").WithSpan(5, 2, 5, 2),
            };
        }
    }
}
