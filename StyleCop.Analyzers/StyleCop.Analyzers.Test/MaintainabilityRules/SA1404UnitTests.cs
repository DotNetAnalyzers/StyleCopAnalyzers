using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using System;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1404UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1404CodeAnalysisSuppressionMustHaveJustification.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }


        public async Task TestSuppressionWithStringLiteral()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""a justification"")]
    public string Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithNoJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 6)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithEmptyJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """")]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 66)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithWhitespaceJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""    "")]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 66)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithNullJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = null)]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 3, 66)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithComplexJustification()
        {
            var testCode = @"public class Foo
{
    const string JUSTIFICATION = ""Foo"";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """" + JUSTIFICATION)]
    public string Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithComplexWhitespaceJustification()
        {
            var testCode = @"public class Foo
{
    const string JUSTIFICATION = ""    "";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """" + JUSTIFICATION)]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 66)
                        }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDiagnosticDoesNotThrowNullReferenceForWrongConstantType()
        {
            var testCode = @"public class Foo
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = 5)]
    public string Bar()
    {

    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Code analysis suppression must have justification",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 66)
                        }
                }
            };
            try
            {
                await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            }
            catch (NullReferenceException)
            {
                Assert.True(false, "Diagnostic threw NullReferenceException");
            }
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1404CodeAnalysisSuppressionMustHaveJustification();
        }
    }
}