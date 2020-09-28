// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1407ArithmeticExpressionsMustDeclarePrecedence,
        StyleCop.Analyzers.MaintainabilityRules.SA1407SA1408CodeFixProvider>;

    public class SA1407UnitTests
    {
        [Fact]
        public async Task TestAdditionAndSubtractionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 - 1 + 1 - 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiplicationAndDivisionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 / 1 * 1 / 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLeftShiftRightShiftAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 >> 1 << 1 >> 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAdditionMultiplicationAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + 1 * 1;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 21);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + (1 * 1);
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiplicationAdditionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 * 1 + 1;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(5, 17);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) + 1;
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAdditionMultiplicationParenthesizedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + (1 * 1);
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiplicationAdditionParenthesizedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) * 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
