// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        StyleCop.Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    public partial class SA1122CSharp11UnitTests : SA1122CSharp10UnitTests
    {
        /// <summary>
        /// Verifies that an empty UTF-8 string literal is not reported, because it is not a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3996, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3996")]
        public async Task TestEmptyUtf8StringLiteralAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        ReadOnlySpan<byte> value = """"u8;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty string used as a constant pattern for a <see cref="System.ReadOnlySpan{T}"/> of
        /// <see cref="char"/> is not reported, because patterns require constants.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4002, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4002")]
        public async Task TestEmptyStringPatternOnSpanAsync()
        {
            var testCode = @"using System;

class TestClass
{
    int TestMethod(ReadOnlySpan<char> value)
    {
        if (value is """")
        {
            return 0;
        }

        return value switch
        {
            """" => 1,
            ""a"" => 2,
            _ => 3,
        };
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty raw string literals, which C# 11 introduced, are reported and fixed. Only the multi-line
        /// form can be empty, written as a single blank line between the delimiters, and that includes interpolated raw
        /// string literals and ones with longer delimiters. (A <c>$$</c> prefix can't be tested here, because the test
        /// markup uses <c>$$</c> to mark a position.)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyRawStringLiteralAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var test1 = {|#0:""""""

            """"""|};
        var test2 = {|#1:""""""""

            """"""""|};
        var test3 = {|#2:$""""""

            """"""|};
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        var test1 = string.Empty;
        var test2 = string.Empty;
        var test3 = string.Empty;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that raw string literals with any content are not reported. Two blank lines is the boundary of the
        /// empty case: the line break ending the last content line belongs to the closing delimiter, so that value is a
        /// single line break rather than empty.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRawStringLiteralWithContentIsNotReportedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar(string value)
    {
        var test1 = """"""text"""""";
        var test2 = """"""
            text
            """""";
        var test3 = """"""


            """""";
        var test4 = $""""""text"""""";
        var test5 = $""""""{value}"""""";
        var test6 = $""""""
            {value}
            """""";
        var test7 = """"""text""""""u8;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty raw string literals are not reported where the language requires a constant.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyRawStringLiteralAsConstantIsNotReportedAsync()
        {
            var testCode = @"using System.ComponentModel;

public class Foo
{
    private const string TestField = """"""

        """""";

    [Description(""""""

        """""")]
    public void Bar(string value)
    {
        const string test = $""""""

            """""";

        switch (value)
        {
        case """"""

            """""":
            break;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
