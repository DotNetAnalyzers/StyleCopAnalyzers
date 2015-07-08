namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA16X0InternalElementsMustBeDocumented"/>.
    /// </summary>
    public class SA16X0UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA16X0InternalElementsMustBeDocumented.DiagnosticId;
        private const string NoDiagnostic = null;

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestClassWithEmptyDocumentationAsync()
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteLine(@"/// <summary>");
            gen.WriteLine(@"/// </summary>");
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.Diagnostic, "internal");
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, DiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestClassAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestEnumAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteEnumStart("TestEnum", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, new[] { "private" }, NoDiagnostic)]
        public async Task TestNestedClassAsync(DocumentationOptions options, string[] parentModifiers, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("ParentClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, parentModifiers);
            gen.WriteClassStart("TestClass", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, NoDiagnostic)]
        public async Task TestExplicitInterfacePropertyImplementationAsync(DocumentationOptions options, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder(new[] { DiagnosticId });
            gen.WriteClassStart("SomeClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, new[] { "public" }, new[] { "SomeClass.IInterface" });
            gen.WriteExplicitInterfaceProperty("SomeProperty", "IInterface", options, ExpectedResult.Diagnostic);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestStructAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteStructStart("TestStruct", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestEnumWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteEnumStart("TestEnum", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestInterfaceAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteInterfaceStart("TestInterface", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestConstructorAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteConstructor("TestClass", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, new[] { "public" }, DiagnosticId)]
        public async Task TestDelegateAsync(DocumentationOptions options, string[] ownerModifiers, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, ownerModifiers);
            gen.WriteDelegate("SomeDelegate", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "public", "public", new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "internal", "public", new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "public", "private", new[] { "public" }, NoDiagnostic)]
        public async Task TestNestedClassDelegateAsync(DocumentationOptions options, string outerClassModifier, string innerClassModifier, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("OuterClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, new[] { outerClassModifier });
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, new[] { innerClassModifier });
            gen.WriteDelegate("SomeDelegate", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestEventAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteEvent("SomeEvent", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestFieldWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteField("someField", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, DiagnosticId)]
        public async Task TestDestructorWithoutDocumentationAsync(DocumentationOptions options, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "internal");
            gen.WriteDestructor("TestClass", options, ExpectedResult.Diagnostic);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestPublicIndexerWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteIndexer(options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestPublicMethodWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteMethod("SomeMethod", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticId)]
        public async Task TestPublicPropertyWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteProperty("SomeProperty", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        protected Task VerifyCSharpDiagnosticAsync(DocumentationRuleTestSampleCodeBuilder gen, string diagnosticId, CancellationToken cancellationToken)
        {
            var code = gen.GeneratedText;

            var diagnostics = string.IsNullOrEmpty(diagnosticId)
                ? EmptyDiagnosticResults
                : gen.DiagnosticLocations.Select(x => this.CSharpDiagnostic(diagnosticId).WithLocation(x.Line, x.Column)).ToArray();

            return this.VerifyCSharpDiagnosticAsync(code, diagnostics, cancellationToken);
        }

        [Fact]
        public async Task TestEmptyXmlCommentsAsync()
        {
            var testCodeWithEmptyDocumentation = @"    /// <summary>
    /// </summary>
internal class
OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
internal class
OuterClass
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic(NoDiagnostic).WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCDataXmlCommentsAsync()
        {
            var testCodeWithEmptyDocumentation = @"/// <summary>
    /// <![CDATA[]]>
    /// </summary>
internal class
OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// <![CDATA[A summary.]]>
    /// </summary>
internal class
OuterClass
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementXmlCommentsAsync()
        {
            // A self-closing XML tag is permitted as an XML comment.
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteLine(@"/// <inheritdoc />");
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "internal");
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, DiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiLineDocumentationAsync()
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteLine(@"/**
 * <summary>This is a documentation comment summary.</summary>
 */");
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "internal");
            gen.WriteLine(@"/**");
            gen.WriteLine(@" * <summary>This is a documentation comment summary.</summary>");
            gen.WriteLine(@" */");
            gen.WriteMethod("SomeMethod", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, DiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we recognize the auto-generated xml element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSkipAutoGeneratedCodeAsync()
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteAutoGeneratedHeader();
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "internal");
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, DiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we recognize the autogenerated xml element.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSkipAutoGeneratedCode2Async()
        {
            var testCode = @"//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

internal class OuterClass
{
    public void SomeMethod() { }
}";

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
internal partial {0} TypeName
{{
}}";
            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class", "public", NoDiagnostic)]
        [InlineData("class", "internal", DiagnosticId)]
        [InlineData("struct", "public", NoDiagnostic)]
        [InlineData("struct", "internal", DiagnosticId)]
        [InlineData("interface", "public", NoDiagnostic)]
        [InlineData("interface", "internal", DiagnosticId)]
        public async Task TestPartialTypeWithoutDocumentationAsync(string typeKeyword, string typeModifier, string expectedDiagnosticId)
        {
            var testCode = @"
{0} partial {1}
TypeName
{{
}}";

            if (expectedDiagnosticId == NoDiagnostic)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeModifier, typeKeyword), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(3, 1);
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeModifier, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
            }
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
internal partial {0} 
TypeName
{{
}}";

            DiagnosticResult expected = this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, typeKeyword), expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPartialMethodWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
internal partial class TypeName
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
        internal partial class TypeName
        {
            partial void MemberName();
        }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnumFieldWithDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Some Documentation
/// </summary>
internal enum TypeName
{
    /// <summary>
    /// Some Documentation
    /// </summary>
    Bar
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public", NoDiagnostic)]
        [InlineData("internal", DiagnosticId)]
        public async Task TestEnumFieldWithoutDocumentationAsync(string enumModifier, string expectedDiagnosticId)
        {
            var testCode = @"/// <summary>
/// Some text.
/// </summary>
{0} enum TypeName
{{
    Bar
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

        [Theory]
        [InlineData("public", NoDiagnostic)]
        [InlineData("protected", NoDiagnostic)]
        [InlineData("protected internal", NoDiagnostic)]
        [InlineData("internal", DiagnosticId)]
        [InlineData("private", NoDiagnostic)]
        public async Task TestNestedEnumFieldWithoutDocumentationAsync(string enumModifier, string expectedDiagnosticId)
        {
            var testCode = @"/// <summary>
/// Some text.
/// </summary>
public class OuterClass
{{
{0} enum
TypeName
{{
    /// <summary>
    /// The comments.
    /// </summary>
    Bar
}}
}}";

            if (expectedDiagnosticId == NoDiagnostic)
            {
                await this.VerifyCSharpDiagnosticAsync(string.Format(testCode, enumModifier), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                DiagnosticResult expected = this.CSharpDiagnostic(expectedDiagnosticId).WithLocation(7, 1);

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
internal enum TypeName
{
    /// <summary>
    /// 
    /// </summary>
    Bar
}";

            DiagnosticResult expected = this.CSharpDiagnostic(DiagnosticId).WithLocation(10, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA16X0InternalElementsMustBeDocumented();
        }
    }
}
