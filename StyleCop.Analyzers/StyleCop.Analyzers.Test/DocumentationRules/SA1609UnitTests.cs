namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Analyzers.DocumentationRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1609PropertyDocumentationMustHaveValue"/>.
    /// </summary>
    [TestClass]
    public class SA1609UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1609PropertyDocumentationMustHaveValue.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithDocumentation()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithInheritedDocumentation()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    public ClassName Property { get; set; }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPropertyWithoutDocumentation()
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Property documentation must have value",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 22)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1609PropertyDocumentationMustHaveValue();
        }
    }
}
