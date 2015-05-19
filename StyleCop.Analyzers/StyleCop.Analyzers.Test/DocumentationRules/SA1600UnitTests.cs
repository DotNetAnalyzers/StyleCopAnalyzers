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
    /// This class contains unit tests for <see cref="SA1600ElementsMustBeDocumented"/>.
    /// </summary>
    public class SA1600UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1600ElementsMustBeDocumented.DiagnosticId;
        private const string DiagnosticIdInternal = SA1600ElementsMustBeDocumented.DiagnosticIdInternal;
        private const string DiagnosticIdPrivate = SA1600ElementsMustBeDocumented.DiagnosticIdPrivate;
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
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.Diagnostic, "public");
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, DiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
        public async Task TestClassAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
        public async Task TestEnumAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteEnumStart("TestEnum", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "internal" }, DiagnosticIdInternal)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, new[] { "public" }, DiagnosticIdInternal)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, DiagnosticId)]
        public async Task TestExplicitInterfacePropertyImplementationAsync(DocumentationOptions options, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder(new[] { DiagnosticId, DiagnosticIdInternal, DiagnosticIdPrivate });
            gen.WriteClassStart("SomeClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, new[] { "public" }, new[] { "SomeClass.IInterface" });
            gen.WriteExplicitInterfaceProperty("SomeProperty", "IInterface", options, ExpectedResult.Diagnostic);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
        public async Task TestStructAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteStructStart("TestStruct", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
        public async Task TestEnumWithoutDocumentationAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteEnumStart("TestEnum", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
        public async Task TestInterfaceAsync(DocumentationOptions options, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteInterfaceStart("TestInterface", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, new[] { "internal" }, DiagnosticIdInternal)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, new[] { "public" }, DiagnosticIdInternal)]
        public async Task TestDelegateAsync(DocumentationOptions options, string[] ownerModifiers, string[] modifiers, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, ownerModifiers);
            gen.WriteDelegate("SomeDelegate", options, ExpectedResult.Diagnostic, modifiers);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "public", "public", new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "internal", "public", new[] { "public" }, DiagnosticIdInternal)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, "public", "private", new[] { "public" }, DiagnosticIdPrivate)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
        public async Task TestFinalizerWithoutDocumentationAsync(DocumentationOptions options, string expectedDiagnosticId)
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteClassStart("TestClass", DocumentationOptions.WriteSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
            gen.WriteFinalizer("TestClass", options, ExpectedResult.Diagnostic);
            gen.WriteBlockEnd();

            await this.VerifyCSharpDiagnosticAsync(gen, expectedDiagnosticId, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(DocumentationOptions.WriteSampleDocumentation, new[] { "public" }, NoDiagnostic)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "private" }, DiagnosticIdPrivate)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "public" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "protected", "internal" }, DiagnosticId)]
        [InlineData(DocumentationOptions.OmitSampleDocumentation, new[] { "internal" }, DiagnosticIdInternal)]
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
public class OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// A summary
    /// </summary>
public class OuterClass
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic(DiagnosticId).WithLocation(3, 14);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCDataXmlCommentsAsync()
        {
            var testCodeWithEmptyDocumentation = @"/// <summary>
    /// <![CDATA[]]>
    /// </summary>
public class OuterClass
{
}";
            var testCodeWithDocumentation = @"    /// <summary>
    /// <![CDATA[A summary.]]>
    /// </summary>
public class OuterClass
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic(DiagnosticId).WithLocation(4, 14);

            await this.VerifyCSharpDiagnosticAsync(testCodeWithDocumentation, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(testCodeWithEmptyDocumentation, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyElementXmlCommentsAsync()
        {
            var gen = new DocumentationRuleTestSampleCodeBuilder();
            gen.WriteLine(@"/// <inheritdoc />");
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
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
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
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
            gen.WriteClassStart("TestClass", DocumentationOptions.OmitSampleDocumentation, ExpectedResult.NoDiagnostic, "public");
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

public class OuterClass
{
    public void SomeMethod() { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1600ElementsMustBeDocumented();
        }
    }
}
