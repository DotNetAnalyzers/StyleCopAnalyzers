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
        /// Verifies that no diagnostics are reported for the valid classes defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies diagnostics and codefixes for all invalid class definitions.
        /// </summary>
        /// <remarks>
        /// These will normally also report SA1401, but not in the unit test.
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
                this.CSharpDiagnostic().WithLocation(3, 32),

                // InvalidClass2
                this.CSharpDiagnostic().WithLocation(6, 32),

                // InvalidClass3
                this.CSharpDiagnostic().WithLocation(10, 32),
                this.CSharpDiagnostic().WithLocation(11, 27),

                // InvalidClass4
                this.CSharpDiagnostic().WithLocation(13, 32),

                // InvalidClass5
                this.CSharpDiagnostic().WithLocation(18, 27),

                // InvalidClass6
                this.CSharpDiagnostic().WithLocation(21, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
