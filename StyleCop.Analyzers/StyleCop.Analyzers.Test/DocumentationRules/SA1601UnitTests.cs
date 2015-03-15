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

        [Fact]
        public async Task TestPartialTypeWithDocumentation()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
public partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "class"), EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "struct"), EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "interface"), EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPartialTypeWithoutDocumentation()
        {
            var testCode = @"
public partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "class"), expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "struct"), expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "interface"), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestPartialClassWithEmptyDocumentation()
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

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "class"), expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "struct"), expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, "interface"), expected, CancellationToken.None);
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
