namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1601PartialElementsMustBeDocumented"/>-
    /// </summary>
    public class SA1601UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1601PartialElementsMustBeDocumented.DiagnosticId;

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
        public async Task TestPartialTypeWithDocumentation(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialTypeWithoutDocumentation(string typeKeyword)
        {
            var testCode = @"
public partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None);
        }

        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [InlineData("interface")]
        public async Task TestPartialClassWithEmptyDocumentation(string typeKeyword)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public partial {0} 
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPartialMethodWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{{
    /// <summary>
    /// Some Documentation
    /// </summary>
    partial void MemberName();
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPartialMethodWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{{
    partial void MemberName();
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPartialMethodWithEmptyDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial class TypeName
{{
    /// <summary>
    /// 
    /// </summary>
    partial void MemberName();
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1601PartialElementsMustBeDocumented();
        }
    }
}
