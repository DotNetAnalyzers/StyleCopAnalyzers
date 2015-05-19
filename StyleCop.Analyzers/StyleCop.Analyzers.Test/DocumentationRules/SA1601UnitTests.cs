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
    /// This class contains unit tests for <see cref="SA1601PartialElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1601UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialTypeWithDocumentationAsync(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", "public", SA1601PartialElementsMustBeDocumented.DiagnosticId)]
        [InlineData("class", "internal", SA1601PartialElementsMustBeDocumented.DiagnosticIdInternal)]
        [InlineData("struct", "public", SA1601PartialElementsMustBeDocumented.DiagnosticId)]
        [InlineData("struct", "public", SA1601PartialElementsMustBeDocumented.DiagnosticId)]
        [InlineData("interface", "public", SA1601PartialElementsMustBeDocumented.DiagnosticId)]
        public async Task TestPartialTypeWithoutDocumentationAsync(string typeKeyword, string typeModifier, string expectedDiagnosticId)
        {
            var testCode = @"
{0} partial {1}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeModifier, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialClassWithEmptyDocumentationAsync(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial {0} 
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic(SA1601PartialElementsMustBeDocumented.DiagnosticId).WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    /// <summary>
    /// Some Documentation
    /// </summary>
    partial void MemberName();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    partial void MemberName();
}";

            DiagnosticResult expected = this.CSharpDiagnostic(SA1601PartialElementsMustBeDocumented.DiagnosticIdPrivate).WithLocation(7, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithEmptyDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{
    /// <summary>
    /// 
    /// </summary>
    partial void MemberName();
}";

            DiagnosticResult expected = this.CSharpDiagnostic(SA1601PartialElementsMustBeDocumented.DiagnosticIdPrivate).WithLocation(10, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1601PartialElementsMustBeDocumented();
        }
    }
}
