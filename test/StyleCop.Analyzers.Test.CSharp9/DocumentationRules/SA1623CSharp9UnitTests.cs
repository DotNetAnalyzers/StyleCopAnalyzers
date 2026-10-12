// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationAnalyzer,
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationCodeFixProvider>;

    public partial class SA1623CSharp9UnitTests : SA1623CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that property documentation that does not start with the appropriate text will result in a diagnostic.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="expectedArgument">The expected argument for the diagnostic message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("public", "int", "{ get; init; }", "Gets or initializes")]
        [InlineData("public", "int", "{ get; private init; }", "Gets")]
        [InlineData("public", "int", "{ init { } }", "Initializes")]
        [InlineData("public", "int", "{ private get { return 0; } init { } }", "Initializes")]
        [InlineData("public", "bool", "{ get; init; }", "Gets or initializes a value indicating whether")]
        [InlineData("public", "bool", "{ get; private init; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ init { } }", "Initializes a value indicating whether")]
        [InlineData("public", "bool", "{ private get { return false; } init { } }", "Initializes a value indicating whether")]
        public async Task VerifyDocumentationWithWrongStartingTextWithInitAccessorWillProduceDiagnosticAsync(string accessibility, string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// The first test value.
    /// </summary>
    {accessibility} {type} {{|#0:TestProperty|}} {accessors}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    /// <summary>
    /// {expectedArgument} the first test value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
}}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(0).WithArguments(expectedArgument);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);

            if (fixedTestCode.Contains("Gets or initializes"))
            {
                // These are allowed to be written as just 'Gets'
                await VerifyCSharpDiagnosticAsync(fixedTestCode.Replace("Gets or initializes", "Gets"), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that property documentation that does not start with the appropriate text will result in a
        /// diagnostic. This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1844.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="expectedArgument">The expected argument for the diagnostic message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("public", "int", "{ get; init; }", "Gets or initializes")]
        [InlineData("public", "int", "{ get; private init; }", "Gets")]
        [InlineData("public", "int", "{ init { } }", "Initializes")]
        [InlineData("public", "int", "{ private get { return 0; } init { } }", "Initializes")]
        [InlineData("public", "bool", "{ get; init; }", "Gets or initializes a value indicating whether")]
        [InlineData("public", "bool", "{ get; private init; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ init { } }", "Initializes a value indicating whether")]
        [InlineData("public", "bool", "{ private get { return false; } init { } }", "Initializes a value indicating whether")]
        public async Task VerifyDocumentationOfPublicMethodInPrivateClassWithInitAccessorWillProduceDiagnosticAsync(string accessibility, string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    private class PrivateTestClass
    {{
        /// <summary>
        /// The first test value.
        /// </summary>
        {accessibility} {type} {{|#0:TestProperty|}} {accessors}
    }}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    private class PrivateTestClass
    {{
        /// <summary>
        /// {expectedArgument} the first test value.
        /// </summary>
        {accessibility} {type} TestProperty {accessors}
    }}
}}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(0).WithArguments(expectedArgument);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);

            if (fixedTestCode.Contains("Gets or initializes"))
            {
                // These are allowed to be written as just 'Gets'
                await VerifyCSharpDiagnosticAsync(fixedTestCode.Replace("Gets or initializes", "Gets"), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Verifies that an incorrect summary tag with a known prefix will be fixed correctly.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="summaryPrefix">The summary prefix used in the test code.</param>
        /// <param name="expectedArgument">The expected argument for the diagnostic message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("public", "int", "{ get; init; }", "Sets", "Gets or initializes")]
        [InlineData("public", "int", "{ get; init; }", "Initializes", "Gets or initializes")]
        [InlineData("public", "int", "{ init {} }", "Gets", "Initializes")]
        [InlineData("public", "int", "{ init {} }", "Gets or sets", "Initializes")]
        [InlineData("public", "int", "{ init {} }", "Sets", "Initializes")]
        [InlineData("public", "int", "{ init {} }", "Gets or initializes", "Initializes")]
        [InlineData("public", "int", "{ get; private init; }", "Sets", "Gets")]
        [InlineData("public", "int", "{ get; private init; }", "Initializes", "Gets")]
        [InlineData("public", "int", "{ private get; init; }", "Gets", "Initializes")]
        [InlineData("public", "int", "{ private get; init; }", "Sets", "Initializes")]
        [InlineData("public", "int", "{ get; init; }", "Returns", "Gets or initializes")]
        [InlineData("public", "bool", "{ get; init; }", "Returns a value indicating whether", "Gets or initializes a value indicating whether")]
        public async Task IncorrectSummaryTagWithKnownPrefixAndInitAccessorShouldBeFixedCorrectlyAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summaryPrefix} the value.
    /// </summary>
    {accessibility} {type} {{|#0:TestProperty|}} {accessors}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    /// <summary>
    /// {expectedArgument} the value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(0).WithArguments(expectedArgument),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);

            if (fixedTestCode.Contains("Gets or initializes"))
            {
                // These are allowed to be written as just 'Gets'
                await VerifyCSharpDiagnosticAsync(fixedTestCode.Replace("Gets or initializes", "Gets"), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            }
        }

        [Fact]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        public async Task TestGetInitPropertyRequiresGetsOrInitializesSummaryAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// Sets the value.
    /// </summary>
    public string {|#0:Name|} { get; init; }
}
";
            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Gets or initializes the value.
    /// </summary>
    public string Name { get; init; }
}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor)
                .WithLocation(0)
                .WithArguments("Gets or initializes");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
