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
        /// Verifies that no diagnostics are reported for the valid interfaces defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceValidAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid interface definitions.
        /// </summary>
        /// <remarks>
        /// <para>These will normally also report SA1401, but not in the unit test.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceInvalidAsync()
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
                Diagnostic().WithLocation(3, 40),

                // InvalidInterface2
                Diagnostic().WithLocation(6, 40),

                // InvalidInterface3
                Diagnostic().WithLocation(10, 40),
                Diagnostic().WithLocation(11, 21),

                // InvalidInterface4
                Diagnostic().WithLocation(13, 40),

                // InvalidInterface5
                Diagnostic().WithLocation(18, 21),

                // InvalidInterface6
                Diagnostic().WithLocation(21, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
