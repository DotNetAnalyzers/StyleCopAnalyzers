using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    [TestClass]
    public class SA1408UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1408ConditionalExpressionsMustDeclarePrecedence.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOr()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false || true || false;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAnd()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false && true && false;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOrAndAnd()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false && true;
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Conditional expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 26)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false && true);
    }
}";

            await VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAndAndOr()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true;
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Conditional expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 18)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";

            await VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOrAndAndParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true || false) && true;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOrAndEqualsParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false == true);
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAndAndEquals()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false == true;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAndAndOrParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMultipleViolations()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true && false;
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Conditional expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 18)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Conditional expressions must declare precedence",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 35)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || (true && false);
    }
}";

            await VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCodeFix()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true && false;
        bool y = true || y && b && c;
        // the following test makes sure the code fix doesn't alter spacing
        bool z = z ? true&&true||false :false;
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || (true && false);
        bool y = true || (y && b && c);
        // the following test makes sure the code fix doesn't alter spacing
        bool z = z ? (true&&true)||false :false;
    }
}";

            await VerifyCSharpFixAsync(testCode, fixedCode);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1408ConditionalExpressionsMustDeclarePrecedence();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1407SA1408CodeFixProvider();
        }
    }
}