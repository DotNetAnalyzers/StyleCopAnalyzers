namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1609PropertyDocumentationMustHaveValue"/>.
    /// </summary>
    public class SA1609UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    ///
    /// </summary>
    /// <value>
    ///
    /// </value>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithInheritedDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyNoDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyWithoutDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <summary>
    /// 
    /// </summary>
    public ClassName Property { get; set; }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 22);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1609PropertyDocumentationMustHaveValue();
        }
    }
}
