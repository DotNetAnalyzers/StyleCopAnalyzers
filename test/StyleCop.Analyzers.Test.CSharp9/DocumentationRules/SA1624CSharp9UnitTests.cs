// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.CSharp8.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationAnalyzer,
        StyleCop.Analyzers.DocumentationRules.PropertySummaryDocumentationCodeFixProvider>;

    public partial class SA1624CSharp9UnitTests : SA1624CSharp8UnitTests
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
        [Theory(DisplayName = "Init accessor findings")]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("public", "bool", "get; private init;", "Gets or sets a value indicating whether", "get", "Gets a value indicating whether")]
        [InlineData("public", "bool", "private get; init;", "Gets or sets a value indicating whether", "init", "Initializes a value indicating whether")]
        [InlineData("internal", "int", "get; private init;", "Gets or sets", "get", "Gets")]
        [InlineData("internal", "int", "private get; init;", "Gets or sets", "init", "Initializes")]
        [InlineData("public", "bool", "get; private init;", "Gets or initializes a value indicating whether", "get", "Gets a value indicating whether")]
        [InlineData("public", "bool", "private get; init;", "Gets or initializes a value indicating whether", "init", "Initializes a value indicating whether")]
        [InlineData("internal", "int", "get; private init;", "Gets or initializes", "get", "Gets")]
        [InlineData("internal", "int", "private get; init;", "Gets or initializes", "init", "Initializes")]
        public async Task VerifyThatInvalidDocumentationWithInitAccessorWillReportDiagnosticAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument1, string expectedArgument2)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summaryPrefix} the test property.
    /// </summary>
    {accessibility} {type} {{|#0:TestProperty|}}
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

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor).WithLocation(0).WithArguments(expectedArgument1, expectedArgument2);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("sets")]
        [InlineData("initializes")]
        public async Task TestGetInitWithRestrictedInitAsync(string setterDescription)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Gets or {setterDescription} the value.
    /// </summary>
    public string {{|#0:Name|}} {{ get; internal init; }}
}}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Name { get; internal init; }
}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor)
                .WithLocation(0)
                .WithArguments("get", "Gets");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(3966, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3966")]
        [InlineData("sets")]
        [InlineData("initializes")]
        public async Task TestGetBooleanInitWithRestrictedInitAsync(string setterDescription)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// Gets or {setterDescription} a value indicating whether the value.
    /// </summary>
    public bool {{|#0:Name|}} {{ get; internal init; }}
}}
";

            var fixedCode = @"
public class TestClass
{
    /// <summary>
    /// Gets a value indicating whether the value.
    /// </summary>
    public bool Name { get; internal init; }
}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor)
                .WithLocation(0)
                .WithArguments("get", "Gets a value indicating whether");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
