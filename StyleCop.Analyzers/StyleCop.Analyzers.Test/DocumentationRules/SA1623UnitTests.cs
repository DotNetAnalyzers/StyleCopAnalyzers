// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationAnalyzer,
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationCodeFixProvider>;

    /// <summary>
    /// This class contains the unit tests for SA1623.
    /// </summary>
    public class SA1623UnitTests
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
        [InlineData("public", "int", "{ get; set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; protected set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; protected internal set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; internal set; }", "Gets")]
        [InlineData("public", "int", "{ get; private set; }", "Gets")]
        [InlineData("public", "int", "{ get; }", "Gets")]
        [InlineData("public", "int", "{ get; } = 0;", "Gets")]
        [InlineData("public", "int", "=> 0;", "Gets")]
        [InlineData("public", "int", "{ set { } }", "Sets")]
        [InlineData("public", "int", "{ internal get { return 0; } set { } }", "Sets")]
        [InlineData("public", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("public", "bool", "{ get; set; }", "Gets or sets a value indicating whether")]
        [InlineData("public", "bool", "{ get; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get; } = true;", "Gets a value indicating whether")]
        [InlineData("public", "bool", "=> true;", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get; private set; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ set { } }", "Sets a value indicating whether")]
        [InlineData("public", "bool", "{ private get { return false; } set { } }", "Sets a value indicating whether")]
        [InlineData("protected", "int", "{ get; private set; }", "Gets")]
        [InlineData("protected", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("protected internal", "int", "{ get; internal set; }", "Gets")]
        [InlineData("protected internal", "int", "{ get; private set; }", "Gets")]
        [InlineData("protected internal", "int", "{ internal get { return 0; } set { } }", "Sets")]
        [InlineData("protected internal", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("internal", "int", "{ get; private set; }", "Gets")]
        [InlineData("internal", "int", "{ private get { return 0; } set { } }", "Sets")]
        public async Task VerifyDocumentationWithWrongStartingTextWillProduceDiagnosticAsync(string accessibility, string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// The first test value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
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

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(7, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
        [InlineData("public", "int", "{ get; set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; protected set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; protected internal set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; internal set; }", "Gets or sets")]
        [InlineData("public", "int", "{ get; private set; }", "Gets")]
        [InlineData("public", "int", "{ get; }", "Gets")]
        [InlineData("public", "int", "{ get; } = 0;", "Gets")]
        [InlineData("public", "int", "=> 0;", "Gets")]
        [InlineData("public", "int", "{ set { } }", "Sets")]
        [InlineData("public", "int", "{ internal get { return 0; } set { } }", "Gets or sets")]
        [InlineData("public", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("public", "bool", "{ get; set; }", "Gets or sets a value indicating whether")]
        [InlineData("public", "bool", "{ get; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get; } = true;", "Gets a value indicating whether")]
        [InlineData("public", "bool", "=> true;", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get; private set; }", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ set { } }", "Sets a value indicating whether")]
        [InlineData("public", "bool", "{ private get { return false; } set { } }", "Sets a value indicating whether")]
        [InlineData("protected", "int", "{ get; private set; }", "Gets")]
        [InlineData("protected", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("protected internal", "int", "{ get; internal set; }", "Gets or sets")]
        [InlineData("protected internal", "int", "{ get; private set; }", "Gets")]
        [InlineData("protected internal", "int", "{ internal get { return 0; } set { } }", "Gets or sets")]
        [InlineData("protected internal", "int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("internal", "int", "{ get; private set; }", "Gets")]
        [InlineData("internal", "int", "{ private get { return 0; } set { } }", "Sets")]
        public async Task VerifyDocumentationOfPublicMethodInPrivateClassWillProduceDiagnosticAsync(string accessibility, string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    private class PrivateTestClass
    {{
        /// <summary>
        /// The first test value.
        /// </summary>
        {accessibility} {type} TestProperty {accessors}
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

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(9, 11 + accessibility.Length + type.Length).WithArguments(expectedArgument);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty summary tag does not throw an exception.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1943, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1943")]
        public async Task EmptySummaryTagShouldNotThrowAnExceptionAsync()
        {
            var testCode = @"public class ClassName
{
    /// <summary></summary>
    public int Property
    {
        get;
    }
}";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(4, 16).WithArguments("Gets");
            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
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
        [InlineData("public", "int", "{ get; set; }", "Gets", "Gets or sets")] // Regression test for #2098
        [InlineData("public", "int", "{ get; set; }", "Sets", "Gets or sets")] // Regression test for #2098
        [InlineData("public", "int", "{ get; }", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ get; }", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "=> 0;", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "bool", "=> false;", "Gets or sets a value indicating whether", "Gets a value indicating whether")] // Regression test for #2253
        [InlineData("protected", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("protected internal", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("internal", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ set {} }", "Gets", "Sets")] // Regression test for #2253
        [InlineData("public", "int", "{ set {} }", "Gets or sets", "Sets")] // Regression test for #2253
        [InlineData("public", "int", "{ get; private set; }", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ private get; set; }", "Gets", "Sets")] // Regression test for #2253
        public async Task IncorrectSummaryTagWithKnownPrefixShouldBeFixedCorrectlyAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summaryPrefix} the value.
    /// </summary>
    {accessibility} {type} TestProperty {accessors}
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
                Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(7, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
