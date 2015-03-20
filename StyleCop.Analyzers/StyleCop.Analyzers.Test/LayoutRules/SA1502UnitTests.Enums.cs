namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the enums part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty enum will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyEnum()
        {
            var testCode = @"public enum Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty enum defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyEnumOnSingleLine()
        {
            var testCode = "public enum Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumOnSingleLine()
        {
            var testCode = "public enum Foo { Value1 }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumWithBlockOnSingleLine()
        {
            var testCode = @"public enum Foo
{ Value1 }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumWithBlockStartOnSameLine()
        {
            var testCode = @"public enum Foo { 
Value1 }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty enum defined on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestEmptyEnumOnSingleLineCodeFix()
        {
            var testCode = "public enum Foo { }";
            var fixedTestCode = @"public enum Foo
{
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestEnumOnSingleLineCodeFix()
        {
            var testCode = "public enum Foo { Value1 }";
            var fixedTestCode = @"public enum Foo
{
    Value1
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition with multiple values on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestMultiValueEnumOnSingleLineCodeFix()
        {
            var testCode = "public enum Foo { Value1, Value2 }";
            var fixedTestCode = @"public enum Foo
{
    Value1, Value2
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum with its block defined on a single line will properly.
        /// </summary>
        [Fact]
        public async Task TestEnumWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public enum Foo
{ Value1 }";
            var fixedTestCode = @"public enum Foo
{
    Value1
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition with lots of trivia will work properly.
        /// </summary>
        [Fact]
        public async Task TestEnumWithLotsOfTriviaCodeFix()
        {
            var testCode = "public enum Foo /* TR1 */ { /* TR2 */ Value1, /* TR3 */ Value2 /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public enum Foo /* TR1 */
{ /* TR2 */
    Value1, /* TR3 */ Value2 /* TR4 */
} /* TR5 */
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
