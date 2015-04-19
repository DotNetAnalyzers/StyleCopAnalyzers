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
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid switch statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics outside of the unit test scenario.
        /// </remarks>
        [Fact]
        public async Task TestSwitchValid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // valid switch #1
        switch (this.X)
        {
            case 0:
                break;
        }

        // valid switch #2 (valid only for SA1500)
        switch (this.X) { case 0: break; }

        // valid switch #3 (valid only for SA1500)
        switch (this.X) 
        { case 0: break; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid switch statements.
        /// </summary>
        [Fact]
        public async Task TestSwitchInvalid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // invalid switch #1
        switch (this.X) {
            case 0:
                break;
        }

        // invalid switch #2
        switch (this.X) {
            case 0:
                break; }

        // invalid switch #3
        switch (this.X) { case 0:
                break;
        }

        // invalid switch #4
        switch (this.X)
        {
            case 0:
                break; }

        // invalid switch #5
        switch (this.X)
        { case 0:
                break;
        }
    }
}";

            var fixedTestCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // invalid switch #1
        switch (this.X)
        {
            case 0:
                break;
        }

        // invalid switch #2
        switch (this.X)
        {
            case 0:
                break;
        }

        // invalid switch #3
        switch (this.X)
        {
            case 0:
                break;
        }

        // invalid switch #4
        switch (this.X)
        {
            case 0:
                break;
        }

        // invalid switch #5
        switch (this.X)
        {
            case 0:
                break;
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // invalid switch #1
                this.CSharpDiagnostic().WithLocation(8, 25),

                // invalid switch #2
                this.CSharpDiagnostic().WithLocation(14, 25),
                this.CSharpDiagnostic().WithLocation(16, 24),

                // invalid switch #3
                this.CSharpDiagnostic().WithLocation(19, 25),

                // invalid switch #4
                this.CSharpDiagnostic().WithLocation(27, 24),

                // invalid switch #5
                this.CSharpDiagnostic().WithLocation(31, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
