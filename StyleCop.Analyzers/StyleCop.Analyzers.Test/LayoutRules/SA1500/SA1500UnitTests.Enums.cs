// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
        /// Verifies that no diagnostics are reported for the valid enums defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(3, 30),

                // InvalidEnum2
                this.CSharpDiagnostic().WithLocation(6, 30),

                // InvalidEnum3
                this.CSharpDiagnostic().WithLocation(10, 30),
                this.CSharpDiagnostic().WithLocation(11, 14),

                // InvalidEnum4
                this.CSharpDiagnostic().WithLocation(13, 30),

                // InvalidEnum5
                this.CSharpDiagnostic().WithLocation(18, 14),

                // InvalidEnum6
                this.CSharpDiagnostic().WithLocation(21, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
