namespace StyleCop.Analyzers.Test.ReadabilityRules
{
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
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestEmptyStringLiteral(bool useVerbatimLiteral)
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

        private async Task TestParenthesizedEmptyStringLiteral(bool useVerbatimLiteral)
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

        private async Task TestLocalStringLiteralImpl(bool useVerbatimLiteral, bool isConst)
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

        private async Task TestParenthesizedLocalStringLiteralImpl(bool useVerbatimLiteral, bool isConst)
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

        public async Task TestWhitespaceStringLiteral(bool useVerbatimLiteral)
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

        private async Task TestAttributeStringLiteralImpl(bool useVerbatimLiteral)
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

        private async Task TestDefaultParameterStringLiteralImpl(bool useVerbatimLiteral)
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

        public async Task TestNullInMethod()
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        string test = null;
    }}
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestSimpleCodeFix(bool useVerbatimLiteral)
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
        public async Task TestCodeFixMultipleNodes()
        {
            // Tests if the code fix works if the SourceSpan of the diagnostic has more then one SynatxNode associated with it
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

        [Fact]
        public async Task TestLiteralInMethodVerbatim()
        {
            await this.TestEmptyStringLiteral(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethod()
        {
            await this.TestEmptyStringLiteral(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethodVerbatim()
        {
            await this.TestParenthesizedEmptyStringLiteral(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethod()
        {
            await this.TestParenthesizedEmptyStringLiteral(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalStringLiteralVerbatim()
        {
            await this.TestLocalStringLiteralImpl(true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalStringLiteral()
        {
            await this.TestLocalStringLiteralImpl(false, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstStringLiteralVerbatim()
        {
            await this.TestLocalStringLiteralImpl(true, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstStringLiteral()
        {
            await this.TestLocalStringLiteralImpl(false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteralVerbatim()
        {
            await this.TestParenthesizedLocalStringLiteralImpl(true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteral()
        {
            await this.TestParenthesizedLocalStringLiteralImpl(false, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteralVerbatim()
        {
            await this.TestParenthesizedLocalStringLiteralImpl(true, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteral()
        {
            await this.TestParenthesizedLocalStringLiteralImpl(false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeStringLiteralVerbatim()
        {
            await this.TestAttributeStringLiteralImpl(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeStringLiteral()
        {
            await this.TestAttributeStringLiteralImpl(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteralVerbatim()
        {
            await this.TestDefaultParameterStringLiteralImpl(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteral()
        {
            await this.TestDefaultParameterStringLiteralImpl(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethodVerbatimCodeFix()
        {
            await this.TestSimpleCodeFix(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethodCodeFix()
        {
            await this.TestSimpleCodeFix(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatFixDoesntRemoveTrivia()
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestGetterOnlyPropertyWithInitializer()
        {
            string testCode = @"
class ClassName
{
    string PropertyName { get; } = ""Value"";
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestGetterOnlyPropertyWithEmptyInitializer()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestExpressionPropertyWithLiteralResult()
        {
            string testCode = @"
class ClassName
{
    string PropertyName => ""Value"";
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestExpressionPropertyWithEmptyLiteralResult()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1122CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1122UseStringEmptyForEmptyStrings();
        }
    }
}
