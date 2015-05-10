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
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestBlockValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar()
    {
        // valid block #1
        {
        }

        // valid block #2
        {
            Debug.Indent();
        }

        // valid block #3 (valid only for SA1500)
        { }

        // valid block #4 (valid only for SA1500)
        { Debug.Indent(); }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid blocks.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestBlockInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar()
    {
        // invalid block #1
        { Debug.Indent();
        }

        // invalid block #2
        { 
            Debug.Indent(); }
    }
}";
            var expectedDiagnostics = new[]
            {
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(13, 29)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
