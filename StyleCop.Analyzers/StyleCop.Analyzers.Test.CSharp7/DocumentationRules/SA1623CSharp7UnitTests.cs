// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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

    public class SA1623CSharp7UnitTests : SA1623UnitTests
    {
        /// <summary>
        /// Verifies that property documentation that does not start with the appropriate text will result in a
        /// diagnostic. This test extends
        /// <see cref="SA1623UnitTests.VerifyDocumentationWithWrongStartingTextWillProduceDiagnosticAsync"/> for the
        /// purpose of testing expression-bodied property accessors.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="expectedArgument">The expected argument for the diagnostic message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("public", "int", "{ get => 0; set => this.field = value; }", "Gets or sets")]
        [InlineData("public", "int", "{ get => 0; protected set => this.field = value; }", "Gets or sets")]
        [InlineData("public", "int", "{ get => 0; protected internal set => this.field = value; }", "Gets or sets")]
        [InlineData("public", "int", "{ get => 0; internal set => this.field = value; }", "Gets")]
        [InlineData("public", "int", "{ get => 0; private set => this.field = value; }", "Gets")]
        [InlineData("public", "int", "{ get => 0; }", "Gets")]
        [InlineData("public", "int", "{ set => this.field = value; }", "Sets")]
        [InlineData("public", "int", "{ internal get => 0; set => this.field = value; }", "Sets")]
        [InlineData("public", "int", "{ private get => 0; set => this.field = value; }", "Sets")]
        [InlineData("public", "bool", "{ get => true; set => this.field = value; }", "Gets or sets a value indicating whether")]
        [InlineData("public", "bool", "{ get => true; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get => true; private set => this.field = value; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ set => this.field = value; }", "Sets a value indicating whether")]
        [InlineData("public", "bool", "{ private get => false; set => this.field = value; }", "Sets a value indicating whether")]
        [InlineData("protected", "int", "{ get => 0; private set => this.field = value; }", "Gets")]
        [InlineData("protected", "int", "{ private get => 0; set => this.field = value; }", "Sets")]
        [InlineData("protected internal", "int", "{ get => 0; internal set => this.field = value; }", "Gets")]
        [InlineData("protected internal", "int", "{ get => 0; private set => this.field = value; }", "Gets")]
        [InlineData("protected internal", "int", "{ internal get => 0; set => this.field = value; }", "Sets")]
        [InlineData("protected internal", "int", "{ private get => 0; set => this.field = value; }", "Sets")]
        [InlineData("internal", "int", "{ get => 0; private set => this.field = value; }", "Gets")]
        [InlineData("internal", "int", "{ private get => 0; set => this.field = value; }", "Sets")]

        [WorkItem(2861, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2861")]
        [InlineData("public", "ref int", "{ get => throw null; }", "Gets")]
        public async Task VerifyDocumentationWithWrongStartingTextWillProduceDiagnosticWithExpressionBodiedAccessorsAsync(string accessibility, string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    private object field;

    /// <summary>
    /// The first test value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    private object field;

    /// <summary>
    /// {expectedArgument} the first test value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
}}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(9, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
