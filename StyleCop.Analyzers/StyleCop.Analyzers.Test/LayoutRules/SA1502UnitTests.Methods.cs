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
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public void Bar()
    {
    }
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyMethodOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public void Bar() { }
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public bool Bar() { return false; }
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithBlockOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public bool Bar() 
    { return false; }
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithBlockStartOnSameLine(string elementType)
        {
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public bool Bar() {
        return false; }
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with an expression body will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestMethodWithExpressionBody(string elementType)
        {
            var testCodeFormat = @"public ##TOKEN## Foo
{
    public bool Bar(int x, int y) => x > y;
}";
            var testCode = testCodeFormat.Replace("##TOKEN##", elementType);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
