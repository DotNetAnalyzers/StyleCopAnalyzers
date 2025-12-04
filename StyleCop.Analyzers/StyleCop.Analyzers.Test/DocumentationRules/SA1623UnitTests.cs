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

        [Fact]
        [WorkItem(1934, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1934")]
        public async Task SummaryInParagraphIsAllowedAsync()
        {
            var testCode = @"public class ClassName
{
    /// <summary><para>Gets the first test value.</para></summary>
    public int Property1
    {
        get;
    }

    /// <summary>
    /// <para>Gets the second test value.</para>
    /// </summary>
    public int Property2
    {
        get;
    }

    /// <summary>
    /// <para>
    /// Gets the third test value.
    /// </para>
    /// </summary>
    public int Property3
    {
        get;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1934, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1934")]
        public async Task SummaryInParagraphCanBeFixedAsync()
        {
            var testCode = @"public class ClassName
{
    /// <summary><para>Gets the first test value.</para></summary>
    public int Property1
    {
        set { }
    }

    /// <summary>
    /// <para>Gets the second test value.</para>
    /// </summary>
    public int Property2
    {
        set { }
    }

    /// <summary>
    /// <para>
    /// Gets the third test value.
    /// </para>
    /// </summary>
    public int Property3
    {
        set { }
    }
}";
            var fixedTestCode = @"public class ClassName
{
    /// <summary><para>Sets the first test value.</para></summary>
    public int Property1
    {
        set { }
    }

    /// <summary>
    /// <para>Sets the second test value.</para>
    /// </summary>
    public int Property2
    {
        set { }
    }

    /// <summary>
    /// <para>
    /// Sets the third test value.
    /// </para>
    /// </summary>
    public int Property3
    {
        set { }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(4, 16).WithArguments("Sets"),
                Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(12, 16).WithArguments("Sets"),
                Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(22, 16).WithArguments("Sets"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
        [InlineData("public", "int", "{ get; set; }", "Initializes", "Gets or sets")] // Regression test for #3966
        [InlineData("public", "int", "{ get; }", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ get; }", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ get; }", "Gets or initializes", "Gets")] // Regression test for #3966
        [InlineData("public", "int", "=> 0;", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "bool", "=> false;", "Gets or sets a value indicating whether", "Gets a value indicating whether")] // Regression test for #2253
        [InlineData("protected", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("protected internal", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("internal", "int", "=> 0;", "Gets or sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ set {} }", "Gets", "Sets")] // Regression test for #2253
        [InlineData("public", "int", "{ set {} }", "Gets or sets", "Sets")] // Regression test for #2253
        [InlineData("public", "int", "{ set {} }", "Initializes", "Sets")] // Regression test for #3966
        [InlineData("public", "int", "{ set {} }", "Gets or initializes", "Sets")] // Regression test for #3966
        [InlineData("public", "int", "{ get; private set; }", "Sets", "Gets")] // Regression test for #2253
        [InlineData("public", "int", "{ get; private set; }", "Initializes", "Gets")] // Regression test for #3966
        [InlineData("public", "int", "{ private get; set; }", "Gets", "Sets")] // Regression test for #2253
        [InlineData("public", "int", "{ private get; set; }", "Initializes", "Sets")] // Regression test for #3966
        [InlineData("public", "int", "{ get; }", "Returns", "Gets")]
        [InlineData("public", "int", "{ get; set; }", "Returns", "Gets or sets")]
        [InlineData("public", "bool", "{ get; }", "Returns a value indicating whether", "Gets a value indicating whether")]
        [InlineData("public", "bool", "{ get; set; }", "Returns a value indicating whether", "Gets or sets a value indicating whether")]
        public async Task IncorrectSummaryTagWithKnownPrefixShouldBeFixedCorrectlyAsync(string accessibility, string type, string accessors, string summaryPrefix, string expectedArgument)
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

        [Theory]
        [InlineData("<inheritdoc/>")]
        [InlineData("<inheritdoc/> XYZ")]
        [WorkItem(3465, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3465")]
        public async Task VerifyInheritdocInSummaryTagIsAllowedAsync(string summary)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// {summary}
    /// </summary>
    public int TestProperty {{ get; set; }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3465, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3465")]
        public async Task VerifyInheritdocAfterTextStillReportsWarningAsync()
        {
            var testCode = @"
public class TestClass
{
    /// <summary>
    /// XYZ <inheritdoc/>
    /// </summary>
    public int {|#0:TestProperty|} { get; set; }
}
";

            var expected = Diagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(0).WithArguments("Gets or sets");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
