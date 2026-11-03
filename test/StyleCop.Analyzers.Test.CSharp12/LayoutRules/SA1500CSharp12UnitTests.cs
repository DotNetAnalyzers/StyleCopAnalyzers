// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.LayoutRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp11.LayoutRules;
using StyleCop.Analyzers.Test.Helpers;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
    StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

public partial class SA1500CSharp12UnitTests : SA1500CSharp11UnitTests
{
    /// <summary>
    /// Verifies that unindented nested closing brackets move, but single-line collections remain intact.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestNestedCollectionClosingBracketIndentationAsync()
    {
        var testCode = """
            class C
            {
                int[][] multiline = {|#0:[|}[
                        1,
            ],
                ];

                int[][] singleLine = {|#1:[|}[1, 2],
                ];
            }
            """;

        var fixedCode = """
            class C
            {
                int[][] multiline = [
                    [
                        1,
                    ],
                ];

                int[][] singleLine = [
                    [1, 2],
                ];
            }
            """;

        await VerifyCSharpFixAsync(
            testCode,
            new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
            fixedCode,
            CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that moving a nested opening bracket also aligns its otherwise valid closing bracket.
    /// </summary>
    /// <param name="lineEnding">The line ending used in the source.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
    public async Task TestNestedCollectionWithoutInnerDiagnosticAsync(string lineEnding)
    {
        var testCode = """
            class C
            {
                int[][] values = {|#0:[|}[
                        1, // Preserve the element comment.
                /* Preserve the closing comment. */
                ],
                ];
            }
            """;

        var fixedCode = """
            class C
            {
                int[][] values = [
                    [
                        1, // Preserve the element comment.
                /* Preserve the closing comment. */
                    ],
                ];
            }
            """;

        await VerifyCSharpFixAsync(
            testCode.ReplaceLineEndings(lineEnding),
            Diagnostic().WithLocation(0),
            fixedCode.ReplaceLineEndings(lineEnding),
            CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that a multiline collection's final element cannot share its closing bracket's line.
    /// </summary>
    /// <param name="comma">The optional trailing comma.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("")]
    [InlineData(",")]
    public async Task TestCollectionExpressionClosingBracketAsync(string comma)
    {
        var testCode = $$"""
            class C
            {
                object[] M(string text, int maxCount)
                {
                    object[] parameters = [
                        new { Value = $"\"{text}\"" }, // Preserve this comment.
                        new { Value = maxCount }{{comma}}{|#0:]|};
                    return parameters;
                }
            }
            """;

        var fixedCode = $$"""
            class C
            {
                object[] M(string text, int maxCount)
                {
                    object[] parameters = [
                        new { Value = $"\"{text}\"" }, // Preserve this comment.
                        new { Value = maxCount }{{comma}}
                    ];
                    return parameters;
                }
            }
            """;

        await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that a multiline collection's opening bracket does not share a line with its first element.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestCollectionExpressionOpeningBracketAsync()
    {
        var testCode = """
            class C
            {
                object[][] values =
                [
                    {|#0:[|}new { Value = "path" },
                        new { Value = 1 },
                        new { Value = 2 },
                ],
                ];
            }
            """;

        var fixedCode = """
            class C
            {
                object[][] values =
                [
                    [
                        new { Value = "path" },
                        new { Value = 1 },
                        new { Value = 2 },
                ],
                ];
            }
            """;

        await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that fixing an inline opening bracket preserves its position and comments.
    /// </summary>
    /// <param name="lineEnding">The line ending used in the source.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
    public async Task TestInlineCollectionExpressionOpeningBracketAsync(string lineEnding)
    {
        var testCode = """
            class C
            {
                int[] M(int[] other)
                {
                    return {|#0:[|}/* Spread. */ .. other,
                        1,
                    ];
                }
            }
            """;

        var fixedCode = """
            class C
            {
                int[] M(int[] other)
                {
                    return [/* Spread. */
                        .. other,
                        1,
                    ];
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
    /// Verifies that opening brackets may follow other code and that single-line collections remain valid.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestCollectionExpressionValidLayoutsAsync()
    {
        var testCode = """
            class C
            {
                int[][] values = [
                    [1, 2],
                    [],
                    [
                        3,
                        4,
                    ],
                ];

                int[] M(int[] other) => (int[])[
                    .. other,
                    5,
                ];

                int[] empty = [
                ];

                int[][] Nested() => [
                    [
                        1,
                    ],
                ];

                void Use(int[] values)
                {
                    Use([
                        1,
                        2,
                    ]);
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that Fix All handles adjacent and nested brackets while preserving comments.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestNestedCollectionExpressionBracketsAsync()
    {
        var testCode = """
            class C
            {
                int[][] values = {|#0:[|}{|#1:[|}1, // First element.
                        2, /* Last element. */ {|#2:]|}, {|#3:]|};
            }
            """;

        var fixedCode = """
            class C
            {
                int[][] values = [
                    [
                        1, // First element.
                        2, /* Last element. */
                    ],
                ];
            }
            """;

        await VerifyCSharpFixAsync(
            testCode,
            new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2), Diagnostic().WithLocation(3) },
            fixedCode,
            CancellationToken.None).ConfigureAwait(false);
    }
}
