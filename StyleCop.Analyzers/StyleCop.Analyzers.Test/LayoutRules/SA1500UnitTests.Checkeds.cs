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
        /// Verifies that no diagnostics are reported for the valid checked statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics outside of the unit test scenario.
        /// </remarks>
        [Fact]
        public async Task TestCheckedValid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // valid checked #1
        checked
        {
        }

        // valid checked #2
        checked
        {
            this.X = 1;
        }

        // valid checked #3 (valid only for SA1500)
        checked { }

        // valid checked #4 (valid only for SA1500)
        checked { this.X = 1; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid checked statements.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestCheckedInvalid()
        {
            var testCode = @"public class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // invalid checked #1
        checked {
            this.X = 1;
        }

        // invalid checked #2
        checked {
            this.X = 1; }

        // invalid checked #3
        checked { this.X = 1;
        }

        // invalid checked #4
        checked 
        {
            this.X = 1; }

        // invalid checked #5
        checked 
        { this.X = 1;
        }

        // invalid checked #6
        checked 
        { this.X = 1; }
    }
}";

            var expectedDiagnostics = new[]
            {
                // invalid checked #1
                this.CSharpDiagnostic().WithLocation(8, 17),

                // invalid checked #2
                this.CSharpDiagnostic().WithLocation(13, 17),
                this.CSharpDiagnostic().WithLocation(14, 25),

                // invalid checked #3
                this.CSharpDiagnostic().WithLocation(17, 17),

                // invalid checked #4
                this.CSharpDiagnostic().WithLocation(23, 25),

                // invalid checked #5
                this.CSharpDiagnostic().WithLocation(27, 9),

                // invalid checked #6
                this.CSharpDiagnostic().WithLocation(32, 9),
                this.CSharpDiagnostic().WithLocation(32, 23)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

