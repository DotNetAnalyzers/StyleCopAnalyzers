using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1407UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1407ArithmeticExpressionsMustDeclarePrecedence.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAdditionAndSubtraction()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 - 1 + 1 - 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMultiplicationAndDivision()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 / 1 * 1 / 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLeftShiftRightShift()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 >> 1 << 1 >> 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAdditionMultiplication()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + 1 * 1;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + (1 * 1);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestMultiplicationAddition()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 * 1 + 1;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) + 1;
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestAdditionMultiplicationParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 + (1 * 1);
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMultiplicationAdditionParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) * 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMultipleViolations()
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
                    this.CSharpDiagnostic().WithLocation(5, 17),
                    this.CSharpDiagnostic().WithLocation(5, 25)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) + (1 * 1);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestSubViolations()
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
                    this.CSharpDiagnostic().WithLocation(5, 22),
                    this.CSharpDiagnostic().WithLocation(5, 26)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1 << (1 + (1 * 1));
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestCodeFix()
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
                    this.CSharpDiagnostic().WithLocation(6, 17),
                    this.CSharpDiagnostic().WithLocation(6, 25),
                    this.CSharpDiagnostic().WithLocation(7, 21),
                    this.CSharpDiagnostic().WithLocation(7, 21),
                    this.CSharpDiagnostic().WithLocation(9, 24),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1407ArithmeticExpressionsMustDeclarePrecedence();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1407SA1408CodeFixProvider();
        }
    }
}