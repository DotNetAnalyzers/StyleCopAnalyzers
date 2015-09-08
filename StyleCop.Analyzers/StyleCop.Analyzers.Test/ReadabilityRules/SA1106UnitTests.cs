namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1106UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptyStatementAsBlockAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
            ;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
        {
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementWithinBlockAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
        {
            var temp = i;
            ;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
        {
            var temp = i;
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementInForStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (;;)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        ;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementFollowedByEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        ;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementFollowedByNonEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        int x = 3;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        int x = 3;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class Foo { }")]
        [InlineData("struct Foo { }")]
        [InlineData("interface IFoo { }")]
        [InlineData("enum Foo { }")]
        [InlineData("namespace Foo { }")]
        public async Task TestMemberAsync(string declaration)
        {
            var testCode = declaration + @"
;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1106CodeFixProvider();
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1106CodeMustNotContainEmptyStatements();
        }
    }
}
