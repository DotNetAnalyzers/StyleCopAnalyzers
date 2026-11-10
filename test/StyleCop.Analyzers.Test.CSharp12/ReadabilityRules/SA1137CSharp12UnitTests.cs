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
        /// Verifies that a closing bracket aligns with the line containing an inline opening bracket.
        /// </summary>
        /// <param name="lineEnding">The line ending used in the source.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestInlineOpeningBracketIndentationAsync(string lineEnding)
        {
            var testCode = """
                class C
                {
                    object[] M(string text, int maxCount)
                    {
                        object[] parameters = [
                            new { Value = $"\"{text}\"" }, // Keep this comment.
                            new { Value = maxCount },
                {|#0:            |}];
                        return parameters;
                    }
                }
                """;

            var fixedCode = """
                class C
                {
                    object[] M(string text, int maxCount)
                    {
                        object[] parameters = [
                            new { Value = $"\"{text}\"" }, // Keep this comment.
                            new { Value = maxCount },
                        ];
                        return parameters;
                    }
                }
                """;

            await VerifyCSharpFixAsync(
                testCode.ReplaceLineEndings(lineEnding),
                Diagnostic().WithLocation(0),
                fixedCode.ReplaceLineEndings(lineEnding),
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies bracket alignment even when the first element shares the opening bracket's line.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestOpeningBracketSharingFirstElementLineAsync()
        {
            var testCode = """
                class C
                {
                    object[][] values =
                    [
                        [new { Value = "path" },
                            new { Value = 1 },
                            new { Value = 2 },
                {|#0:    |}],
                    ];
                }
                """;

            var fixedCode = """
                class C
                {
                    object[][] values =
                    [
                        [new { Value = "path" },
                            new { Value = 1 },
                            new { Value = 2 },
                        ],
                    ];
                }
                """;

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies inline bracket alignment with tabs, comments, and unindented closing brackets.
        /// </summary>
        /// <param name="indentation">The indentation of the opening bracket's line.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [InlineData("    ")]
        [InlineData("\t")]
        public async Task TestInlineOpeningBracketWithUnindentedClosingBracketAsync(string indentation)
        {
            var testCode = $$"""
                class C
                {
                {{indentation}}int[] M(int[] other) => /* Collection. */ [
                        .. other, // Keep this comment.
                {|#0:]|};
                }
                """;

            var fixedCode = $$"""
                class C
                {
                {{indentation}}int[] M(int[] other) => /* Collection. */ [
                        .. other, // Keep this comment.
                {{indentation}}];
                }
                """;

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that aligned inline-opening collections and single-line collections remain valid.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestInlineOpeningBracketValidIndentationAsync()
        {
            var testCode = """
                class C
                {
                    int[] M(int[] other)
                    {
                        return [
                            .. other,
                            1,
                        ];
                    }

                    int[] empty = [
                    ];

                    int[] single = [1, 2];
                }

                class Unindented
                {
                int[] M() => [
                    1,
                ];
                }
                """;

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a closing bracket aligned with an inline opening bracket is accepted.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4399, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4399")]
        public async Task TestInlineOpeningBracketAlignedClosingBracketIsAcceptedAsync()
        {
            var testCode = @"class C
{
    string[] something = [
                            ""some"", ""text"", ""here"",
                         ];
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that collection-expression bracket alignment uses visual columns when tabs are enabled.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestInlineOpeningBracketAlignedClosingBracketWithTabsIsAcceptedAsync()
        {
            const string testCode = "class C\n{\n\tstring[] something = [\n\t\t\"some\", \"text\", \"here\",\n\t\t\t\t\t\t ];\n}";
            var test = new CSharpTest
            {
                TestCode = testCode,
                UseTabs = true,
                TabSize = 4,
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that collection-expression brackets at different visual columns are still diagnosed with tabs.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestInlineOpeningBracketMisalignedClosingBracketWithTabsIsDiagnosedAsync()
        {
            const string testCode = "class C\n{\n\tstring[] something = [\n\t\t\"some\", \"text\", \"here\",\n{|#0:\t\t\t\t\t|}];\n}";
            var test = new CSharpTest
            {
                TestCode = testCode,
                UseTabs = true,
                TabSize = 4,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix can align a closing bracket with an inline opening bracket.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        [WorkItem(4399, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4399")]
        public async Task TestAlignClosingBracketWithOpeningBracketCodeFixAsync()
        {
            var testCode = @"class C
{
    string[] something = [
        ""some"", ""text"", ""here"",
{|#0:     |}];
}";
            var fixedCode = @"class C
{
    string[] something = [
        ""some"", ""text"", ""here"",
                         ];
}";
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CodeActionEquivalenceKey = "AlignClosingBracketWithOpeningBracket",
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the alternate code fix uses tabs when aligning a closing bracket with an inline opening bracket.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestAlignClosingBracketWithOpeningBracketCodeFixWithTabsAsync()
        {
            const string testCode = "class C\n{\n\tstring[] something = [\n\t\t\"some\", \"text\", \"here\",\n{|#0:\t\t|}];\n}";
            const string fixedCode = "class C\n{\n\tstring[] something = [\n\t\t\"some\", \"text\", \"here\",\n\t\t\t\t\t\t ];\n}";
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                CodeActionEquivalenceKey = "AlignClosingBracketWithOpeningBracket",
                UseTabs = true,
                TabSize = 4,
            };
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that inline closing brackets use the same fix as inline closing braces.
        /// </summary>
        /// <param name="lineEnding">The line ending used in the source.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
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
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
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

        [Fact]
        public async Task TestCollectionExpressionWithMissingCloseBracketAsync()
        {
            string testCode = @"
class TestClass
{
    void TestMethod()
    {
        int[] a = [1,
            2{|CS1003:;|}
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
