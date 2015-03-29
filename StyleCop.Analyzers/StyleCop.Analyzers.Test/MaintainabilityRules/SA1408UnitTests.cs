using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1408UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1408ConditionalExpressionsMustDeclarePrecedence.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOr()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false || true || false;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnd()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false && true && false;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOrAndAnd()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || false && true;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 26);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false && true);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestAndAndOr()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false || true;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestOrAndAndParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true || false) && true;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOrAndEqualsParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true || (false == true);
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAndAndEquals()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = true && false == true;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAndAndOrParenthesized()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || true;
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
        bool x = true && false || true && false;
    }
}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(5, 18),
                    this.CSharpDiagnostic().WithLocation(5, 35)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = (true && false) || (true && false);
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
        bool b = true, c = true;
        bool x = true && false || true && false;
        bool y = true || x && b && c;
        // the following test makes sure the code fix doesn't alter spacing
        bool z = b ? true&&true||false :false;
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool b = true, c = true;
        bool x = (true && false) || (true && false);
        bool y = true || (x && b && c);
        // the following test makes sure the code fix doesn't alter spacing
        bool z = b ? (true&&true)||false :false;
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 18),
                    this.CSharpDiagnostic().WithLocation(6, 35),
                    this.CSharpDiagnostic().WithLocation(7, 26),
                    this.CSharpDiagnostic().WithLocation(9, 22),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            await this.VerifyCSharpFixAsync(testCode, fixedCode);
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