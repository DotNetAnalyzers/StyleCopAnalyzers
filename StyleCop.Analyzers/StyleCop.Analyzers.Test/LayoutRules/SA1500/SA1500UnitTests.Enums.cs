// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
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
        /// Verifies that no diagnostics are reported for the valid enums defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumValidAsync()
        {
            var testCode = @"public class Foo
{
    public enum ValidEnum1
    {
    }

    public enum ValidEnum2
    {
        Test
    }

    public enum ValidEnum3 { } /* Valid only for SA1500 */

    public enum ValidEnum4 { Test }  /* Valid only for SA1500 */

    public enum ValidEnum5 /* Valid only for SA1500 */
    { Test }  
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid enum definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumInvalidAsync()
        {
            var testCode = @"public class Foo
{
    public enum InvalidEnum1 {
    }

    public enum InvalidEnum2 {
        Test
    }

    public enum InvalidEnum3 {
        Test }

    public enum InvalidEnum4 { Test
    }

    public enum InvalidEnum5
    {
        Test }

    public enum InvalidEnum6
    { Test
    }
}";

            var fixedTestCode = @"public class Foo
{
    public enum InvalidEnum1
    {
    }

    public enum InvalidEnum2
    {
        Test
    }

    public enum InvalidEnum3
    {
        Test
    }

    public enum InvalidEnum4
    {
        Test
    }

    public enum InvalidEnum5
    {
        Test
    }

    public enum InvalidEnum6
    {
        Test
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidEnum1
                Diagnostic().WithLocation(3, 30),

                // InvalidEnum2
                Diagnostic().WithLocation(6, 30),

                // InvalidEnum3
                Diagnostic().WithLocation(10, 30),
                Diagnostic().WithLocation(11, 14),

                // InvalidEnum4
                Diagnostic().WithLocation(13, 30),

                // InvalidEnum5
                Diagnostic().WithLocation(18, 14),

                // InvalidEnum6
                Diagnostic().WithLocation(21, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
