using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

using StyleCop.Analyzers.LayoutRules;
using TestHelper;
using Microsoft.CodeAnalysis.CodeFixes;

namespace StyleCop.Analyzers.Test.LayoutRules
{
    /// <summary>
    /// Unit tests for the SA1503: CurlyBracketsMustNotBeOmitted analyzer.
    /// </summary>
    [TestClass]
	public class SA1503UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1503CurlyBracketsMustNotBeOmitted.DiagnosticId;
        private const string IfTestStatement = "if (i == 0)";
        private const string WhileTestStatement = "while (i == 0)";
        private const string ForTestStatement = "for (var j = 0; j < i; j++)";
        private const string ForEachTestStatement = "foreach (var j in new[] { 1, 2, 3 })";

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestIfStatementWithoutCurlyBrackets()
        {
            await this.TestStatementWithoutCurlyBrackets(IfTestStatement);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestIfStatementWithCurlyBrackets()
        {
            await this.TestStatementWithCurlyBrackets(IfTestStatement);
        }

        /// <summary>
        /// Verifies that an if / else statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestIfElseStatementWithoutCurlyBrackets()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Curly brackets must not be omitted",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 13) }
                },

                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Curly brackets must not be omitted",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestIfElseStatementWithCurlyBrackets()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestWhileStatementWithoutCurlyBrackets()
        {
            await this.TestStatementWithoutCurlyBrackets(WhileTestStatement);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestWhileStatementWithCurlyBrackets()
        {
            await this.TestStatementWithCurlyBrackets(WhileTestStatement);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestForStatementWithoutCurlyBrackets()
        {
            await this.TestStatementWithoutCurlyBrackets(ForTestStatement);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestForStatementWithCurlyBrackets()
        {
            await this.TestStatementWithCurlyBrackets(ForTestStatement);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestForEachStatementWithoutCurlyBrackets()
        {
            await this.TestStatementWithoutCurlyBrackets(ForEachTestStatement);
        }

        /// <summary>
        /// Verifies that a while statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns></returns>
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestForEachStatementWithCurlyBrackets()
        {
            await this.TestStatementWithCurlyBrackets(ForEachTestStatement);
        }

        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderForIfStatement()
        {
            await this.TestCodeFixForStatement(IfTestStatement);
        }

        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderForIfElseStatement()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
            Debug.Assert(false);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderForWhileStatement()
        {
            await this.TestCodeFixForStatement(WhileTestStatement);
        }

        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderForForStatement()
        {
            await this.TestCodeFixForStatement(ForTestStatement);
        }

        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderForForEachStatement()
        {
            await this.TestCodeFixForStatement(ForEachTestStatement);
        }

        // VW: Not sure if this is a valid test at all.
        [TestMethod, TestCategory("MaintainabilityRules/SA1503")]
        public async Task TestCodeFixProviderWithAlternateIndentation()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
 }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
  {
   Debug.Assert(true);
  }
 }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        private async Task TestStatementWithoutCurlyBrackets(string statementText)
        {
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Curly brackets must not be omitted",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 13) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(this.GenerateTestStatement(statementText), expected, CancellationToken.None);
        }

        private async Task TestStatementWithCurlyBrackets(string statementText)
        {
            await this.VerifyCSharpDiagnosticAsync(this.GenerateFixedTestStatement(statementText), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestCodeFixForStatement(string statementText)
        {
            await this.VerifyCSharpFixAsync(this.GenerateTestStatement(statementText), this.GenerateFixedTestStatement(statementText));
        }

        private string GenerateTestStatement(string statementText)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        #STATEMENT#
            Debug.Assert(true);
    }
}";
            return testCodeFormat.Replace("#STATEMENT#", statementText);
        }

        private string GenerateFixedTestStatement(string statementText)
        {
            var fixedTestCodeFormat = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        #STATEMENT#
        {
            Debug.Assert(true);
        }
    }
}";
            return fixedTestCodeFormat.Replace("#STATEMENT#", statementText);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1503CurlyBracketsMustNotBeOmitted();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1503CodeFixProvider();
        }
    }
}
