using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.MaintainabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    public class SA1404UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithStringLiteral()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""a justification"")]
    public void Bar()
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
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 6);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithEmptyJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithWhitespaceJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = ""    "")]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithNullJustification()
        {
            var testCode = @"public class Foo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = null)]
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestSuppressionWithComplexJustification()
        {
            var testCode = @"public class Foo
{
    const string JUSTIFICATION = ""Foo"";
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = """" + JUSTIFICATION)]
    public void Bar()
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
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 66);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDiagnosticDoesNotThrowNullReferenceForWrongConstantType()
        {
            var testCode = @"public class Foo
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, null, Justification = 5)]
    public void Bar()
    {

    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 66),
                new DiagnosticResult
                {
                    Id = "CS0029",
                    Message = "Cannot implicitly convert type 'int' to 'string'",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 4, 82) }
                }
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1404CodeAnalysisSuppressionMustHaveJustification();
        }
    }
}