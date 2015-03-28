namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1605PartialElementDocumentationMustHaveSummary"/>-
    /// </summary>
    public class SA1605UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1605PartialElementDocumentationMustHaveSummary.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeNoDocumentation(string typeName)
        {
            var testCode = @"
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithSummaryDocumentation(string typeName)
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithContentDocumentation(string typeName)
        {
            var testCode = @"
/// <content>
/// 
/// </content>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithInheritedDocumentation(string typeName)
        {
            var testCode = @"
/// <inheritdoc/>
partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), EmptyDiagnosticResults, CancellationToken.None);
        }

        private async Task TestTypeWithoutDocumentation(string typeName)
        {
            var testCode = @"
///
partial {0}
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeName), expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestClassWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceWithDocumentation()
        {
            await this.TestTypeWithSummaryDocumentation("interface");
        }

        [Fact]
        public async Task TestClassWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceWithContentDocumentation()
        {
            await this.TestTypeWithContentDocumentation("interface");
        }

        [Fact]
        public async Task TestClassWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceWithInheritedDocumentation()
        {
            await this.TestTypeWithInheritedDocumentation("interface");
        }

        [Fact]
        public async Task TestClassWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("class");
        }

        [Fact]
        public async Task TestStructWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceWithoutDocumentation()
        {
            await this.TestTypeWithoutDocumentation("interface");
        }

        [Fact]
        public async Task TestEnumNoDocumentation()
        {
            await this.TestTypeNoDocumentation("enum");
        }

        [Fact]
        public async Task TestClassNoDocumentation()
        {
            await this.TestTypeNoDocumentation("class");
        }

        [Fact]
        public async Task TestStructNoDocumentation()
        {
            await this.TestTypeNoDocumentation("struct");
        }

        [Fact]
        public async Task TestInterfaceNoDocumentation()
        {
            await this.TestTypeNoDocumentation("interface");
        }

        [Fact]
        public async Task TestMethodNoDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithSummaryDocumentation()
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
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithContentDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <content>
    ///
    /// </content>
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithInheritedDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    /// <inheritdoc/>
    partial void Test();
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithoutDocumentation()
        {
            var testCode = @"
/// <summary>
/// 
/// </summary>
public class ClassName
{
    ///
    partial void Test();
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1605PartialElementDocumentationMustHaveSummary();
        }
    }
}
