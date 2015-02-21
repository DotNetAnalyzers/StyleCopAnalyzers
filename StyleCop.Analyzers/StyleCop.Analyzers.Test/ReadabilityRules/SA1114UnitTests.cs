using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.ReadabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    [TestClass]
    public class SA1114UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1114ParameterListMustFollowDeclaration.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar(

string s)
    {

    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationNoParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar(

)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(

1);
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(
1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallNoParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var i = 1.ToString(
                
            );
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1114ParameterListMustFollowDeclaration();
        }
    }
}