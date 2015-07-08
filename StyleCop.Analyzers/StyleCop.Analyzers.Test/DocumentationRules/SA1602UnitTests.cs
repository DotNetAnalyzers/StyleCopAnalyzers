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
        private const string DiagnosticId = SA1602EnumerationItemsMustBeDocumented.DiagnosticId;
        private const string NoDiagnostic = null;

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumFieldWithDocumentationAsync()
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
        [InlineData("public")]
        [InlineData("internal")]
        public async Task TestEnumFieldWithoutDocumentationAsync(string enumModifier)
        {
            var testCode = @"
{0} enum TypeName
{{
    Bar
}}";

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public", NoDiagnostic)]
        [InlineData("protected", NoDiagnostic)]
        [InlineData("protected internal", NoDiagnostic)]
        [InlineData("internal", NoDiagnostic)]
        [InlineData("private", DiagnosticId)]
        public async Task TestNestedEnumFieldWithoutDocumentationAsync(string enumModifier, string expectedDiagnosticId)
        {
            var testCode = @"
public class OuterClass
{{
{0} enum TypeName
{{
    Bar
}}
}}";

            if (expectedDiagnosticId == NoDiagnostic)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(6, 5);

                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), expected, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task TestEnumFieldWithEmptyDocumentationAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1602EnumerationItemsMustBeDocumented();
        }
    }
}
