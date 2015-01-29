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
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
{0} TypeName
{{
}}";
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
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
            await VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestClassWithDocumentation()
        {
            await TestTypeWithSummaryDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithDocumentation()
        {
            await TestTypeWithSummaryDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithDocumentation()
        {
            await TestTypeWithSummaryDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithContentDocumentation()
        {
            await TestTypeWithContentDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithContentDocumentation()
        {
            await TestTypeWithContentDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithContentDocumentation()
        {
            await TestTypeWithContentDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithInheritedDocumentation()
        {
            await TestTypeWithInheritedDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassWithoutSummaryDocumentation()
        {
            await TestTypeWithoutSummaryDocumentation("class");
        }

        [TestMethod]
        public async Task TestClassWithoutContentDocumentation()
        {
            await TestTypeWithoutContentDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructWithoutSummaryDocumentation()
        {
            await TestTypeWithoutSummaryDocumentation("struct");
        }

        [TestMethod]
        public async Task TestStructWithoutContentDocumentation()
        {
            await TestTypeWithoutContentDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceWithoutSummaryDocumentation()
        {
            await TestTypeWithoutSummaryDocumentation("interface");
        }

        [TestMethod]
        public async Task TestInterfaceWithoutContentDocumentation()
        {
            await TestTypeWithoutContentDocumentation("interface");
        }

        [TestMethod]
        public async Task TestClassNoDocumentation()
        {
            await TestTypeNoDocumentation("class");
        }

        [TestMethod]
        public async Task TestStructNoDocumentation()
        {
            await TestTypeNoDocumentation("struct");
        }

        [TestMethod]
        public async Task TestInterfaceNoDocumentation()
        {
            await TestTypeNoDocumentation("interface");
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
                        Id = DiagnosticId,
                        Message = "Element documentation must not have default summary",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 5)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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
                        Id = DiagnosticId,
                        Message = "Element documentation must not have default summary",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 5)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1608ElementDocumentationMustNotHaveDefaultSummary();
        }
    }
}
