namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    [TestClass]
    public class SA1106UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1106CodeMustNotContainEmptyStatements.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyStatementAsBlock()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
            ;
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Code must not contain empty statements",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyStatementInForStatement()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (;;)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyStatement()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        ;
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Code must not contain empty statements",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 9)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLabeledEmptyStatement()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLabeledEmptyStatementFollowedByEmptyStatement()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        ;
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Code must not contain empty statements",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 9)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLabeledEmptyStatementFollowedByNonEmptyStatement()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        int x = 3;
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Code must not contain empty statements",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 9)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1106CodeMustNotContainEmptyStatements();
        }
    }
}
