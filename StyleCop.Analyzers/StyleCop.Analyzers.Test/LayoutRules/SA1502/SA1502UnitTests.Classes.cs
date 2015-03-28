namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the classes part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty class will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyClass()
        {
            var testCode = @"public class Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty class defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyClassOnSingleLine()
        {
            var testCode = "public class Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassOnSingleLine()
        {
            var testCode = "public class Foo { private int bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo
{ private int bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a class definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo { 
    private int bar; 
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an empty class element is working properly.
        /// </summary>
        [Fact]
        public async Task TestEmptyClassOnSingleLineCodeFix()
        {
            var testCode = "public class Foo { }";
            var fixedTestCode = @"public class Foo
{
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a class with a statement is working properly.
        /// </summary>
        [Fact]
        public async Task TestClassOnSingleLineCodeFix()
        {
            var testCode = "public class Foo { private int bar; }";
            var fixedTestCode = @"public class Foo
{
    private int bar;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a class with multiple statements is working properly.
        /// </summary>
        [Fact]
        public async Task TestClassOnSingleLineWithMultipleStatementsCodeFix()
        {
            var testCode = "public class Foo { private int bar; private bool baz; }";
            var fixedTestCode = @"public class Foo
{
    private int bar; private bool baz;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a class with its block defined on a single line is working properly.
        /// </summary>
        [Fact]
        public async Task TestClassWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{ private int bar; }";
            var fixedTestCode = @"public class Foo
{
    private int bar;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a class with lots of trivia is working properly.
        /// </summary>
        [Fact]
        public async Task TestClassWithLotsOfTriviaCodeFix()
        {
            var testCode = @"public class Foo /* TR1 */ { /* TR2 */ private int bar; /* TR3 */ private int baz; /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public class Foo /* TR1 */
{ /* TR2 */
    private int bar; /* TR3 */ private int baz; /* TR4 */
} /* TR5 */
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
