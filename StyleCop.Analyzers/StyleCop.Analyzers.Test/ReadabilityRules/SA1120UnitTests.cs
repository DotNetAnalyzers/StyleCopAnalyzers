namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1120UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithSingleLineComment()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithSingleLineCommentOnLineWithCode()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Console.WriteLine(""A""); //
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 40);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationWithMultiLineCommentOnASingleLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        /* */
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

        }

        [Fact]
        public async Task TestViolationWithMultiLineCommentMultiLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        /* 

        */
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoViolationOnCommentedOutCodeBlocks()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        ////
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }


        [Fact]
        public async Task TestViolationCodeBlockFirstLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        //
        // Foo
        // Bar
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationCodeBlockLastLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // Bar
        // 
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestViolationCodeBlockFirstAndLastLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // 
        // Bar
        // 
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(8, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2}, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeBlockNoDiagnosticOnInternalBlankLines()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        // Foo
        // 
        //
        // Bar
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1120CommentsMustContainText();
        }
    }
}