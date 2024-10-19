﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
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
        public static IEnumerable<object[]> StatementBlocksTokenList
        {
            get
            {
                yield return new[] { "checked" };
                yield return new[] { "fixed (int* p = new[] { 1, 2, 3 })" };
                yield return new[] { "for (var y = 0; y < 2; y++)" };
                yield return new[] { "foreach (var y in new[] { 1, 2, 3 })" };
                yield return new[] { "lock (this)" };
                yield return new[] { "unchecked" };
                yield return new[] { "unsafe" };
                yield return new[] { "using (var x = new System.Threading.ManualResetEvent(true))" };
                yield return new[] { "while (this.X < 2)" };
            }
        }

        /// <summary>
        /// Verifies that no diagnostics are reported for the valid statements defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics outside of the unit test scenario.
        ///
        /// The class is marked unsafe to make testing the fixed statement possible.</para>
        /// </remarks>
        /// <param name="token">The source code preceding the opening <c>{</c> of a statement block.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(StatementBlocksTokenList))]
        public async Task TestStatementBlockValidAsync(string token)
        {
            var testCode = @"public unsafe class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // valid #1
        #TOKEN#
        {
        }

        // valid #2
        #TOKEN#
        {
            this.X = 1;
        }

        // valid #3 (valid only for SA1500)
        #TOKEN# { }

        // valid #4 (valid only for SA1500)
        #TOKEN# { this.X = 1; }

        // valid #5 (valid only for SA1500)
        #TOKEN# 
        { this.X = 1; }
    }
}";

            testCode = testCode.Replace("#TOKEN#", token);

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid statements.
        /// </summary>
        /// <remarks>
        /// <para>The class is marked unsafe to make testing the fixed statement possible.</para>
        /// </remarks>
        /// <param name="token">The source code preceding the opening <c>{</c> of a statement block.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(StatementBlocksTokenList))]
        public async Task TestStatementBlockInvalidAsync(string token)
        {
            var testCode = @"public unsafe class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // invalid #1
        #TOKEN# {
            this.X = 1;
        }

        // invalid #2
        #TOKEN# {
            this.X = 1; }

        // invalid #3
        #TOKEN# { this.X = 1;
        }

        // invalid #4
        #TOKEN#
        {
            this.X = 1; }

        // invalid #5
        #TOKEN#
        { this.X = 1;
        }
    }
}";

            var fixedTestCode = @"public unsafe class Foo
{
    public int X { get; set; }

    public void Bar()
    {
        // invalid #1
        #TOKEN#
        {
            this.X = 1;
        }

        // invalid #2
        #TOKEN#
        {
            this.X = 1;
        }

        // invalid #3
        #TOKEN#
        {
            this.X = 1;
        }

        // invalid #4
        #TOKEN#
        {
            this.X = 1;
        }

        // invalid #5
        #TOKEN#
        {
            this.X = 1;
        }
    }
}";

            testCode = testCode.Replace("#TOKEN#", token);
            fixedTestCode = fixedTestCode.Replace("#TOKEN#", token);
            var tokenLength = token.Length;

            DiagnosticResult[] expectedDiagnostics =
            {
                // invalid #1
                Diagnostic().WithLocation(8, 10 + tokenLength),

                // invalid #2
                Diagnostic().WithLocation(13, 10 + tokenLength),
                Diagnostic().WithLocation(14, 25),

                // invalid #3
                Diagnostic().WithLocation(17, 10 + tokenLength),

                // invalid #4
                Diagnostic().WithLocation(23, 25),

                // invalid #5
                Diagnostic().WithLocation(27, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
