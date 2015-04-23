namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1027TabsMustNotBeUsed"/>
    /// </summary>

    public class SA1027UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that tabs used inside string and char literals are not producing diagnostics.
        /// </summary>
        [Fact]
        public async Task TestValidTabs()
        {
            var testCode =
                "public class Foo\r\n" +
                "{\r\n" +
                "    public const string ValidTestString = \"\tText\";\r\n" +
                "    public const char ValidTestChar = '\t';\r\n" +
                "}\r\n";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that tabs used inside disabled code is not producing diagnostics.
        /// </summary>
        [Fact]
        public async Task TestDisabledCode()
        {
            var testCode =
                "public class Foo\r\n" +
                "{\r\n" +
                "#if false\r\n" +
                "\tpublic const string ValidTestString = \"Text\";\r\n" +
                "\tpublic const char ValidTestChar = 'c';\r\n" +
                "#endif\r\n" +
                "}\r\n";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestInvalidTabs()
        {
            var testCode =
                "using\tSystem.Diagnostics;\r\n" +
                "\r\n" +
                "public\tclass\tFoo\r\n" +
                "{\r\n" +
                "\tpublic void Bar()\r\n" +
                "\t{\r\n" +
                "\t  \t// Comment\r\n" +
                "\t \tDebug.Indent();\r\n" +
                "   \t}\r\n" +
                "}\r\n";

            var fixedTestCode = @"using   System.Diagnostics;

public  class   Foo
{
    public void Bar()
    {
        // Comment
        Debug.Indent();
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 6),
                this.CSharpDiagnostic().WithLocation(3, 7),
                this.CSharpDiagnostic().WithLocation(3, 13),
                this.CSharpDiagnostic().WithLocation(5, 1),
                this.CSharpDiagnostic().WithLocation(6, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
                this.CSharpDiagnostic().WithLocation(8, 1),
                this.CSharpDiagnostic().WithLocation(9, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1027TabsMustNotBeUsed();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1027CodeFixProvider();
        }
    }
}
