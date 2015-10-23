// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1122UseStringEmptyForEmptyStrings"/> and
    /// <see cref="SA1122CodeFixProvider"/>.
    /// </summary>
    public class SA1122UnitTests : CodeFixVerifier
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpFixAsync(oldSource, newSource).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 23);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 24);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 15)
            };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? EmptyDiagnosticResults : expected, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 16)
            };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? EmptyDiagnosticResults : expected, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpFixAsync(string.Format(oldSource, useVerbatimLiteral ? "@" : string.Empty), newSource).ConfigureAwait(false);
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 36);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 28);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies the analyzer will properly handle an empty string in a case label.
        /// This is a regression test for <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1281">DotNetAnalyzers/StyleCopAnalyzers#1281</see>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1122UseStringEmptyForEmptyStrings();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1122CodeFixProvider();
        }
    }
}
