// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public partial class SA1500CSharp8UnitTests : SA1500CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that a switch expression may be followed by a null-coalescing operator without a new line.
        /// </summary>
        /// <param name="whitespace">The whitespace before the null-coalescing operator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(" ")]
        [InlineData("\r\n        ")]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestSwitchExpressionFollowedByNullCoalescingOperatorAsync(string whitespace)
        {
            var testCode = $$"""

                #nullable enable
                using System;

                public class TestClass
                {
                    public string Test(int n)
                    {
                        var s0 = n switch
                        {
                            0 => GetNonNullString(),
                            1 => GetNullableString(),
                            _ => throw new NotSupportedException(),
                        }{{whitespace}}?? throw new InvalidOperationException("Could not resolve valid string.");
                        return s0;
                    }

                    private static string GetNonNullString() => throw new NotImplementedException();

                    private static string? GetNullableString() => throw new NotImplementedException();
                }

                """;

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestSwitchExpressionBracesMustOwnLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public int Test(Item value)
    {
        return value switch {|#0:{|}
            { Prop: 1 } => 1,
            _ => 0 {|#1:}|};
    }
}

public class Item
{
    public int Prop { get; set; }
}
";

            var fixedCode = @"
public class TestClass
{
    public int Test(Item value)
    {
        return value switch
        {
            { Prop: 1 } => 1,
            _ => 0
        };
    }
}

public class Item
{
    public int Prop { get; set; }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a switch expression entirely on one line is allowed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestSingleLineSwitchExpressionAsync()
        {
            var testCode = """
                public class TestClass
                {
                    public char Test(int a)
                    {
                        var r = a switch { 1 => 'a', 2 => 'b', _ => 'c' };
                        return r;
                    }
                }
                """;

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multiline switch expression's closing brace cannot share a line with its final arm.
        /// </summary>
        /// <param name="comma">The optional trailing comma after the final arm.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("")]
        [InlineData(",")]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestSwitchExpressionFinalArmMustNotShareClosingBraceLineAsync(string comma)
        {
            var testCode = $$"""
                public class TestClass
                {
                    public string Test(int n) =>
                        n switch
                        {
                            _ => "first"{{comma}} {|#0:}|} ?? "fallback";
                }
                """;

            var fixedCode = $$"""
                public class TestClass
                {
                    public string Test(int n) =>
                        n switch
                        {
                            _ => "first"{{comma}}
                        } ?? "fallback";
                }
                """;

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that Fix All separates nested switch expression closing braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestNestedSwitchExpressionClosingBracesAsync()
        {
            var testCode = """
                public class TestClass
                {
                    public int Test(int n) =>
                        n switch
                        {
                            _ => n switch
                            {
                                _ => 1 {|#0:}|} {|#1:}|};
                }
                """;

            var fixedCode = """
                public class TestClass
                {
                    public int Test(int n) =>
                        n switch
                        {
                            _ => n switch
                            {
                                _ => 1
                            }
                        };
                }
                """;

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a closing brace cannot share the final line of a multiline string arm.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestMultilineStringArmClosingBraceAsync()
        {
            var testCode = """
                public class TestClass
                {
                    public string Test(int n) =>
                        n switch
                        {
                            _ => @"first
                second" {|#0:}|} ?? "fallback";
                }
                """;

            var fixedCode = """
                public class TestClass
                {
                    public string Test(int n) =>
                        n switch
                        {
                            _ => @"first
                second"
                        } ?? "fallback";
                }
                """;

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that operators and comments may continue a switch expression without a new line.
        /// </summary>
        /// <param name="continuation">The continuation after the switch expression.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(" + 1")]
        [InlineData(" /* comment */ + 1")]
        [InlineData("\r\n        // comment\r\n        + 1")]
        [InlineData(" == 1 ? 2 : 3")]
        [WorkItem(4332, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4332")]
        public async Task TestSwitchExpressionContinuationAsync(string continuation)
        {
            var testCode = $$"""

                public class TestClass
                {
                    public int Test(int n) =>
                        n switch
                        {
                            _ => 0,
                        }{{continuation}};
                }

                """;

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternBracesMustOwnLineAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test(object value)
    {
        return value is TestType {|#0:{|} A: 1,
            B: 2 {|#1:}|};
    }
}

public class TestType
{
    public int A { get; set; }

    public int B { get; set; }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool Test(object value)
    {
        return value is TestType
        {
            A: 1,
            B: 2
        };
    }
}

public class TestType
{
    public int A { get; set; }

    public int B { get; set; }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
