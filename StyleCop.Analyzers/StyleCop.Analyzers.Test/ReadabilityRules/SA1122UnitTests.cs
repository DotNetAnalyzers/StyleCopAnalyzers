// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1122UseStringEmptyForEmptyStrings,
        StyleCop.Analyzers.ReadabilityRules.SA1122CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1122UseStringEmptyForEmptyStrings"/> and
    /// <see cref="SA1122CodeFixProvider"/>.
    /// </summary>
    public class SA1122UnitTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestWhitespaceStringLiteralAsync(bool useVerbatimLiteral)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        string test = {0}""  "";
    }}
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNullInMethodAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string test = null;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestCodeFixMultipleNodesAsync(string lineEnding)
        {
            // Tests if the code fix works if the SourceSpan of the diagnostic has more then one SyntaxNode associated with it
            // In this case it is a InterpolatedStringInsert and the StringLiteralExpression
            string oldSource = @"public class Foo
{
    public void Bar()
    {
        string test = $""{{|#0:""""|}}"";
    }
}".ReplaceLineEndings(lineEnding);
            string newSource = @"public class Foo
{
    public void Bar()
    {
        string test = $""{string.Empty}"";
    }
}".ReplaceLineEndings(lineEnding);

            var expected = Diagnostic().WithLocation(0);
            await VerifyCSharpFixAsync(oldSource, expected, newSource, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that every form of empty string literal available in C# 6 is reported, including empty interpolated
        /// strings, and that the code fix replaces it with <c>string.Empty</c>. Forms added in later language versions
        /// are covered by the test project for that version.
        /// </summary>
        /// <param name="literal">The empty string literal.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\"\"")]
        [InlineData("@\"\"")]
        [InlineData("$\"\"")]
        [InlineData("$@\"\"")]
        public async Task TestEmptyStringLiteralIsReportedAsync(string literal)
        {
            var testCode = $@"public class Foo
{{
    public void Bar(string value)
    {{
        var test = [|{literal}|];
    }}
}}";
            var fixedCode = @"public class Foo
{
    public void Bar(string value)
    {
        var test = string.Empty;
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that string literals and interpolated strings with any content are not reported, even when the
        /// content is only an interpolation.
        /// </summary>
        /// <param name="literal">The string literal.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\"text\"")]
        [InlineData("@\"text\"")]
        [InlineData("$\"text\"")]
        [InlineData("$@\"text\"")]
        [InlineData("$\"{value}\"")]
        [InlineData("$@\"{value}\"")]
        [InlineData("$\"{{}}\"")]
        public async Task TestStringLiteralIsNotReportedAsync(string literal)
        {
            var testCode = $@"public class Foo
{{
    public void Bar(string value)
    {{
        var test = {literal};
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty string literals are not reported in <see langword="const"/> declarations. Interpolated
        /// strings can only be constants from C# 10 onwards, so they are covered by that test project.
        /// </summary>
        /// <param name="literal">The empty string literal.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\"\"")]
        [InlineData("@\"\"")]
        public async Task TestEmptyStringLiteralAsConstantIsNotReportedAsync(string literal)
        {
            var testCode = $@"public class Foo
{{
    private const string TestField = {literal};

    public void Bar()
    {{
        const string test = {literal};
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that several empty interpolated strings in one expression are all reported and fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleEmptyInterpolatedStringsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar(string value)
    {
        string test = [|$""""|] + value + ([|$@""""|]);
        Baz([|$""""|]);
    }

    public void Baz(object value)
    {
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar(string value)
    {
        string test = string.Empty + value + (string.Empty);
        Baz(string.Empty);
    }

    public void Baz(object value)
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty interpolated string converted to <c>FormattableString</c> or
        /// <see cref="IFormattable"/> is not reported, because <c>string.Empty</c> can't be converted to those types,
        /// while one converted to a type that a string converts to is still reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyInterpolatedStringConvertedToFormattableStringAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        FormattableString formattable = $"""";
        IFormattable formattable2 = $@"""";
        Baz($"""");
        object value = [|$""""|];
        IComparable comparable = [|$""""|];
    }

    public void Baz(FormattableString value)
    {
    }
}";
            var fixedCode = @"using System;

public class Foo
{
    public void Bar()
    {
        FormattableString formattable = $"""";
        IFormattable formattable2 = $@"""";
        Baz($"""");
        object value = string.Empty;
        IComparable comparable = string.Empty;
    }

    public void Baz(FormattableString value)
    {
    }
}";

            // FormattableString first appeared in .NET Framework 4.6.
            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net46.Default,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestEmptyStringLiteralAsync(bool useVerbatimLiteral)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        string test = {0}"""";
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 23);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestParenthesizedEmptyStringLiteralAsync(bool useVerbatimLiteral)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        string test = ({0}"""");
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 24);

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task TestLocalStringLiteralAsync(bool useVerbatimLiteral, bool isConst)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        {1}
string test = {0}"""";
    }}
}}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 15),
            };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? DiagnosticResult.EmptyDiagnosticResults : expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task TestParenthesizedLocalStringLiteralAsync(bool useVerbatimLiteral, bool isConst)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        {1}
string test = ({0}"""");
    }}
}}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 16),
            };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? DiagnosticResult.EmptyDiagnosticResults : expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestAttributeStringLiteralAsync(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    [System.Diagnostics.CodeAnalysis.SuppressMessage({0}"""", ""checkId"",
                                                    Justification = ({0}""""))]
    public void Bar()
    {{
    }}
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestDefaultParameterStringLiteralAsync(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    public void Bar(string x = {0}"""", string y = ({0}""""))
    {{
    }}
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestSimpleCodeFixAsync(bool useVerbatimLiteral)
        {
            string oldSource = @"public class Foo
{{
    public void Bar()
    {{
        string test = {0}"""";
    }}
}}";
            string newSource = @"public class Foo
{
    public void Bar()
    {
        string test = string.Empty;
    }
}";

            var expected = Diagnostic().WithLocation(5, 23);
            await VerifyCSharpFixAsync(string.Format(oldSource, useVerbatimLiteral ? "@" : string.Empty), expected, newSource, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatFixDoesntRemoveTriviaAsync()
        {
            string testCode = @"class Foo
{
    void Bar()
    {
        string test = /*a*/""""/*b*/;
    }
}";
            string fixedCode = @"class Foo
{
    void Bar()
    {
        string test = /*a*/string.Empty/*b*/;
    }
}";

            var expected = Diagnostic().WithLocation(5, 28);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGetterOnlyPropertyWithInitializerAsync()
        {
            string testCode = @"
class ClassName
{
    string PropertyName { get; } = ""Value"";
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGetterOnlyPropertyWithEmptyInitializerAsync()
        {
            string testCode = @"
class ClassName
{
    string PropertyName { get; } = """";
}
";
            string fixedCode = @"
class ClassName
{
    string PropertyName { get; } = string.Empty;
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 36);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExpressionPropertyWithLiteralResultAsync()
        {
            string testCode = @"
class ClassName
{
    string PropertyName => ""Value"";
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestExpressionPropertyWithEmptyLiteralResultAsync()
        {
            string testCode = @"
class ClassName
{
    string PropertyName => """";
}
";
            string fixedCode = @"
class ClassName
{
    string PropertyName => string.Empty;
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 28);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the analyzer will properly handle an empty string in a case label.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1281, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1281")]
        public async Task TestEmptyStringInCaseLabelNotReportedAsync()
        {
            string testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        switch (""Test string"")
        {
        case """":
            break;
        case ("""" + ""a""):
            break;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
