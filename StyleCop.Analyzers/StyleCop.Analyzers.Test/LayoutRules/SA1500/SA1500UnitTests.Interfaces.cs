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
        /// Verifies that no diagnostics are reported for the valid interfaces defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestInterfaceValid()
        {
            var testCode = @"public class Foo
{
    public interface ValidInterface1
    {
    }

    public interface ValidInterface2
    {
        void Bar();
    }

    public interface ValidInterface3 { } /* Valid only for SA1500 */

    public interface ValidInterface4 { void Bar(); }  /* Valid only for SA1500 */

    public interface ValidInterface5 /* Valid only for SA1500 */
    { void Bar(); }  
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid interface definitions.
        /// </summary>
        /// <remarks>
        /// These will normally also report SA1401, but not in the unit test.
        /// </remarks>
        [Fact]
        public async Task TestInterfaceInvalid()
        {
            var testCode = @"public class Foo
{
    public interface InvalidInterface1 {
    }

    public interface InvalidInterface2 {
        void Bar();
    }

    public interface InvalidInterface3 {
        void Bar(); }

    public interface InvalidInterface4 { void Bar();
    }

    public interface InvalidInterface5
    {
        void Bar(); }

    public interface InvalidInterface6
    { void Bar();
    }
}";

            var fixedTestCode = @"public class Foo
{
    public interface InvalidInterface1
    {
    }

    public interface InvalidInterface2
    {
        void Bar();
    }

    public interface InvalidInterface3
    {
        void Bar();
    }

    public interface InvalidInterface4
    {
        void Bar();
    }

    public interface InvalidInterface5
    {
        void Bar();
    }

    public interface InvalidInterface6
    {
        void Bar();
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidInterface1
                this.CSharpDiagnostic().WithLocation(3, 40),
                // InvalidInterface2
                this.CSharpDiagnostic().WithLocation(6, 40),
                // InvalidInterface3
                this.CSharpDiagnostic().WithLocation(10, 40),
                this.CSharpDiagnostic().WithLocation(11, 21),
                // InvalidInterface4
                this.CSharpDiagnostic().WithLocation(13, 40),
                // InvalidInterface5
                this.CSharpDiagnostic().WithLocation(18, 21),
                // InvalidInterface6
                this.CSharpDiagnostic().WithLocation(21, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
