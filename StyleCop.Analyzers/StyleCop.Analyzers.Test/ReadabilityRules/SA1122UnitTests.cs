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
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestEmptyStringLiteralAsync(bool useVerbatimLiteral)
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

        private async Task TestParenthesizedEmptyStringLiteralAsync(bool useVerbatimLiteral)
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

        private async Task TestLocalStringLiteralImplAsync(bool useVerbatimLiteral, bool isConst)
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

        private async Task TestParenthesizedLocalStringLiteralImplAsync(bool useVerbatimLiteral, bool isConst)
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

        private async Task TestAttributeStringLiteralImplAsync(bool useVerbatimLiteral)
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

        private async Task TestDefaultParameterStringLiteralImplAsync(bool useVerbatimLiteral)
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

        public async Task TestNullInMethodAsync()
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

        private async Task TestSimpleCodeFixAsync(bool useVerbatimLiteral)
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
        public async Task TestCodeFixMultipleNodesAsync()
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
        public async Task TestLiteralInMethodVerbatimAsync()
        {
            await this.TestEmptyStringLiteralAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethodAsync()
        {
            await this.TestEmptyStringLiteralAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethodVerbatimAsync()
        {
            await this.TestParenthesizedEmptyStringLiteralAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethodAsync()
        {
            await this.TestParenthesizedEmptyStringLiteralAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalStringLiteralVerbatimAsync()
        {
            await this.TestLocalStringLiteralImplAsync(true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalStringLiteralAsync()
        {
            await this.TestLocalStringLiteralImplAsync(false, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstStringLiteralVerbatimAsync()
        {
            await this.TestLocalStringLiteralImplAsync(true, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstStringLiteralAsync()
        {
            await this.TestLocalStringLiteralImplAsync(false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteralVerbatimAsync()
        {
            await this.TestParenthesizedLocalStringLiteralImplAsync(true, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteralAsync()
        {
            await this.TestParenthesizedLocalStringLiteralImplAsync(false, false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteralVerbatimAsync()
        {
            await this.TestParenthesizedLocalStringLiteralImplAsync(true, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteralAsync()
        {
            await this.TestParenthesizedLocalStringLiteralImplAsync(false, true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeStringLiteralVerbatimAsync()
        {
            await this.TestAttributeStringLiteralImplAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeStringLiteralAsync()
        {
            await this.TestAttributeStringLiteralImplAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteralVerbatimAsync()
        {
            await this.TestDefaultParameterStringLiteralImplAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteralAsync()
        {
            await this.TestDefaultParameterStringLiteralImplAsync(false).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethodVerbatimCodeFixAsync()
        {
            await this.TestSimpleCodeFixAsync(true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralInMethodCodeFixAsync()
        {
            await this.TestSimpleCodeFixAsync(false).ConfigureAwait(false);
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
