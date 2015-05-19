namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1602EnumerationItemsMustBeDocumented"/>.
    /// </summary>
    public class SA1602UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
enum TypeName
{
    /// <summary>
    /// Some Documentation
    /// </summary>
    Bar
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public", SA1602EnumerationItemsMustBeDocumented.DiagnosticId)]
        [InlineData("internal", SA1602EnumerationItemsMustBeDocumented.DiagnosticIdInternal)]
        public async Task TestEnumWithoutDocumentationAsync(string enumModifier, string expectedDiagnosticId)
        {
            var testCode = @"
{0} enum TypeName
{{
    Bar
}}";

            DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(4, 5);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public", SA1602EnumerationItemsMustBeDocumented.DiagnosticId)]
        [InlineData("protected internal", SA1602EnumerationItemsMustBeDocumented.DiagnosticId)]
        [InlineData("protected internal", SA1602EnumerationItemsMustBeDocumented.DiagnosticId)]
        [InlineData("internal", SA1602EnumerationItemsMustBeDocumented.DiagnosticIdInternal)]
        [InlineData("private", SA1602EnumerationItemsMustBeDocumented.DiagnosticIdPrivate)]
        public async Task TestNestedEnumWithoutDocumentationAsync(string enumModifier, string expectedDiagnosticId)
        {
            var testCode = @"
public class OuterClass
{{
{0} enum TypeName
{{
    Bar
}}
}}";

            DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumWithEmptyDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public enum TypeName
{
    /// <summary>
    /// 
    /// </summary>
    Bar
}";

            DiagnosticResult expected = this.CSharpDiagnostic(SA1602EnumerationItemsMustBeDocumented.DiagnosticId).WithLocation(10, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1602EnumerationItemsMustBeDocumented();
        }
    }
}
