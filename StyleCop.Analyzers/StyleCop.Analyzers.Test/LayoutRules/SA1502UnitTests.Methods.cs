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
        [Fact]
        public async Task TestValidEmptyMethod()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyMethodOnSingleLine()
        {
            var testCode = @"public class Foo
{
    public void Bar() { }
}";
            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestMethodOnSingleLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar() { return false; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestMethodWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar() 
    { return false; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestMethodWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar() {
        return false; }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a method with an expression body will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestMethodWithExpressionBody()
        {
            var testCode = @"public class Foo
{
    public bool Bar(int x, int y) => x > y;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
