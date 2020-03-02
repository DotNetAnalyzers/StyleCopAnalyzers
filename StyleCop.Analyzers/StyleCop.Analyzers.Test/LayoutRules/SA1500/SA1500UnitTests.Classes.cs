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
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid classes defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassValidAsync()
        {
            var testCode = @"public class Foo
{
    public class ValidClass1
    {
    }

    public class ValidClass2
    {
        public int Field;
    }

    public class ValidClass3 { } /* Valid only for SA1500 */

    public class ValidClass4 { public int Field; }  /* Valid only for SA1500 */

    public class ValidClass5 
    { public int Field; }  /* Valid only for SA1500 */
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies diagnostics and codefixes for all invalid class definitions.
        /// </summary>
        /// <remarks>
        /// <para>These will normally also report SA1401, but not in the unit test.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClassInvalidAsync()
        {
            var testCode = @"public class Foo
{
    public class InvalidClass1 {
    }

    public class InvalidClass2 {
        public int Field;
    }

    public class InvalidClass3 {
        public int Field; }

    public class InvalidClass4 { public int Field;
    }

    public class InvalidClass5
    {
        public int Field; }

    public class InvalidClass6
    { public int Field;
    }
}";

            var fixedTestCode = @"public class Foo
{
    public class InvalidClass1
    {
    }

    public class InvalidClass2
    {
        public int Field;
    }

    public class InvalidClass3
    {
        public int Field;
    }

    public class InvalidClass4
    {
        public int Field;
    }

    public class InvalidClass5
    {
        public int Field;
    }

    public class InvalidClass6
    {
        public int Field;
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidClass1
                Diagnostic().WithLocation(3, 32),

                // InvalidClass2
                Diagnostic().WithLocation(6, 32),

                // InvalidClass3
                Diagnostic().WithLocation(10, 32),
                Diagnostic().WithLocation(11, 27),

                // InvalidClass4
                Diagnostic().WithLocation(13, 32),

                // InvalidClass5
                Diagnostic().WithLocation(18, 27),

                // InvalidClass6
                Diagnostic().WithLocation(21, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
