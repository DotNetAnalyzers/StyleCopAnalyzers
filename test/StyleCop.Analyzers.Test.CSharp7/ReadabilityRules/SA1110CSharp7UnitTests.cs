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
        StyleCop.Analyzers.ReadabilityRules.SA1110OpeningParenthesisMustBeOnDeclarationLine,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1110CSharp7UnitTests : SA1110UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningParenthesisInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar
                        ()
        {

        }
    }
}";
            var fixedCode = @"
class Foo
{
    public void Method()
    {
        void Bar()
        {

        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 25);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningParenthesisInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void 
                        Bar()
        {

        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionWithMissingNameAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void {|CS1001:<|}T>(T value)
        {
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionWithMissingParameterListAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar<T>{|CS1003:|}{|CS1026:|}
        {
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingType()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(5, 9, 5, 26).WithArguments("TestClass.implicit operator (TestClass value, ?)()"),
                DiagnosticResult.CompilerError("CS8124").WithSpan(5, 25, 5, 26),
                DiagnosticResult.CompilerError("CS1001").WithSpan(5, 26, 5, 26),
                DiagnosticResult.CompilerError("CS1003").WithSpan(5, 26, 5, 26).WithArguments("(", "{"),
                DiagnosticResult.CompilerError("CS1019").WithSpan(6, 1, 8, 1),
                DiagnosticResult.CompilerError("CS1002").WithSpan(7, 18, 7, 18),
                DiagnosticResult.CompilerError("CS1026").WithSpan(7, 18, 7, 18),
                DiagnosticResult.CompilerError("CS1022").WithSpan(9, 1, 9, 2),
            };
        }

        protected override DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 37, 4, 46).WithArguments("TestClass.implicit operator TestClass()"),
                DiagnosticResult.CompilerError("CS1001").WithSpan(4, 46, 4, 47),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 46, 4, 47).WithArguments("(", ";"),
                DiagnosticResult.CompilerError("CS1019").WithSpan(4, 46, 5, 1),
                DiagnosticResult.CompilerError("CS1002").WithSpan(4, 47, 4, 47),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 47, 4, 47),
            };
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
