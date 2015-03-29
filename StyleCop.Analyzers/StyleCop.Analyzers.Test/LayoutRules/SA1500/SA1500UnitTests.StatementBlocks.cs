﻿namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using System.Collections.Generic;


    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests : DiagnosticVerifier
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
        /// These are valid for SA1500 only, some will report other diagnostics outside of the unit test scenario.
        /// 
        /// The class is marked unsafe to make testing the fixed statement possible.
        /// </remarks>
        [Theory]
        [MemberData(nameof(StatementBlocksTokenList))]
        public async Task TestStatementBlockValid(string token)
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid statements.
        /// </summary>
        /// <remarks>
        /// The class is marked unsafe to make testing the fixed statement possible.
        /// </remarks>
        [Theory(Skip = "Disabled until the SA1500 implementation is available")]
        [MemberData(nameof(StatementBlocksTokenList))]
        public async Task TestStatementBlockInvalid(string token)
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

            testCode = testCode.Replace("#TOKEN#", token);
            var tokenLength = token.Length;

            var expectedDiagnostics = new[]
            {
                // invalid #1
                this.CSharpDiagnostic().WithLocation(8, 11 + tokenLength),

                // invalid #2
                this.CSharpDiagnostic().WithLocation(13, 11 + tokenLength),
                this.CSharpDiagnostic().WithLocation(14, 25),

                // invalid #3
                this.CSharpDiagnostic().WithLocation(17, 11 + tokenLength),

                // invalid #4
                this.CSharpDiagnostic().WithLocation(23, 25),

                // invalid #5
                this.CSharpDiagnostic().WithLocation(27, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}

