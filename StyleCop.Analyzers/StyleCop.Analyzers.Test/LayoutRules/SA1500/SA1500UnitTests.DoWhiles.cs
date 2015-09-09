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
        /// Verifies that no diagnostics are reported for the valid do ... while statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoWhileValidAsync()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Valid do ... while #1
        do
        {
        }
        while (x == 0);

        // Valid do ... while #2
        do
        {
            x = 1;
        }
        while (x == 0);

        // Valid do ... while #3 (Valid only for SA1500)
        do { } while (x == 0);

        // Valid do ... while #4 (Valid only for SA1500)
        do { x = 1; } while (x == 0);

        // Valid do ... while #5 (Valid only for SA1500)
        do 
        { x = 1; } while (x == 0);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid do ... while statement definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoWhileInvalidAsync()
        {
            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Invalid do ... while #1
        do
        {
        } while (x == 0);

        // Invalid do ... while #2
        do {
        }
        while (x == 0);

        // Invalid do ... while #3
        do {
        } while (x == 0);

        // Invalid do ... while #4
        do
        {
            x = 1;
        } while (x == 0);

        // Invalid do ... while #5
        do
        {
            x = 1; }
        while (x == 0);

        // Invalid do ... while #6
        do
        {
            x = 1; } while (x == 0);

        // Invalid do ... while #7
        do
        { x = 1;
        }
        while (x == 0);

        // Invalid do ... while #8
        do
        { x = 1;
        } while (x == 0);

        // Invalid do ... while #9
        do {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #10
        do {
            x = 1;
        } while (x == 0);

        // Invalid do ... while #11
        do {
            x = 1; }
        while (x == 0);

        // Invalid do ... while #12
        do {
            x = 1; } while (x == 0);

        // Invalid do ... while #13
        do { x = 1;
        }
        while (x == 0);

        // Invalid do ... while #14
        do { x = 1;
        } while (x == 0);
    }
}";

            var fixedTestCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Invalid do ... while #1
        do
        {
        }
        while (x == 0);

        // Invalid do ... while #2
        do
        {
        }
        while (x == 0);

        // Invalid do ... while #3
        do
        {
        }
        while (x == 0);

        // Invalid do ... while #4
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #5
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #6
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #7
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #8
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #9
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #10
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #11
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #12
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #13
        do
        {
            x = 1;
        }
        while (x == 0);

        // Invalid do ... while #14
        do
        {
            x = 1;
        }
        while (x == 0);
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid do ... while #1
                this.CSharpDiagnostic().WithLocation(10, 9),

                // Invalid do ... while #2
                this.CSharpDiagnostic().WithLocation(13, 12),

                // Invalid do ... while #3
                this.CSharpDiagnostic().WithLocation(18, 12),
                this.CSharpDiagnostic().WithLocation(19, 9),

                // Invalid do ... while #4
                this.CSharpDiagnostic().WithLocation(25, 9),

                // Invalid do ... while #5
                this.CSharpDiagnostic().WithLocation(30, 20),

                // Invalid do ... while #6
                this.CSharpDiagnostic().WithLocation(36, 20),

                // Invalid do ... while #7
                this.CSharpDiagnostic().WithLocation(40, 9),

                // Invalid do ... while #8
                this.CSharpDiagnostic().WithLocation(46, 9),
                this.CSharpDiagnostic().WithLocation(47, 9),

                // Invalid do ... while #9
                this.CSharpDiagnostic().WithLocation(50, 12),

                // Invalid do ... while #10
                this.CSharpDiagnostic().WithLocation(56, 12),
                this.CSharpDiagnostic().WithLocation(58, 9),

                // Invalid do ... while #11
                this.CSharpDiagnostic().WithLocation(61, 12),
                this.CSharpDiagnostic().WithLocation(62, 20),

                // Invalid do ... while #12
                this.CSharpDiagnostic().WithLocation(66, 12),
                this.CSharpDiagnostic().WithLocation(67, 20),

                // Invalid do ... while #13
                this.CSharpDiagnostic().WithLocation(70, 12),

                // Invalid do ... while #14
                this.CSharpDiagnostic().WithLocation(75, 12),
                this.CSharpDiagnostic().WithLocation(76, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
