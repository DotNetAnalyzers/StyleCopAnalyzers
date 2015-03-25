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
        /// Verifies that no diagnostics are reported for the valid fixed statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestFixedValid()
        {
            var testCode = @"public unsafe class Foo
{
    private void Bar()
    {
        int[] y = { 1, 2, 3 };

        // Valid fixed #1
        fixed (int* p = y)
        {
        }

        // Valid fixed #2
        fixed (int* p = y)
        {
            p[0] = 1;
        }

        // Valid fixed #3 (Valid only for SA1500)
        fixed (int* p = y) { }

        // Valid fixed #4 (Valid only for SA1500)
        fixed (int* p = y) { p[0] = 1; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid fixed statement definitions.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestFixedInvalid()
        {
            var testCode = @"public unsafe class Foo
{
    private void Bar()
    {
        int[] y = { 1, 2, 3 };

        // Invalid fixed #1
        fixed (int* p = y) {
        }

        // Invalid fixed #2
        fixed (int* p = y) {
            p[0] = 1;
        }

        // Invalid fixed #3
        fixed (int* p = y) {
            p[0] = 1; }

        // Invalid fixed #4
        fixed (int* p = y) { p[0] = 1;
        }

        // Invalid fixed #5
        fixed (int* p = y) 
        {
            p[0] = 1; }

        // Invalid fixed #6
        fixed (int* p = y) 
        { p[0] = 1;
        }

        // Invalid fixed #7
        fixed (int* p = y) 
        { p[0] = 1; }
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid fixed #1
                this.CSharpDiagnostic().WithLocation(8, 28),
                // Invalid fixed #2
                this.CSharpDiagnostic().WithLocation(12, 28),
                // Invalid fixed #3
                this.CSharpDiagnostic().WithLocation(17, 28),
                this.CSharpDiagnostic().WithLocation(18, 23),
                // Invalid fixed #4
                this.CSharpDiagnostic().WithLocation(21, 28),
                // Invalid fixed #5
                this.CSharpDiagnostic().WithLocation(27, 23),
                // Invalid fixed #6
                this.CSharpDiagnostic().WithLocation(31, 9),
                // Invalid fixed #7
                this.CSharpDiagnostic().WithLocation(36, 9),
                this.CSharpDiagnostic().WithLocation(36, 21)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}