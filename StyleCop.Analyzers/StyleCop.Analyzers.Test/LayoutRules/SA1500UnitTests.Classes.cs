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
        /// Verifies that no diagnostics are reported for the valid classes defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestClassValid()
        {
            var testCode = @"public class Foo
{
    public void ValidClass1
    {
    }

    public void ValidClass2
    {
        private int field;
    }

    public void ValidClass3 { } /* Valid only for SA1500 */

    public void ValidClass4 { private int field; }  /* Valid only for SA1500 */
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid class definitions.
        /// </summary>
        [Fact]
        public async Task TestClassInvalid()
        {
            var testCode = @"public class Foo
{
    public void InvalidClass1 {
    }

    public void InvalidClass2 {
        private int field; 
    }

    public void InvalidClass3 {
        private int field; }

    public void InvalidClass4 { private int field; 
    }

    public void InvalidClass5
    { 
        private int field; }

    public void InvalidClass6
    { private int field; 
    }

    public void InvalidClass7
    { private int field; }
}";

            var expectedDiagnostics = new[]
            {
                // InvalidClass1
                this.CSharpDiagnostic().WithLocation(3, 31),
                // InvalidClass2
                this.CSharpDiagnostic().WithLocation(6, 31),
                // InvalidClass3
                this.CSharpDiagnostic().WithLocation(10, 31),
                this.CSharpDiagnostic().WithLocation(11, 28),
                // InvalidClass4
                this.CSharpDiagnostic().WithLocation(13, 31),
                // InvalidClass5
                this.CSharpDiagnostic().WithLocation(18, 28),
                // InvalidClass6
                this.CSharpDiagnostic().WithLocation(21, 5),
                // InvalidClass7
                this.CSharpDiagnostic().WithLocation(25, 5),
                this.CSharpDiagnostic().WithLocation(25, 26)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}