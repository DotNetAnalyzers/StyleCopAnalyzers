namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1608ElementDocumentationMustNotHaveDefaultSummary"/>-
    /// </summary>
    public class SA1608UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1608ElementDocumentationMustNotHaveDefaultSummary.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithSummaryDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// Foo
/// </summary>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithContentDocumentation(string typeName)
        {
            var testCode = @"
/// <content>
/// Foo
/// </content>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutSummaryDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
{0}
TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestTypeWithoutContentDocumentation(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
partial {0}
TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassWithDefaultDocumentation()
        {
            var testCode = @"
/// <summary>
/// Summary description for the ClassName class.
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassWithDefaultDocumentationMultipleWhitespaces()
        {
            var testCode = @"
/// <summary>
/// Summary           description 
/// for the      ClassName class.
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1608ElementDocumentationMustNotHaveDefaultSummary();
        }
    }
}
