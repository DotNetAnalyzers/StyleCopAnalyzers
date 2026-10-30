// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
using StyleCop.Analyzers.Test.Helpers;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1143ConditionalExpressionsMustBeFormattedConsistently,
    StyleCop.Analyzers.ReadabilityRules.SA1143CodeFixProvider>;

/// <summary>
/// Tests formatting of conditional expressions with C# 12 syntax.
/// </summary>
public partial class SA1143CSharp12UnitTests : SA1143CSharp11UnitTests
{
    /// <summary>
    /// Verifies that the collection expression in the reported example keeps its multiline layout.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    [WorkItem(4390, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4390")]
    public async Task TestCollectionExpressionDelimiterAlignmentAsync()
    {
        const string source = """
            using System;

            Console.WriteLine($"[{string.Join(", ", Foo(true))}]");

            static string[] Foo(bool condition) =>
                condition
                    {|#0:?|} [
                        "a",
                        "b",
                    ]
                    : [];
            """;
        const string fixedSource = """
            using System;

            Console.WriteLine($"[{string.Join(", ", Foo(true))}]");

            static string[] Foo(bool condition) =>
                condition
                    ? [
                        "a",
                        "b",
                      ]
                    : [];
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
            TestState = { OutputKind = OutputKind.ConsoleApplication },
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies nested collection delimiters while preserving raw strings and unrelated branch formatting.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestNestedCollectionDelimiterAlignmentAsync()
    {
        const string source = """"
            class C
            {
                string[][] M(bool condition)
                {
                    return condition
                        {|#0:?|} [
                            [
                                """
                                ] ) }
                                """,
                                condition ? "a" :
                                condition ? "b" :
                                "c",
                                condition ? "d" : "e",

                                "last", // keep
                        ],
                        ]
                        : [
                                [ "unchanged" ]
                            ];
                }
            }
            """";
        const string fixedSource = """"
            class C
            {
                string[][] M(bool condition)
                {
                    return condition
                        ? [
                            [
                                """
                                ] ) }
                                """,
                                condition ? "a" :
                                condition ? "b" :
                                "c",
                                condition ? "d" : "e",

                                "last", // keep
                            ],
                          ]
                        : [
                                [ "unchanged" ]
                            ];
                }
            }
            """";
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that delimiter alignment observes tab widths without changing the branch's other indentation.
    /// </summary>
    /// <param name="tabSize">The visual width of a tab.</param>
    /// <param name="useTabs">Whether generated alignment uses tabs.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData(4, true)]
    [InlineData(8, true)]
    [InlineData(4, false)]
    [InlineData(8, false)]
    public async Task TestCollectionDelimiterAlignmentWithTabsAsync(int tabSize, bool useTabs)
    {
        const string source = "class C\n{\n\tstring[] M(bool condition)\n\t{\n\t\treturn condition\n\t\t\t{|#0:?|} [\n\t\t\t\t\"a\",\n\t\t\t]\n\t\t\t: [];\n\t}\n}";
        const string fixedSource = "class C\n{\n\tstring[] M(bool condition)\n\t{\n\t\treturn condition\n\t\t\t? [\n\t\t\t\t\"a\",\nALIGN]\n\t\t\t: [];\n\t}\n}";
        var alignment = useTabs ? "\t\t\t  " : new string(' ', (3 * tabSize) + 2);
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource.Replace("ALIGN", alignment),
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
            UseTabs = useTabs,
            TabSize = tabSize,
            IndentationSize = 2,
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that collection expressions retain both existing layout choices.
    /// </summary>
    /// <param name="equivalenceKey">The existing layout choice.</param>
    /// <param name="fixedExpression">The corresponding formatted expression.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Theory]
    [InlineData("SingleLine", "condition ? [ \"a\", \"b\", ] : []")]
    [InlineData("Expanded", "condition\n            ? [ \"a\", \"b\", ]\n            : []")]
    public async Task TestCollectionExpressionExistingLayoutsAsync(string equivalenceKey, string fixedExpression)
    {
        const string source = """
            class C
            {
                string[] M(bool condition)
                {
                    return condition
                        {|#0:?|} [
                            "a",
                            "b",
                        ]
                        : [];
                }
            }
            """;
        var fixedSource = $$"""
            class C
            {
                string[] M(bool condition)
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
    /// Verifies that a partial delimiter correction is not offered for underindented collection elements.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestUnderindentedCollectionElementsHaveNoAlignmentFixAsync()
    {
        const string source = """
            class C
            {
                string[] M(bool condition)
                {
                    return condition
                        {|#0:?|} [
                        "a",
                        ]
                        : [];
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = source,
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that an alignment action shared by nested diagnostics cannot leave a nested violation behind.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestInvalidNestedConditionalHasNoAlignmentFixAsync()
    {
        const string source = """
            class C
            {
                string[] M(bool condition)
                {
                    return condition
                        {|#0:?|} [
                            condition {|#1:?|} "a" :
                                "b",
                        ]
                        : [];
                }
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = source,
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
        };
        test.ExpectedDiagnostics.AddRange(new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) });
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies alignment when a collection element shares the conditional expression's span.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestDelimiterAlignmentInCollectionElementAsync()
    {
        const string source = """
            class C
            {
                int[][] M(bool condition) =>
                    [
                        condition
                            {|#0:?|} [
                                1,
                            ]
                            : [],
                    ];
            }
            """;
        const string fixedSource = """
            class C
            {
                int[][] M(bool condition) =>
                    [
                        condition
                            ? [
                                1,
                              ]
                            : [],
                    ];
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "AlignBranchDelimiters",
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
