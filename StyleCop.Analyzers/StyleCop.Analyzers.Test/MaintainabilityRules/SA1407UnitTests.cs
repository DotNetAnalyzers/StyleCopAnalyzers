// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1407ArithmeticExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    public class SA1407UnitTests
    {
        [Theory]
        [InlineData("+", "+", "+")]
        [InlineData("-", "+", "-")]
        [InlineData("-", "-", "-")]
        [InlineData("+", "-", "+")]
        public async Task TestAdditiveAsync(string op1, string op2, string op3)
        {
            var testCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} 1 {op2} 1 {op3} 1;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("*", "*", "*")]
        [InlineData("/", "*", "/")]
        [InlineData("/", "/", "/")]
        [InlineData("*", "/", "*")]
        public async Task TestMultiplicativeAsync(string op1, string op2, string op3)
        {
            var testCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} 1 {op2} 1 {op3} 1;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(">>", ">>", ">>")]
        [InlineData("<<", ">>", "<<")]
        [InlineData("<<", "<<", "<<")]
        [InlineData(">>", "<<", ">>")]
        public async Task TestShiftAsync(string op1, string op2, string op3)
        {
            var testCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} 1 {op2} 1 {op3} 1;
    }}
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("+", "*")]
        [InlineData("+", "/")]
        [InlineData("-", "*")]
        [InlineData("-", "/")]
        public async Task TestAdditiveMultiplicativeAsync(string op1, string op2)
        {
            var testCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} 1 {op2} 1;
    }}
}}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 21);

            var fixedCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} (1 {op2} 1);
    }}
}}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("*", "+")]
        [InlineData("*", "-")]
        [InlineData("/", "+")]
        [InlineData("/", "-")]
        public async Task TestMultiplicativeAdditiveAsync(string op1, string op2)
        {
            var testCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = 1 {op1} 1 {op2} 1;
    }}
}}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 17);

            var fixedCode = $@"public class Foo
{{
    public void Bar()
    {{
        int x = (1 {op1} 1) {op2} 1;
    }}
}}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleViolationsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 * 1 + 1 * 1;
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(5, 17),
                    Diagnostic().WithLocation(5, 25),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) + (1 * 1);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSubViolationsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 << 1 + 1 * 1;
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(5, 22),
                    Diagnostic().WithLocation(5, 26),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 << (1 + (1 * 1));
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int b = 1;
        int x = 1 * 1 + 1 * 1;
        int y = 5 + x * b / 6 % x - 2;
        // the following test makes sure the code fix doesn't alter spacing
        int z = y==1 ? 4*3+-1 :0;
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int b = 1;
        int x = (1 * 1) + (1 * 1);
        int y = 5 + ((x * b / 6) % x) - 2;
        // the following test makes sure the code fix doesn't alter spacing
        int z = y==1 ? (4*3)+-1 :0;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 17),
                    Diagnostic().WithLocation(6, 25),
                    Diagnostic().WithLocation(7, 21),
                    Diagnostic().WithLocation(7, 21),
                    Diagnostic().WithLocation(9, 24),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
