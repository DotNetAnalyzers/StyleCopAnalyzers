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
        /// Verifies that no diagnostics are reported for the valid lock statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics outside of the unit test scenario.
        /// </remarks>
        [Fact]
        public async Task TestLockValid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        var y = new object();

        // valid lock #1
        lock (y)
        {
        }

        // valid lock #2
        lock (y)
        {
            this.X = 1;
        }

        // valid lock #3 (valid only for SA1500)
        lock (y) { }

        // valid lock #4 (valid only for SA1500)
        lock (y) { this.X = 1; }

        // valid lock #5 (valid only for SA1500)
        lock (y) 
        { this.X = 1; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid lock statements.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestLockInvalid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        var y = new object();

        // invalid lock #1
        lock (y) {
            this.X = 1;
        }

        // invalid lock #2
        lock (y) {
            this.X = 1; }

        // invalid lock #3
        lock (y) { this.X = 1;
        }

        // invalid lock #4
        lock (y) 
        {
            this.X = 1; }

        // invalid lock #5
        lock (y) 
        { this.X = 1;
        }
    }
}";

            var expectedDiagnostics = new[]
            {
                // invalid lock #1
                this.CSharpDiagnostic().WithLocation(10, 18),

                // invalid lock #2
                this.CSharpDiagnostic().WithLocation(15, 18),
                this.CSharpDiagnostic().WithLocation(16, 25),

                // invalid lock #3
                this.CSharpDiagnostic().WithLocation(19, 18),

                // invalid lock #4
                this.CSharpDiagnostic().WithLocation(25, 25),

                // invalid lock #5
                this.CSharpDiagnostic().WithLocation(29, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

