namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.DocumentationRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1608ElementDocumentationMustNotHaveDefaultSummary"/>-
    /// </summary>
    [TestClass]
    public class SA1608UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1608ElementDocumentationMustNotHaveDefaultSummary.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithSummaryDocumentation(string typeName)
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

        private async Task TestTypeWithContentDocumentation(string typeName)
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

        private async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithoutSummaryDocumentation(string typeName)
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

        private async Task TestTypeWithoutContentDocumentation(string typeName)
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

        [TestMethod]
        public async Task TestClassWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithoutSummaryDocumentation()
        {
            await this.TestTypeWithoutSummaryDocumentation("class");
        }

        [TestMethod]
        public async Task TestClassWithoutContentDocumentation()
        {
            await this.TestTypeWithoutContentDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithoutSummaryDocumentation()
        {
            await this.TestTypeWithoutSummaryDocumentation("struct");
        }

        [TestMethod]
        public async Task TestStructWithoutContentDocumentation()
        {
            await this.TestTypeWithoutContentDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithoutSummaryDocumentation()
        {
            await this.TestTypeWithoutSummaryDocumentation("interface");
        }

        [TestMethod]
        public async Task TestInterfaceWithoutContentDocumentation()
        {
            await this.TestTypeWithoutContentDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassNoDocumentation()
        {
            await this.TestTypeNoDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructNoDocumentation()
        {
            await this.TestTypeNoDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceNoDocumentation()
        {
            await this.TestTypeNoDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithDefaultDocumentation()
        {
            var testCode = @"
/// <summary>
/// Summary description for the ClassName class.
/// </summary>
public class ClassName
{
}";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Element documentation must not have default summary",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 5)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Element documentation must not have default summary",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 5)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1608ElementDocumentationMustNotHaveDefaultSummary();
        }
    }
}
