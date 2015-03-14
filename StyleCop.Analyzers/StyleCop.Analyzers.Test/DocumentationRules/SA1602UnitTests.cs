namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1602EnumerationItemsMustBeDocumented"/>-
    /// </summary>
    public class SA1602UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1602EnumerationItemsMustBeDocumented.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEnumWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{{
    /// <summary>
    /// Some Documentation
    /// </summary>
    Bar
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEnumWithoutDocumentation()
        {
            var testCode = @"
enum TypeName
{{
    Bar
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEnumWithEmptyDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{{
    /// <summary>
    /// 
    /// </summary>
    Bar
}}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(10, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1602EnumerationItemsMustBeDocumented();
        }
    }
}
