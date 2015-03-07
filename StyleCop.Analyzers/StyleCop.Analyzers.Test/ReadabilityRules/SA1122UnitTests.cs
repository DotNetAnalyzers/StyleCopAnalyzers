namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1122UseStringEmptyForEmptyStrings"/> and
    /// <see cref="SA1122CodeFixProvider"/>.
    /// </summary>
    public class SA1122UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1122UseStringEmptyForEmptyStrings.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 23)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None);
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 24)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None);
        }

        private async Task TestLocalStringLiteral(bool useVerbatimLiteral, bool isConst)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        {1}
string test = {0}"""";
    }}
}}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 15)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? EmptyDiagnosticResults : expected, CancellationToken.None);
        }

        private async Task TestParenthesizedLocalStringLiteral(bool useVerbatimLiteral, bool isConst)
        {
            var testCode = @"public class Foo
{{
    public void Bar()
    {{
        {1}
string test = ({0}"""");
    }}
}}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 16)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? EmptyDiagnosticResults : expected, CancellationToken.None);
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
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestAttributeStringLiteral(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    [System.Diagnostics.CodeAnalysis.SuppressMessage({0}"""", 
                                                    Justification = ({0}""""))]
    public void Bar()
    {{
    }}
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDefaultParameterStringLiteral(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    public void Bar(string x = {0}"""", string y = ({0}""""))
    {{
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await this.VerifyCSharpFixAsync(string.Format(oldSource, useVerbatimLiteral ? "@" : string.Empty), newSource);
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

            await this.VerifyCSharpFixAsync(oldSource, newSource);
        }

        [Fact]
        public async Task TestLiteralInMethodVerbatim()
        {
            await this.TestEmptyStringLiteral(true);
        }

        [Fact]
        public async Task TestLiteralInMethod()
        {
            await this.TestEmptyStringLiteral(false);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethodVerbatim()
        {
            await this.TestParenthesizedEmptyStringLiteral(true);
        }

        [Fact]
        public async Task TestParenthesizedLiteralInMethod()
        {
            await this.TestParenthesizedEmptyStringLiteral(false);
        }

        [Fact]
        public async Task TestLocalStringLiteralVerbatim()
        {
            await this.TestLocalStringLiteral(true, false);
        }

        [Fact]
        public async Task TestLocalStringLiteral()
        {
            await this.TestLocalStringLiteral(false, false);
        }

        [Fact]
        public async Task TestConstStringLiteralVerbatim()
        {
            await this.TestLocalStringLiteral(true, true);
        }

        [Fact]
        public async Task TestConstStringLiteral()
        {
            await this.TestLocalStringLiteral(false, true);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteralVerbatim()
        {
            await this.TestParenthesizedLocalStringLiteral(true, false);
        }

        [Fact]
        public async Task TestParenthesizedLocalStringLiteral()
        {
            await this.TestParenthesizedLocalStringLiteral(false, false);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteralVerbatim()
        {
            await this.TestParenthesizedLocalStringLiteral(true, true);
        }

        [Fact]
        public async Task TestParenthesizedConstStringLiteral()
        {
            await this.TestParenthesizedLocalStringLiteral(false, true);
        }

        [Fact]
        public async Task TestAttributeStringLiteralVerbatim()
        {
            await this.TestAttributeStringLiteral(true);
        }

        [Fact]
        public async Task TestAttributeStringLiteral()
        {
            await this.TestAttributeStringLiteral(false);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteralVerbatim()
        {
            await this.TestDefaultParameterStringLiteral(true);
        }

        [Fact]
        public async Task TestDefaultParameterStringLiteral()
        {
            await this.TestDefaultParameterStringLiteral(false);
        }

        [Fact]
        public async Task TestLiteralInMethodVerbatimCodeFix()
        {
            await this.TestSimpleCodeFix(true);
        }

        [Fact]
        public async Task TestLiteralInMethodCodeFix()
        {
            await this.TestSimpleCodeFix(false);
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
