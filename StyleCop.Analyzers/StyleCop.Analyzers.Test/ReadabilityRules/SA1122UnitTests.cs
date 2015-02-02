namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1122UseStringEmptyForEmptyStrings"/> and
    /// <see cref="SA1122CodeFixProvider"/>.
    /// </summary>
    [TestClass]
    public class SA1122UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1122UseStringEmptyForEmptyStrings.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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
                        Id = DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 23)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), expected, CancellationToken.None);
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
                        Id = DiagnosticId,
                        Message = "Use string.Empty for empty strings",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 15)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty, isConst ? "const" : string.Empty), isConst ? EmptyDiagnosticResults : expected, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
        }

        
        private async Task TestAttributeStringLiteral(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    [System.Diagnostics.CodeAnalysis.SuppressMessage({0}"""", 
                                                    Justification = {0}"""")]
    public void Bar()
    {{
    }}
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestDefaultParameterStringLiteral(bool useVerbatimLiteral)
        {
            var testCode = @"using System.Diagnostics.CodeAnalysis;
public class Foo
{{
    public void Bar(string value = {0}"""")
    {{
    }}
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, useVerbatimLiteral ? "@" : string.Empty), EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

            await VerifyCSharpFixAsync(string.Format(oldSource, useVerbatimLiteral ? "@" : string.Empty), newSource);
        }

        [TestMethod]
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

            await VerifyCSharpFixAsync(oldSource, newSource);
        }

        [TestMethod]
        public async Task TestLiteralInMethodVerbatim()
        {
            await TestEmptyStringLiteral(true);
        }

        [TestMethod]
        public async Task TestLiteralInMethod()
        {
            await TestEmptyStringLiteral(false);
        }

        [TestMethod]
        public async Task TestLocalStringLiteralVerbatim()
        {
            await TestLocalStringLiteral(true, false);
        }

        [TestMethod]
        public async Task TestLocalStringLiteral()
        {
            await TestLocalStringLiteral(false, false);
        }

        [TestMethod]
        public async Task TestConstStringLiteralVerbatim()
        {
            await TestLocalStringLiteral(true, true);
        }

        [TestMethod]
        public async Task TestConstStringLiteral()
        {
            await TestLocalStringLiteral(false, true);
        }

        [TestMethod]
        public async Task TestAttributeStringLiteralVerbatim()
        {
            await TestAttributeStringLiteral(true);
        }

        [TestMethod]
        public async Task TestAttributeStringLiteral()
        {
            await TestAttributeStringLiteral(false);
        }

        [TestMethod]
        public async Task TestDefaultParameterStringLiteralVerbatim()
        {
            await TestDefaultParameterStringLiteral(true);
        }

        [TestMethod]
        public async Task TestDefaultParameterStringLiteral()
        {
            await TestDefaultParameterStringLiteral(false);
        }

        [TestMethod]
        public async Task TestLiteralInMethodVerbatimCodeFix()
        {
            await TestSimpleCodeFix(true);
        }

        [TestMethod]
        public async Task TestLiteralInMethodCodeFix()
        {
            await TestSimpleCodeFix(false);
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
