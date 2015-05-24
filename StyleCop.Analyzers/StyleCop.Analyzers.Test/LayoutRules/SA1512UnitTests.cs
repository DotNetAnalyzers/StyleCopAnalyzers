namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1512SingleLineCommentsMustNotBeFollowedByBlankLine"/>
    /// </summary>
    public class SA1512UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that all known types valid single line comment lines will not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSingleLineCommentsAsync()
        {
            var testCode = @"// A single line comment at the start of the file is valid
namespace Foo
{
    public class Bar
    {
        // A single line comment at the start of the scope is valid
        private int field1;

        // This is valid as well ofcourse
        private int field2;
        private int field3; // This should not trigger ofcourse

#if (SPECIALTEST)
        // This is allowed because the statement is disabled by the directive

        private int field4;
#else
        // this is also allowed
        private double field4;
#endif

        // Two single line comments separated by

        // a single empty line are valid ofcourse
        private int field5;

        // Multiple single line comments
        // directly after each other are valid as well
        public int Baz()
        {
            var x = field1;

            ////return 0;

            return x;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid single line comment lines will produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSingleLineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid

        private int field2;

        public int Baz(int x)
        {
            // invalid as well

            return x;
        }
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid
        private int field2;

        public int Baz(int x)
        {
            // invalid as well
            return x;
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(11, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that single line comment followed by a blank line with spurious whitespace will produce the expected diagnostic and gets fixed correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSingleLineCommentWithSpuriousWhitespaceAsync()
        {
            var testCode =
                "namespace Foo\r\n" +
                "{\r\n" +
                "    public class Bar\r\n" +
                "    {\r\n" +
                "        // This is invalid\r\n" +
                "        \r\n" +
                "        private int field2;\r\n" +
                "    }\r\n" +
                "}\r\n";

            var fixedTestCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid
        private int field2;
    }
}
";

            DiagnosticResult[] expectedDiagnostic =
            {
                this.CSharpDiagnostic().WithLocation(5, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line comment followed by multiple blank lines will not produce the correct diagnostic when SA1507 is enabled or not.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <remarks>This cannot be tested properly until #522 is available, for now only situation where SA1507 is enabled is tested.</remarks>
        [Fact]
        public async Task TestSingleLineCommentFollowedByMultipleBlankLinesAsync()
        {
            var testCode = @"namespace Foo
{
    public class Bar
    {
        // This is invalid


        private int field2;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1512SingleLineCommentsMustNotBeFollowedByBlankLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1512CodeFixProvider();
        }
    }
}
