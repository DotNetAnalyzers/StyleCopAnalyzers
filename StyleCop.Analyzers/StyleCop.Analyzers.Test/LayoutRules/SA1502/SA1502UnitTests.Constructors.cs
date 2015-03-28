namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the constructors part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a valid constructor will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestValidEmptyConstructor(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyConstructorOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() { }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 18);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 18);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithBlockOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() 
    { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithBlockStartOnSameLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() { 
        int bar; }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty constructor with its block on the same line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyConstructorOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() { }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo()
    {
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with its block on the same line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() { int bar; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo()
    {
        int bar;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with its block on a single line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithBlockOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() 
    { int bar; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo()
    {
        int bar;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with lots of trivia will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithLotsOfTriviaCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo() /* TR1 */ { /* TR2 */ int bar; /* TR3 */ } /* TR4 */
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo() /* TR1 */
    { /* TR2 */
        int bar; /* TR3 */
    } /* TR4 */
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }
    }
}
