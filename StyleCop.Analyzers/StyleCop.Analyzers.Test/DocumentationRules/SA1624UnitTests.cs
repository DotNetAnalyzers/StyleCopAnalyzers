// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationAnalyzer,
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationCodeFixProvider>;

    /// <summary>
    /// This class contains the unit tests for SA1624.
    /// </summary>
    public class SA1624UnitTests
    {
        /// <summary>
        /// Verifies that documentation that starts with the proper text for multiple accessors will produce a diagnostic when one of the accessors has reduced visibility.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="summaryPrefix">The prefix to use in the summary text.</param>
        /// <param name="expectedArgument1">The first expected argument for the diagnostic.</param>
        /// <param name="expectedArgument2">The second expected argument for the diagnostic.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "Property Findings")]
        [InlineData("public", "int", "get; internal set;", "Gets or sets", "get", "Gets")]
        [InlineData("public", "int", "get; private set;", "Gets or sets", "get", "Gets")]
        [InlineData("public", "int", "internal get; set;", "Gets or sets", "set", "Sets")]
        [InlineData("public", "int", "private get; set;", "Gets or sets", "set", "Sets")]
        [InlineData("public", "bool", "get; private set;", "Gets or sets a value indicating whether", "get", "Gets a value indicating whether")]
        [InlineData("public", "bool", "private get; set;", "Gets or sets a value indicating whether", "set", "Sets a value indicating whether")]
        [InlineData("protected", "int", "get; private set;", "Gets or sets", "get", "Gets")]
        [InlineData("protected", "int", "private get; set;", "Gets or sets", "set", "Sets")]
        [InlineData("protected internal", "int", "get; internal set;", "Gets or sets", "get", "Gets")]
        [InlineData("protected internal", "int", "get; private set;", "Gets or sets", "get", "Gets")]
        [InlineData("protected internal", "int", "internal get; set;", "Gets or sets", "set", "Sets")]
        [InlineData("protected internal", "int", "private get; set;", "Gets or sets", "set", "Sets")]
        [InlineData("internal", "int", "get; private set;", "Gets or sets", "get", "Gets")]
        [InlineData("internal", "int", "private get; set;", "Gets or sets", "set", "Sets")]
        public async Task VerifyThatInvalidDocumentationWillReportDiagnosticAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument1, string expectedArgument2)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summaryPrefix} the test property.
    /// </summary>
    {accessibility} {type} TestProperty
    {{
        {accessors}
    }}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    /// <summary>
    /// {expectedArgument2} the test property.
    /// </summary>
    {accessibility} {type} TestProperty
    {{
        {accessors}
    }}
}}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor).WithLocation(7, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument1, expectedArgument2);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that documentation that starts with the proper text for a lone getter will not produce this
        /// diagnostic when the property has an expression body.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="summaryPrefix">The prefix to use in the summary text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "ExpressionBody Gets")]
        [InlineData("public", "int", "Gets")]
        [InlineData("public", "bool", "Gets a value indicating whether")]
        [InlineData("protected", "int", "Gets")]
        [InlineData("protected internal", "int", "Gets")]
        [InlineData("internal", "int", "Gets")]
        public async Task VerifyThatValidDocumentationOnExpressionBodyIsAcceptedAsync(string accessibility, string type, string summaryPrefix)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summaryPrefix} the test property.
    /// </summary>
    {accessibility} {type} TestProperty =>
        default({type});
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that no diagnostic will be reported when a public and a private accessor are present within a property that is defined in a contained class of a private class.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "PrivateContainer Findings")]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task VerifyPrivateAccessorInPrivateContainerAsync(string typeKeyword)
        {
            var testCode = $@"
public class ContainerTestClass
{{
    private {typeKeyword} ContainerTestType
    {{
        public {typeKeyword} TestType
        {{
            /// <summary>
            /// Gets the test property.
            /// </summary>
            public int TestProperty
            {{
                get; private set;
            }}
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty tag summary is ignored (should be handled by SA1606).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2230, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2230")]
        public async Task VerifyEmptySummaryTagIsIgnoredAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary/>
    public int TestProperty { get; set; }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
