namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the methods part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a valid method will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestValidEmptyMethod(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyMethodOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar() { }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() { return false; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithBlockOnSingleLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() 
    { return false; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithBlockStartOnSameLine(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() {
        return false; }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with an expression body will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithExpressionBody(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar(int x, int y) => x > y;
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty method with its block on the same line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyMethodOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar() { }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a method with its block on the same line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() { return false; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar()
    {
        return false;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a method with its block on a single line will work properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithBlockOnSingleLineCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() 
    { return false; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar()
    {
        return false;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a property with lots of trivia is working properly.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithLotsOfTriviaCodeFix(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() /* TR1 */ { /* TR2 */ return false; /* TR3 */ } /* TR4 */
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar() /* TR1 */
    { /* TR2 */
        return false; /* TR3 */
    } /* TR4 */
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }
    }
}
