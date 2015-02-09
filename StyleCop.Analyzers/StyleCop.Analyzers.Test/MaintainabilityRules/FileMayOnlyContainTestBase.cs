using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using TestHelper;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public abstract class FileMayOnlyContainTestBase : CodeFixVerifier
    {
        public abstract string Keyword { get; }
        public abstract string DiagnosticId { get; }

        protected string Message
        {
            get
            {
                return "File may only contain a single " + this.Keyword;
            }
        }

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOneElement()
        {
            var testCode = @"%1 Foo
{
}";
            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestTwoElements()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = this.Message,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, this.Keyword.Length + 2)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestThreeElements()
        {
            var testCode = @"%1 Foo
{
}
%1 Bar
{
}
%1 FooBar
{
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = this.Message,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, this.Keyword.Length + 2)
                        }
                },
                new DiagnosticResult
                {
                    Id = this.DiagnosticId,
                    Message = this.Message,
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 7, this.Keyword.Length + 2)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode.Replace("%1", this.Keyword), expected, CancellationToken.None);
        }
    }
}
