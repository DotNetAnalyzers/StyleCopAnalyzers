// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
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

        [Fact]
        public async Task TestCodeFixMultipleNodesAsync()
        {
            // Tests if the code fix works if the SourceSpan of the diagnostic has more then one SyntaxNode associated with it
            // In this case it is a InterpolatedStringInsert and the StringLiteralExpression
            string oldSource = @"public class Foo
{
    public void Bar()
    {
        string test = $""{""""}"";
    }
}";
            string newSource = @"public class Foo
{
    public void Bar()
    {
        string test = $""{string.Empty}"";
    }
}";

            var expected = Diagnostic().WithLocation(5, 26);
            await VerifyCSharpFixAsync(oldSource, expected, newSource, CancellationToken.None).ConfigureAwait(false);
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
