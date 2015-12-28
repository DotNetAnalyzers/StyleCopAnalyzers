// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains the unit tests for SA1623.
    /// </summary>
    public class SA1623UnitTests : CodeFixVerifier
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

            var expected = this.CSharpDiagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(7, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            var expected = this.CSharpDiagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(9, 11 + accessibility.Length + type.Length).WithArguments(expectedArgument);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that empty summary tag does not throw an exception.
        /// Regression test for #1943
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
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

            var expected = this.CSharpDiagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(4, 16).WithArguments("Gets");
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var offeredFixes = await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false);
            Assert.Empty(offeredFixes);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new PropertySummaryDocumentationCodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new PropertySummaryDocumentationAnalyzer();
        }
    }
}
