using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    [TestClass]
    public class SA1410UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1410RemoveDelegateParenthesisWhenPossible.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMissingParenthesis()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TesNonEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int, int> getNumber = delegate (int i) { return i; };
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestEmptyParameterList()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";
            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Remove delegate parenthesis when possible",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 52)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCodeFix()
        {
            var oldSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate() { return 3; };
    }
}";

            var newSource = @"public class Foo
{
    public void Bar()
    {
        System.Func<int> getRandomNumber = delegate { return 3; };
    }
}";

            await VerifyCSharpFixAsync(oldSource, newSource, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1410RemoveDelegateParenthesisWhenPossible();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1410SA1411CodeFixProvider();
        }
    }
}