using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.NamingRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.NamingRules
{
    [TestClass]
    public class SA1303UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1303ConstFieldNamesMustBeginWithUpperCaseLetter.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstFieldStatingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public const string bar;
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Const field names must begin with upper-case letter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 25)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstFieldStatingWithUpperCase()
        {
            var testCode = @"public class Foo
{
    public const string Bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestFieldWhichIsNotConstStatingWithLowerCase()
        {
            var testCode = @"public class Foo
{
    public string bar;
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1303ConstFieldNamesMustBeginWithUpperCaseLetter();
        }
    }
}