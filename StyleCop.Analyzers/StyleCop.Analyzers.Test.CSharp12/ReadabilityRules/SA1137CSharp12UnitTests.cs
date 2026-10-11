// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    public partial class SA1137CSharp12UnitTests : SA1137CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that inline closing brackets use the same fix as inline closing braces.
        /// </summary>
        /// <param name="lineEnding">The line ending used in the source.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [WorkItem(3296, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3296")]
        public async Task TestInlineCollectionExpressionClosingBracketAsync(string lineEnding)
        {
            string testCode = @"
class C
{
    private int[] values =
    [
        42 /* comment */ {|#0:]|};
}
";
            string fixedCode = @"
class C
{
    private int[] values =
    [
        42 /* comment */
    ];
}
";

            await VerifyCSharpFixAsync(
                testCode.ReplaceLineEndings(lineEnding),
                Diagnostic().WithLocation(0),
                fixedCode.ReplaceLineEndings(lineEnding),
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments leading a closing bracket remain on their own line.
        /// </summary>
        /// <param name="lineEnding">The line ending used in the source.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [WorkItem(3296, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3296")]
        public async Task TestCollectionExpressionClosingBracketAfterLeadingCommentAsync(string lineEnding)
        {
            string testCode = @"
class C
{
    private int[] values =
    [
        42
/* comment */[| |]];
}
";
            string fixedCode = @"
class C
{
    private int[] values =
    [
        42
/* comment */
    ];
}
";

            await VerifyCSharpFixAsync(
                testCode.ReplaceLineEndings(lineEnding),
                DiagnosticResult.EmptyDiagnosticResults,
                fixedCode.ReplaceLineEndings(lineEnding),
                CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestSingleLineCollectionExpressionAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField = [1, 2, 3];
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestMultiLineCollectionExpressionStartingOnPreviousLineAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField =
    [
        1,
[|         |]2,
[|       |]3
[|     |]];
}
";

            string fixedCode = @"
class TestClass
{
    private int[] testField =
    [
        1,
        2,
        3
    ];
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestMultiLineCollectionExpressionStartingOnNextLineAsync()
        {
            string testCode = @"
class TestClass
{
    private int[] testField = [
        1,
[|         |]2,
[|       |]3
    ];
}
";

            string fixedCode = @"
class TestClass
{
    private int[] testField = [
        1,
        2,
        3
    ];
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3904, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3904")]
        public async Task TestCollectionExpressionWithSpreadAndNestedElementsAsync()
        {
            string testCode = @"
class TestClass
{
    void TestMethod(int[] other)
    {
        int[] a =
        [
            1,
[|              |].. other,
            3,
        ];
        Use(
        [
            [1, 2],
[|              |][3, 4],
        ]);
    }

    static void Use(int[][] value)
    {
    }
}
";

            string fixedCode = @"
class TestClass
{
    void TestMethod(int[] other)
    {
        int[] a =
        [
            1,
            .. other,
            3,
        ];
        Use(
        [
            [1, 2],
            [3, 4],
        ]);
    }

    static void Use(int[][] value)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
