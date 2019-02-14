// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid switch statements defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics outside of the unit test
        /// scenario.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSwitchValidAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid switch statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSwitchInvalidAsync()
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
                Diagnostic().WithLocation(8, 25),

                // invalid switch #2
                Diagnostic().WithLocation(14, 25),
                Diagnostic().WithLocation(16, 24),

                // invalid switch #3
                Diagnostic().WithLocation(19, 25),

                // invalid switch #4
                Diagnostic().WithLocation(27, 24),

                // invalid switch #5
                Diagnostic().WithLocation(31, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
