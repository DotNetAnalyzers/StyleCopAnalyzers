// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

public partial class SA1118CSharp8UnitTests : SA1118CSharp7UnitTests
{
    /// <summary>
    /// Verifies that switch expressions can span multiple lines in method arguments.
    /// </summary>
    /// <param name="governingExpression">The single-line governing expression.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData("value")]
    [InlineData("(value + 1)")]
    [InlineData("value /* comment */")]
    [WorkItem(3541, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3541")]
    public async Task TestMultilineSwitchExpressionMethodArgumentsAsync(string governingExpression)
    {
        var testCode = $$"""
            class C
            {
                void M(int first, int second, int third) { }
                void Test(int value)
                {
                    M((value +
                        1) switch
                    {
                        _ => 1,
                    },
                    {{governingExpression}} switch
                    {
                        0 => 2,
                        _ => 3,
                    },
                    third: value switch
                    {
                        _ => 4,
                    });
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that switch expressions can span multiple lines in indexer arguments.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3541, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3541")]
    public async Task TestMultilineSwitchExpressionIndexerArgumentAsync()
    {
        var testCode = """
            class C
            {
                int this[int first, int second] => 0;
                int Test(int value)
                {
                    return this[0, value switch
                    {
                        0 => 1,
                        _ => 2,
                    }];
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that switch expressions can span multiple lines in constructor initializer arguments.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3541, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3541")]
    public async Task TestMultilineSwitchExpressionConstructorInitializerArgumentAsync()
    {
        var testCode = """
            enum MyEnum { A, B, C }
            class MyClass
            {
                public MyClass(string anyString, MyEnum myEnum) { }
                public MyClass(string anyString, int anyValue)
                    : this(
                        anyString,
                        anyValue switch
                        {
                            2 => MyEnum.C,
                            1 => MyEnum.B,
                            _ => MyEnum.A,
                        })
                {
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the switch expression exemption does not extend to enclosing binary expressions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3541, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3541")]
    public async Task TestMultilineBinaryExpressionContainingSwitchArgumentAsync()
    {
        var testCode = """
            class C
            {
                void M(int first, int second) { }
                void Test(int value)
                {
                    M(0, {|#0:1 + (value switch
                    {
                        0 => 2,
                        _ => 3,
                    })|});
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that the governing expression and switch keyword must share a single line.
    /// </summary>
    /// <param name="switchHeader">The governing expression and switch keyword.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData("value\n            switch")]
    [InlineData("(value +\n            1) switch")]
    [InlineData("(value\n            ) switch")]
    public async Task TestMultilineSwitchExpressionHeaderArgumentAsync(string switchHeader)
    {
        var testCode = $$"""
            class C
            {
                void M(int first, int second) { }
                void Test(int value)
                {
                    M(0, {|#0:{{switchHeader}}
                    {
                        0 => 1,
                        _ => 2,
                    }|});
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that interpolated verbatim strings also support the reversed prefix order.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineVerbatimStringWithReversedPrefixArgumentAsync()
    {
        await VerifyMultilineStringArgumentAsync("@$\"first {1}\nsecond\"").ConfigureAwait(false);
    }
}
