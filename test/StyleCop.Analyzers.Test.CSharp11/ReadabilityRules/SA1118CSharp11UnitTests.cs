// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

public partial class SA1118CSharp11UnitTests : SA1118CSharp10UnitTests
{
    /// <summary>
    /// Verifies the reported migration constraint arguments using raw and verbatim strings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineStringCheckConstraintAsync()
    {
        var testCode = """"
            class MigrationBuilder
            {
                void AddCheckConstraint(string name, string table, string sql) { }
                void Test(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.AddCheckConstraint(
                        "CK_TableName_ColumnName",
                        "TableName",
                        """
                        (
                                [Value1] IS NULL
                            AND [Value2] IS NOT NULL
                        )
                        """);

                    migrationBuilder.AddCheckConstraint(
                        "CK_TableName_ColumnName",
                        "TableName",
                        @"
                        (
                                [Value1] IS NULL
                            AND [Value2] IS NOT NULL
                        )
                        ");
                }
            }
            """";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that raw strings and interpolation expressions can span multiple lines.
    /// </summary>
    /// <param name="expression">The string expression to pass as an argument.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData("\"\"\"\nfirst\nsecond\n\"\"\"")]
    [InlineData("$\"\"\"\nfirst {1}\nsecond\n\"\"\"")]
    [InlineData("$$\"\"\"\n{ \"value\": {{1}} }\n\"\"\"")]
    [InlineData("$$$\"\"\"\"\n{{ \"value\": {{{1}}} }}\n\"\"\"\"")]
    [InlineData("$\"first {1 +\n2} second\"")]
    [InlineData("$\"\"\"first {1 +\n2} second\"\"\"")]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineRawOrInterpolatedStringArgumentAsync(string expression)
    {
        await VerifyMultilineStringArgumentAsync(expression).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that multiline UTF-8 string literals receive the same exemption.
    /// </summary>
    /// <param name="expression">The UTF-8 string expression to pass as an argument.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Theory]
    [InlineData("@\"first\nsecond\"u8")]
    [InlineData("\"\"\"\nfirst\nsecond\n\"\"\"u8")]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineUtf8StringArgumentAsync(string expression)
    {
        var testCode = $$"""
            using System;
            class C
            {
                C(int value, ReadOnlySpan<byte> text) { }
                ReadOnlySpan<byte> this[int value, ReadOnlySpan<byte> text] { get { return text; } }
                void M(int value, ReadOnlySpan<byte> text) { }
                void Test()
                {
                    M(0, {{expression}});
                    M(0, text: {{expression}});
                    var instance = new C(0, {{expression}});
                    var result = this[0, {{expression}}];
                }
            }
            """;

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies the raw string exemption in positional and named attribute arguments.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineRawStringAttributeArgumentAsync()
    {
        var testCode = """"
            [Example(0, """
                first
                second
                """, Text = $"""
                third
                fourth
                """)]
            class ExampleAttribute : System.Attribute
            {
                public ExampleAttribute(int value, string text) { }
                public string Text { get; set; }
            }
            """";

        await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that concatenating raw strings still reports a multiline argument.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineRawStringConcatenationArgumentAsync()
    {
        var testCode = """"
            class C
            {
                void M(int value, string text) { }
                void Test()
                {
                    M(0, {|#0:"""
                        first
                        second
                        """ +
                        "third"|});
                }
            }
            """";

        await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(0), CancellationToken.None).ConfigureAwait(false);
    }
}
