// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationAnalyzer,
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationCodeFixProvider>;

    public class SA1624CSharp7UnitTests : SA1624UnitTests
    {
        /// <summary>
        /// Verifies that documentation that starts with the proper text for multiple expression-bodied accessors will
        /// produce a diagnostic when one of the accessors has reduced visibility.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="summaryPrefix">The prefix to use in the summary text.</param>
        /// <param name="expectedArgument1">The first expected argument for the diagnostic.</param>
        /// <param name="expectedArgument2">The second expected argument for the diagnostic.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory(DisplayName = "Expression-bodied Property Accessor Findings")]
        [InlineData("public", "int", "get => 0; internal set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("public", "int", "get => 0; private set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("public", "int", "internal get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        [InlineData("public", "int", "private get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        [InlineData("public", "bool", "get => false; private set => this.field = value;", "Gets or sets a value indicating whether", "get", "Gets a value indicating whether")]
        [InlineData("public", "bool", "private get => false; set => this.field = value;", "Gets or sets a value indicating whether", "set", "Sets a value indicating whether")]
        [InlineData("protected", "int", "get => 0; private set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("protected", "int", "private get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        [InlineData("protected internal", "int", "get => 0; internal set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("protected internal", "int", "get => 0; private set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("protected internal", "int", "internal get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        [InlineData("protected internal", "int", "private get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        [InlineData("internal", "int", "get => 0; private set => this.field = value;", "Gets or sets", "get", "Gets")]
        [InlineData("internal", "int", "private get => 0; set => this.field = value;", "Gets or sets", "set", "Sets")]
        public async Task VerifyThatInvalidDocumentationWillReportDiagnosticWithExpressionBodiedAccessorsAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument1, string expectedArgument2)
        {
            var testCode = $@"
public class TestClass
{{
    private object field;

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
    private object field;

    /// <summary>
    /// {expectedArgument2} the test property.
    /// </summary>
    {accessibility} {type} TestProperty
    {{
        {accessors}
    }}
}}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor).WithLocation(9, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument1, expectedArgument2);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
