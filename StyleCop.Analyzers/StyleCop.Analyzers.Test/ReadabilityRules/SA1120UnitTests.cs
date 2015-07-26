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
        public async Task TestViolationWithSingleLineCommentAsync()
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
        public async Task TestViolationWithSingleLineCommentOnLineWithCodeAsync()
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
        public async Task TestViolationWithMultiLineCommentOnASingleLineAsync()
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
        public async Task TestViolationWithMultiLineCommentMultiLineAsync()
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
        public async Task TestNoViolationOnCommentedOutCodeBlocksAsync()
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
        public async Task TestViolationCodeBlockFirstLineAsync()
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
        public async Task TestViolationCodeBlockLastLineAsync()
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
        public async Task TestViolationCodeBlockFirstAndLastLineAsync()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeBlockNoDiagnosticOnInternalBlankLinesAsync()
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
