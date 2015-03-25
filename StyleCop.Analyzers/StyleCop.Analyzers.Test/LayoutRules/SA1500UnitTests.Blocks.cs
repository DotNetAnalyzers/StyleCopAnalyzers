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
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestBlockValid()
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

        // valid block #3 (valid only for SA1500)
        { }

        // valid block #4 (valid only for SA1500)
        { int b; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid blocks.
        /// </summary>
        [Fact]
        public async Task TestBlockInvalid()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        // invalid block #1
        { int a;
        }

        // invalid block #2
        { 
            int a; }
    }
}";
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(11, 20)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
