namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid block defined in this test.
        /// </summary>
        /// <remarks>
        /// These blocks are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestValidBlocks()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        // valid block #1
        {
        }

        // valid block #2
        {
            int a;
        }

        // valid block #3
        { }

        // valid block #4
        { int b; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that not having the opening curly bracket on its own line will report a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestInvalidOpeningCurlyBracket()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        { int a;
        }
    }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(5, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that not having the closing curly bracket on its own line will report a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestInvalidClosingCurlyBracket()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        { 
            int a; }
    }
}";
            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(6, 20);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
