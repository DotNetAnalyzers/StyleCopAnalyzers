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
        /// Verifies that no diagnostics are reported for the valid foreach statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestForEachValid()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;
        int[] array = { 1, 2 };

        // Valid foreach #1
        foreach (var y in array)
        {
        }

        // Valid foreach #2
        foreach (var y in array)
        {
            x += y;
        }

        // Valid foreach #3 (Valid only for SA1500)
        foreach (var y in array) { }

        // Valid foreach #4 (Valid only for SA1500)
        foreach (var y in array) { x += y; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid foreach statement definitions.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestForEachInvalid()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;
        int[] array = { 1, 2 };

        // Invalid foreach #1
        foreach (var y in array) {
        }

        // Invalid foreach #2
        foreach (var y in array) {
            x += y;
        }

        // Invalid foreach #3
        foreach (var y in array) {
            x += y; }

        // Invalid foreach #4
        foreach (var y in array) { x += y;
        }

        // Invalid foreach #5
        foreach (var y in array)
        {
            x += y; }

        // Invalid foreach #6
        foreach (var y in array)
        { x += y;
        }

        // Invalid foreach #7
        foreach (var y in array)
        { x += y; }
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid foreach #1
                this.CSharpDiagnostic().WithLocation(9, 34),
                // Invalid foreach #2
                this.CSharpDiagnostic().WithLocation(13, 34),
                // Invalid foreach #3
                this.CSharpDiagnostic().WithLocation(18, 34),
                this.CSharpDiagnostic().WithLocation(19, 21),
                // Invalid foreach #4
                this.CSharpDiagnostic().WithLocation(22, 34),
                // Invalid foreach #5
                this.CSharpDiagnostic().WithLocation(28, 21),
                // Invalid foreach #6
                this.CSharpDiagnostic().WithLocation(32, 9),
                // Invalid foreach #7
                this.CSharpDiagnostic().WithLocation(37, 9),
                this.CSharpDiagnostic().WithLocation(37, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}