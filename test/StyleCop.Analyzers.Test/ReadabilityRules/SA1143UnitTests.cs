// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.Helpers;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1143ConditionalExpressionsMustBeFormattedConsistently,
    StyleCop.Analyzers.ReadabilityRules.SA1143CodeFixProvider>;

/// <summary>
/// Tests formatting of conditional expressions.
/// </summary>
public class SA1143UnitTests
{
    /// <summary>
    /// Verifies that partially wrapped conditional expressions are reported.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    [WorkItem(651, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/651")]
    public async Task TestPartiallyWrappedConditionalAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    return condition {|#0:?|} 1 :
                        2;
                }
            }
            """;

        const string fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    return condition ? 1 : 2;
                }
            }
            """;

        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), fixedSource, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the accepted layouts, including nesting and the proposed aligned chain.
    /// </summary>
    /// <param name="expression">The expression to check.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("condition ? 1 : 2")]
    [InlineData("condition\n            ? 1\n            : 2")]
    [InlineData("(condition)\n            ? 1\n            : 2")]
    [InlineData("condition\n            ? (condition ? 1 : 2)\n            : 3")]
    [InlineData("condition\n            ? 1 +\n                2\n            : 3")]
    [InlineData("condition\n            ? 1\n            : 2 +\n                3")]
    [InlineData("condition\n            ? 1\n            : new int[]\n            {\n                1,\n            }.Length")]
    [InlineData("condition\n            ? 1\n            : @\"a\nb\".Length")]
    [InlineData("condition\n            ? 1\n            : condition ? 2 : 3")]
    [InlineData("condition\n            ? 1\n            : condition\n                ? 2\n                : 3")]
    [InlineData("condition ? 1 :\n               condition ? 2 :\n               3")]
    public async Task TestAcceptedLayoutsAsync(string expression)
    {
        var source = $$"""
            class C
            {
                int M(bool condition)
                {
                    return {{expression}};
                }
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that conditional branches can contain multiline arithmetic expressions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestMultilineArithmeticBranchesAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition, int a, int b, int c, int d, int e, int f)
                {
                    var first = condition
                        ? a * b +
                            c * d +
                            e * f
                        : 0;
                    var second = condition
                        ? 0
                        : a * b +
                            c * d +
                            e * f;
                    return first + second;
                }
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the complete chained example from the request.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestProposedChainAsync()
    {
        const string source = """
            class C
            {
                void M(string p)
                {
                    var x = p == "Cat" ? "Animals" :
                            p == "Bat" ? "Equipment" :
                            p == "Fat" ? "Nutrition" :
                            "None";
                }
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that inconsistent wrapping and indentation are reported once per chain.
    /// </summary>
    /// <param name="expression">The incorrectly formatted expression.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("condition {|#0:?|}\n            1 : 2")]
    [InlineData("condition\n            {|#0:?|} 1 : 2")]
    [InlineData("condition {|#0:?|} 1\n            : 2")]
    [InlineData("condition\n        {|#0:?|} 1\n        : 2")]
    [InlineData("condition\n            {|#0:?|} 1\n             : 2")]
    [InlineData("condition\n            {|#0:?|}\n            1\n            : 2")]
    [InlineData("condition\n            {|#0:?|} 1\n            :\n            2")]
    [InlineData("condition\n            {|#0:?|} 1 +\n            2\n            : 3")]
    [InlineData("condition\n            {|#0:?|} 1\n            : 2 +\n            3")]
    [InlineData("condition\n            {|#0:?|} 1 +\n        2\n            : 3")]
    [InlineData("condition\n            {|#0:?|} 1\n            : 2 +\n        3")]
    [InlineData("condition\n            {|#0:?|} 1 +\n                2 +\n            3\n            : 4")]
    [InlineData("condition\n            {|#0:?|} 1\n            : condition\n                ? 2\n                : 3 +\n                4")]
    [InlineData("condition &&\n            condition\n            {|#0:?|} 1\n            : 2")]
    [InlineData("condition\n            {|#0:?|} 1\n            : condition ? 2 :\n                3")]
    [InlineData("condition\n            /* keep */ {|#0:?|} 1\n            : 2")]
    [InlineData("condition {|#0:?|} 1 :\n            condition ? 2 :\n            3")]
    [InlineData("condition {|#0:?|} 1 :\n               condition ? 2 : 3")]
    [InlineData("condition {|#0:?|} 1 :\n               condition ? 2 :\n                3")]
    public async Task TestInvalidLayoutsAsync(string expression)
    {
        var source = $$"""
            class C
            {
                int M(bool condition)
                {
                    return {{expression}};
                }
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies both multiline choices and the single-line choice for chained conditions.
    /// </summary>
    /// <param name="equivalenceKey">The layout to select.</param>
    /// <param name="fixedExpression">The expected expression.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("SingleLine", "condition ? 1 : condition ? 2 : 3")]
    [InlineData("Expanded", "condition\n            ? 1\n            : condition\n                ? 2\n                : 3")]
    [InlineData("Chain", "condition ? 1 :\n               condition ? 2 :\n               3")]
    public async Task TestChainedFixesAsync(string equivalenceKey, string fixedExpression)
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    return condition {|#0:?|} 1 :
                        condition ? 2 : 3;
                }
            }
            """;
        var fixedSource = $$"""
            class C
            {
                int M(bool condition)
                {
                    return {{fixedExpression.ReplaceLineEndings("\r\n")}};
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = equivalenceKey,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the expanded fix for a single conditional and preserves inline comments.
    /// </summary>
    /// <param name="lineEnding">The newline convention.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("\r\n")]
    [InlineData("\n")]
    public async Task TestExpandedFixWithCommentsAsync(string lineEnding)
    {
        var source = """
            class C
            {
                int M(bool condition)
                {
                    return condition /* condition */ {|#0:?|} /* true */ 1 :
                        /* false */ 2;
                }
            }
            """.ReplaceLineEndings(lineEnding);
        var fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    return condition /* condition */
                        ? /* true */ 1
                        : /* false */ 2;
                }
            }
            """.ReplaceLineEndings(lineEnding);
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "Expanded",
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that flattening cannot discard comments, directives, or literal newlines.
    /// </summary>
    /// <param name="expression">The expression whose trivia or literal must be preserved.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("condition {|#0:?|} 1 : // keep\n            2")]
    [InlineData("condition {|#0:?|} 1 : /* keep\n            this */ 2")]
    [InlineData("condition {|#0:?|} 1 :\n#if true\n            2\n#else\n            3\n#endif\n")]
    public async Task TestUnsafeTriviaHasNoFixAsync(string expression)
    {
        var source = $$"""
            class C
            {
                int M(bool condition)
                {
                    return {{expression}};
                }
            }
            """;
        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), source, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that multiline string contents are never rewritten.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestMultilineLiteralHasNoFixAsync()
    {
        const string source = """
            class C
            {
                string M(bool condition)
                {
                    return condition {|#0:?|} @"first
            second" : "third";
                }
            }
            """;
        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), source, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies indentation with tabs and non-default tab widths.
    /// </summary>
    /// <param name="tabSize">The visual width of a tab.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    public async Task TestTabsAsync(int tabSize)
    {
        const string source = "class C\n{\n\tint M(bool condition)\n\t{\n\t\treturn condition {|#0:?|} 1 :\n\t\t2;\n\t}\n}";
        const string fixedSource = "class C\n{\n\tint M(bool condition)\n\t{\n\t\treturn condition\n\t\t\t? 1\n\t\t\t: 2;\n\t}\n}";
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "Expanded",
            UseTabs = true,
            TabSize = tabSize,
            IndentationSize = tabSize,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies fixes in several expressions, including a parenthesized true branch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestFixAllAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    var x = condition {|#0:?|} 1 :
                        2;
                    return condition {|#1:?|} (condition {|#2:?|} 3 :
                        4) : x;
                }
            }
            """;
        const string fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    var x = condition ? 1 : 2;
                    return condition ? (condition ? 3 : 4) : x;
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "SingleLine",
        };
        test.ExpectedDiagnostics.AddRange(new[]
        {
            Diagnostic().WithLocation(0),
            Diagnostic().WithLocation(1),
            Diagnostic().WithLocation(2),
        });
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that expanding several independent expressions works with Fix All.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestExpandedFixAllAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    var x = condition {|#0:?|} 1 :
                        2;
                    return condition {|#1:?|} x :
                        3;
                }
            }
            """;
        const string fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    var x = condition
                        ? 1
                        : 2;
                    return condition
                        ? x
                        : 3;
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "Expanded",
        };
        test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) });
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies aligned-chain fixes with tabs, comments, and three conditions.
    /// </summary>
    /// <param name="tabSize">The visual width of a tab.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData(4)]
    [InlineData(8)]
    public async Task TestAlignedChainWithTabsAsync(int tabSize)
    {
        const string source = "class C\n{\n\tint M(bool condition)\n\t{\n\t\treturn condition {|#0:?|} 1 : /* keep */\n\t\tcondition ? 2 : condition ? 3 : 4;\n\t}\n}";
        const string fixedSource = "class C\n{\n\tint M(bool condition)\n\t{\n\t\treturn condition ? 1 : /* keep */\nALIGNcondition ? 2 :\nALIGNcondition ? 3 :\nALIGN4;\n\t}\n}";
        var alignment = tabSize == 4 ? "\t\t\t   " : "\t\t       ";
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource.Replace("ALIGN", alignment),
            CodeActionEquivalenceKey = "Chain",
            UseTabs = true,
            TabSize = tabSize,
            IndentationSize = tabSize,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the single-line fix preserves inline comments and exterior trivia.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestSingleLineFixWithCommentsAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    // Before
                    return condition {|#0:?|} /* true */ 1 : /* false */
                        2; // After
                }
            }
            """;
        const string fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    // Before
                    return condition ? /* true */ 1 : /* false */ 2; // After
                }
            }
            """;
        await VerifyCSharpFixAsync(source, Diagnostic().WithLocation(0), fixedSource, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that malformed conditionals are left to the compiler.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestMalformedExpressionAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    return condition
                        ? 1 :{|#0:;|}
                }
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.CompilerError("CS1525").WithLocation(0), CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that an expanded fix does not repeatedly re-expand a nested true branch.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestExpandedNestedFixAllAsync()
    {
        const string source = """
            class C
            {
                int M(bool condition)
                {
                    return condition {|#0:?|} (condition {|#1:?|} 1 :
                        2) : 3;
                }
            }
            """;
        const string fixedSource = """
            class C
            {
                int M(bool condition)
                {
                    return condition
                        ? (condition ? 1 : 2)
                        : 3;
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "Expanded",
        };
        test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) });
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
