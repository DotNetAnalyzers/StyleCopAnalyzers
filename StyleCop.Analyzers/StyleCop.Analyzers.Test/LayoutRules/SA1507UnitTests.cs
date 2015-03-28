namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1507CodeMustNotContainMultipleBlankLinesInARow"/>.
    /// </summary>
    public class SA1507UnitTests : CodeFixVerifier
    {
        private const string TestCode = @"public class Foo
{


    using System;


    using System.Collections;

    void Bar()
    {


#if !IGNORE


        return;
#else


        return;
#endif


/*


*/
    }
}
";

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that empty lines at the start of the file do not trigger any diagnostics.
        /// (This will be handled by SA1517)
        /// </summary>
        [Fact]
        public async Task TestEmptyLinesAtStartOfFile()
        {
            var testCode = @"

public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that empty lines at the end of the file do not trigger any diagnostics.
        /// (This will be handled by SA1518)
        /// </summary>
        [Fact]
        public async Task TestEmptyLinesAtEndOfFile()
        {
            var testCode = @"public class Foo
{
}


";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a verbatim string literal does not trigger any diagnostics.
        /// (This will be handled by SA1518)
        /// </summary>
        [Fact]
        public async Task TestVerbatimStringLiteral()
        {
            var testCode = @"public class Foo
{
    private string testCode = @""namespace Bar
{


    using System;
}
"";

}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Validate that all invalid multiple blank lines are reported.
        /// </summary>
        [Fact]
        public async Task TestInvalidMultipleBlankLines()
        {
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(12, 1),
                this.CSharpDiagnostic().WithLocation(15, 1),
                /* line 19 should not report a diagnostic, as it's part of the directive is inactive */
                this.CSharpDiagnostic().WithLocation(23, 1)
                /* line 26 should not report a diagnostic, as it's part of a comment */
            };

            await this.VerifyCSharpDiagnosticAsync(TestCode, expectedDiagnostics, CancellationToken.None);
        }

        /// <summary>
        /// Validate that the code fixes for all invalid multiple blank lines works properly.
        /// </summary>
        [Fact]
        public async Task TestInvalidMultipleBlankLinesCodeFix()
        {
            var fixedTestCode = @"public class Foo
{

    using System;

    using System.Collections;

    void Bar()
    {

#if !IGNORE

        return;
#else


        return;
#endif

/*


*/
    }
}
";

            await this.VerifyCSharpFixAsync(TestCode, fixedTestCode);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1507CodeMustNotContainMultipleBlankLinesInARow();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1507CodeFixProvider();
        }
    }
}
