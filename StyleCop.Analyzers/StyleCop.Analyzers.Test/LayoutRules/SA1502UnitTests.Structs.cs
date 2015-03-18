namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the structs part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty class will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyStruct()
        {
            var testCode = @"public struct Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty struct defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyStructOnSingleLine()
        {
            var testCode = "public struct Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 19);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructOnSingleLine()
        {
            var testCode = "public struct Foo { public int Bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 19);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructWithBlockOnSingleLine()
        {
            var testCode = @"public struct Foo
{ public int Bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructWithBlockStartOnSameLine()
        {
            var testCode = @"public struct Foo { 
    public int Bar; 
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix for an empty struct element is working properly.
        /// </summary>
        [Fact]
        public async Task TestEmptyStructOnSingleLineCodeFix()
        {
            var testCode = "public struct Foo { }";
            var fixedTestCode = @"public struct Foo
{
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a struct with a statement is working properly.
        /// </summary>
        [Fact]
        public async Task TestStructOnSingleLineCodeFix()
        {
            var testCode = "public struct Foo { private int bar; }";
            var fixedTestCode = @"public struct Foo
{
    private int bar;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a struct with multiple statements is working properly.
        /// </summary>
        [Fact]
        public async Task TestStructOnSingleLineWithMultipleStatementsCodeFix()
        {
            var testCode = "public struct Foo { private int bar; private bool baz; }";
            var fixedTestCode = @"public struct Foo
{
    private int bar; private bool baz;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a struct with its block defined on a single line is working properly.
        /// </summary>
        [Fact]
        public async Task TestStructWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public struct Foo
{ private int bar; }";
            var fixedTestCode = @"public struct Foo
{
    private int bar;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a struct with lots of trivia is working properly.
        /// </summary>
        [Fact]
        public async Task TestStructWithLotsOfTriviaCodeFix()
        {
            var testCode = @"public struct Foo /* TR1 */ { /* TR2 */ private int bar; /* TR3 */ private int baz; /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public struct Foo /* TR1 */
{ /* TR2 */
    private int bar; /* TR3 */ private int baz; /* TR4 */
} /* TR5 */
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }
    }
}
