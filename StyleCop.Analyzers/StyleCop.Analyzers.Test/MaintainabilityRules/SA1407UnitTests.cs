using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;

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
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 21)
                        }
                }
            };

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
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 17)
                        }
                }
            };

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
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 17)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 25)
                        }
                }
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
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 22)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Arithmetic expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 26)
                        }
                }
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
        int x = 1 * 1 + 1 * 1;
        int y = 5 + y * b / 6 % z - 2;
        // the following test makes sure the code fix doesn't alter spacing
        int z = z ? 4*3+-1 :false;
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1 * 1) + (1 * 1);
        int y = 5 + ((y * b / 6) % z) - 2;
        // the following test makes sure the code fix doesn't alter spacing
        int z = z ? (4*3)+-1 :false;
    }
}";

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