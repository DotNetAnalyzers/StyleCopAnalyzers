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
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid do ... while statements defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(10, 9),

                // Invalid do ... while #2
                Diagnostic().WithLocation(13, 12),

                // Invalid do ... while #3
                Diagnostic().WithLocation(18, 12),
                Diagnostic().WithLocation(19, 9),

                // Invalid do ... while #4
                Diagnostic().WithLocation(25, 9),

                // Invalid do ... while #5
                Diagnostic().WithLocation(30, 20),

                // Invalid do ... while #6
                Diagnostic().WithLocation(36, 20),

                // Invalid do ... while #7
                Diagnostic().WithLocation(40, 9),

                // Invalid do ... while #8
                Diagnostic().WithLocation(46, 9),
                Diagnostic().WithLocation(47, 9),

                // Invalid do ... while #9
                Diagnostic().WithLocation(50, 12),

                // Invalid do ... while #10
                Diagnostic().WithLocation(56, 12),
                Diagnostic().WithLocation(58, 9),

                // Invalid do ... while #11
                Diagnostic().WithLocation(61, 12),
                Diagnostic().WithLocation(62, 20),

                // Invalid do ... while #12
                Diagnostic().WithLocation(66, 12),
                Diagnostic().WithLocation(67, 20),

                // Invalid do ... while #13
                Diagnostic().WithLocation(70, 12),

                // Invalid do ... while #14
                Diagnostic().WithLocation(75, 12),
                Diagnostic().WithLocation(76, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that no diagnostics are reported for the do/while loop when the <see langword="while"/>
        /// expression is on the same line as the closing brace and the setting is enabled.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoWhileOnClosingBraceWithAllowSettingAsync()
        {
            var testSettings = @"
{
    ""settings"": {
        ""layoutRules"": {
            ""allowDoWhileOnClosingBrace"": true
        }
    }
}";

            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        do
        {
            x = 1;
        } while (x == 0);
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                Settings = testSettings,
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for the invalid <see langword="while"/> loop that
        /// is on the same line as the closing brace which is not part of a <c>do/while</c> loop. This
        /// ensures that the <c>allowDoWhileOnClosingBrace</c> setting is only applicable to a <c>do/while</c>
        /// loop and will not mistakenly allow a plain <see langword="while"/> loop after any arbitrary
        /// closing brace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestJustWhileLoopOnClosingBraceWithAllowDoWhileOnClosingBraceSettingAsync()
        {
            var testSettings = @"
{
    ""settings"": {
        ""layoutRules"": {
            ""allowDoWhileOnClosingBrace"": true
        }
    }
}";

            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        while (x == 0)
        {
            x = 1;
        [|}|] while (x == 0)
        {
            x = 1;
        }
    }
}";

            var fixedCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        while (x == 0)
        {
            x = 1;
        }
        while (x == 0)
        {
            x = 1;
        }
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                Settings = testSettings,
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that no diagnostics are reported for the do/while loop when the <see langword="while"/>
        /// expression is on the same line as the closing brace and the setting is allowed.
        /// </summary>
        /// <remarks>
        /// <para>The "Invalid do ... while #6" code in the <see cref="TestDoWhileInvalidAsync"/> unit test
        /// should account for the proper fix when the <c>allowDoWhileOnClosingBrace</c> is <see langword="false"/>,
        /// which is the default.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFixDoWhileOnClosingBraceWithAllowSettingAsync()
        {
            var testSettings = @"
{
    ""settings"": {
        ""layoutRules"": {
            ""allowDoWhileOnClosingBrace"": true
        }
    }
}";

            var testCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        do
        {
            x = 1; [|}|] while (x == 0);
    }
}";

            var fixedTestCode = @"public class Foo
{
    private void Bar()
    {
        var x = 0;

        do
        {
            x = 1;
        } while (x == 0);
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
                Settings = testSettings,
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
