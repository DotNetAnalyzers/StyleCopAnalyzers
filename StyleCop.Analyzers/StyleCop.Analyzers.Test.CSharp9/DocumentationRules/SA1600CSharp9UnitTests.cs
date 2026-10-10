// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp9UnitTests : SA1600CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that a partial method with an access modifier is not reported, since partial elements are reported
        /// by SA1601 instead.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPartialMethodWithAccessModifierAsync()
        {
            var testCode = @"/// <summary>
/// Summary.
/// </summary>
public partial class TestClass
{
    public partial int TestMethod(out int x);
}

public partial class TestClass
{
    public partial int TestMethod(out int x)
    {
        x = 0;
        return 0;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an undocumented part of a partial method is not reported when the other part is documented.
        /// SA1601 does not report it either, since one documented part is enough for a partial member.
        /// </summary>
        /// <param name="documentDefinition"><see langword="true"/> to document the defining declaration;
        /// <see langword="false"/> to document the implementing declaration.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TestPartialMethodWithAccessModifierOnePartDocumentedAsync(bool documentDefinition)
        {
            var documentation = @"/// <summary>
    /// Summary.
    /// </summary>
    ";
            var testCode = $@"/// <summary>
/// Summary.
/// </summary>
public partial class TestClass
{{
    {(documentDefinition ? documentation : string.Empty)}public partial int TestMethod(out int x);
}}

public partial class TestClass
{{
    {(documentDefinition ? string.Empty : documentation)}public partial int TestMethod(out int x)
    {{
        x = 0;
        return 0;
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(CommonMemberData.TypeKeywordsWhichSupportPrimaryConstructors), MemberType = typeof(CommonMemberData))]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestTypeWithPrimaryConstructorWithoutDocumentationAsync(string typeKeyword)
        {
            var testCode = $@"public {typeKeyword} {{|#0:TestType|}}(int {{|#1:X|}});";

            var expected = this.GetExpectedResultTestTypeWithPrimaryConstructorWithoutDocumentation();
            if (typeKeyword.StartsWith("record", System.StringComparison.Ordinal))
            {
                var test = new CSharpTest { TestCode = testCode, FixedCode = testCode, DisabledDiagnostics = { "CS1591" } };
                test.ExpectedDiagnostics.AddRange(expected);
                test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(1));
                await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
                return;
            }

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that record properties declared by positional parameters require matching documentation.
        /// </summary>
        /// <param name="documentation">The documentation following the summary.</param>
        /// <param name="documented">Whether the parameter is documented.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("", false)]
        [InlineData("/// <param name=\"Parameter\">Parameter.</param>", true)]
        [InlineData("/// <param name=\"Parameter\"/>", true)]
        [InlineData("/// <param name=\"parameter\">Parameter.</param>", false)]
        [InlineData("/// <param>Parameter.</param>", false)]
        [InlineData("/// <paramref name=\"Parameter\"/>", false)]
        [InlineData("/// <inheritdoc/>", true)]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordParameterDocumentationAsync(string documentation, bool documented)
        {
            var testCode = $@"/// <summary>
/// Record.
/// </summary>
{documentation}
public record MyRecord(int {{|#0:Parameter|}});";
            var test = new CSharpTest { TestCode = testCode, DisabledDiagnostics = { "CS1591" } };
            if (!documented)
            {
                test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies parameter documentation for all supported record declaration forms.
        /// </summary>
        /// <param name="keyword">The record declaration keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(CommonMemberData.RecordTypeDeclarationKeywords), MemberType = typeof(CommonMemberData))]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordPartialParameterDocumentationAsync(string keyword)
        {
            var testCode = $@"namespace TestNamespace
{{
    /// <summary>Record.</summary>
    /// <param name=""Documented"">Documented.</param>
    public {keyword} MyRecord<T>(int {{|#0:Missing|}}, T Documented, int {{|#1:@Other|}} = 0);
}}";
            var test = new CSharpTest { TestCode = testCode, DisabledDiagnostics = { "CS1591" } };
            test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(1));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that included documentation is expanded before checking record parameters.
        /// </summary>
        /// <param name="documentation">The included documentation.</param>
        /// <param name="documented">Whether the parameter is documented.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("", false)]
        [InlineData("<param name='Parameter'>Parameter.</param>", true)]
        [InlineData("<param name='parameter'>Parameter.</param>", false)]
        [InlineData("<param/>", false)]
        [InlineData("<inheritdoc/>", true)]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordIncludedParameterDocumentationAsync(string documentation, bool documented)
        {
            var test = new CSharpTest
            {
                DisabledDiagnostics = { "CS1591" },
                TestCode = @"/// <include file='Record.xml' path='/doc/*'/>
public record MyRecord(int {|#0:Parameter|});",
                XmlReferences = { { "Record.xml", $"<doc><summary>Record.</summary>{documentation}</doc>" } },
            };
            if (!documented)
            {
                test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that parameter tags in source and included XML are combined.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordMixedParameterDocumentationAsync()
        {
            var test = new CSharpTest
            {
                DisabledDiagnostics = { "CS1591" },
                TestCode = @"/// <summary>Record.</summary>
/// <param name='First'>First.</param>
/// <include file='Record.xml' path='/doc/*'/>
public record MyRecord(int First, int Second, int {|#0:Missing|});",
                XmlReferences = { { "Record.xml", "<doc><param name='Second'>Second.</param></doc>" } },
            };
            test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that record parameters follow the effective accessibility and documentation settings.
        /// </summary>
        /// <param name="accessibility">The accessibility of the containing type.</param>
        /// <param name="setting">The documentation setting.</param>
        /// <param name="enabled">The setting value.</param>
        /// <param name="reported">Whether the parameter requires documentation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("public", "documentExposedElements", true, true)]
        [InlineData("public", "documentExposedElements", false, false)]
        [InlineData("internal", "documentInternalElements", true, true)]
        [InlineData("internal", "documentInternalElements", false, false)]
        [InlineData("internal", "documentPrivateElements", true, true)]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordParameterDocumentationSettingsAsync(string accessibility, string setting, bool enabled, bool reported)
        {
            var test = new CSharpTest
            {
                DisabledDiagnostics = { "CS1591" },
                TestCode = $@"/// <summary>Container.</summary>
{accessibility} class Container
{{
    /// <summary>Record.</summary>
    public record MyRecord(int {{|#0:Parameter|}});
}}",
                Settings = $@"{{""settings"":{{""documentationRules"":{{""{setting}"":{enabled.ToString().ToLowerInvariant()}}}}}}}",
            };
            if (reported)
            {
                test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            }

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that partial records still document properties declared by their positional parameters.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestPartialRecordParameterDocumentationAsync()
        {
            var test = new CSharpTest
            {
                DisabledDiagnostics = { "CS1591" },
                TestCode = @"/// <summary>Record.</summary>
public partial record MyRecord(int {|#0:Parameter|});
public partial record MyRecord;",
            };
            test.ExpectedDiagnostics.AddRange(this.GetExpectedRecordParameterDiagnostics(0));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that records without positional parameters need no parameter documentation.
        /// </summary>
        /// <param name="parameterList">The optional parameter list.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("")]
        [InlineData("()")]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordWithoutParametersAsync(string parameterList)
        {
            var testCode = $@"/// <summary>Record.</summary>
public record MyRecord{parameterList};";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that disabled documentation parsing also disables record parameter documentation checks.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestRecordParameterWithDocumentationModeNoneAsync()
        {
            var test = new CSharpTest
            {
                DisabledDiagnostics = { "CS1591" },
                TestCode = "public record MyRecord(int Parameter);",
                TestState = { DocumentationMode = DocumentationMode.None },
            };
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedRecordParameterDiagnostics(int location)
        {
            // Roslyn 3.8 and 4.0 invoke record syntax actions twice (dotnet/roslyn#53136).
            return new[] { Diagnostic().WithLocation(location), Diagnostic().WithLocation(location) };
        }

        protected override DiagnosticResult[] GetExpectedResultTestRegressionMethodGlobalNamespace(string code)
        {
            if (code == "public void {|#0:TestMember|}() { }")
            {
                return new[]
                {
                    // error CS8805: Program using top-level statements must be an executable.
                    DiagnosticResult.CompilerError("CS8805"),

                    // /0/Test0.cs(4,1): error CS0106: The modifier 'public' is not valid for this item
                    DiagnosticResult.CompilerError("CS0106").WithSpan(4, 1, 4, 7).WithArguments("public"),
                };
            }

            return base.GetExpectedResultTestRegressionMethodGlobalNamespace(code);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestTypeWithPrimaryConstructorWithoutDocumentation()
        {
            return new[]
            {
                // Diagnostic issued twice because of https://github.com/dotnet/roslyn/issues/53136
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(0),
            };
        }
    }
}
