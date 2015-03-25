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
        /// Verifies that no diagnostics are reported for the valid for statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestForValid()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Valid for #1
        for (var y = 0; y < 2; y++)
        {
        }

        // Valid for #2
        for (var y = 0; y < 2; y++)
        {
            x += y;
        }

        // Valid for #3 (Valid only for SA1500)
        for (var y = 0; y < 2; y++) { }

        // Valid for #4 (Valid only for SA1500)
        for (var y = 0; y < 2; y++) { x += y; }

        // Valid for #5 (Valid only for SA1500)
        for (var y = 0; y < 2; y++) 
        { x += y; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid for statement definitions.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestForInvalid()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Invalid for #1
        for (var y = 0; y < 2; y++) {
        }

        // Invalid for #2
        for (var y = 0; y < 2; y++) {
            x += y;
        }

        // Invalid for #3
        for (var y = 0; y < 2; y++) {
            x += y; }

        // Invalid for #4
        for (var y = 0; y < 2; y++) { x += y;
        }

        // Invalid for #5
        for (var y = 0; y < 2; y++)
        {
            x += y; }

        // Invalid for #6
        for (var y = 0; y < 2; y++)
        { x += y;
        }
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid for #1
                this.CSharpDiagnostic().WithLocation(8, 37),
                // Invalid for #2
                this.CSharpDiagnostic().WithLocation(12, 37),
                // Invalid for #3
                this.CSharpDiagnostic().WithLocation(17, 37),
                this.CSharpDiagnostic().WithLocation(18, 21),
                // Invalid for #4
                this.CSharpDiagnostic().WithLocation(21, 37),
                // Invalid for #5
                this.CSharpDiagnostic().WithLocation(27, 21),
                // Invalid for #6
                this.CSharpDiagnostic().WithLocation(31, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}