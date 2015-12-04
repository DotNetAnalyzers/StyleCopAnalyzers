﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
    /// This class contains the unit tests for SA1624.
    /// </summary>
    public class SA1624UnitTests : CodeFixVerifier
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
        [Theory]
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

            var expected = this.CSharpDiagnostic(PropertySummaryDocumentationAnalyzer.SA1624Descriptor).WithLocation(7, 7 + accessibility.Length + type.Length).WithArguments(expectedArgument1, expectedArgument2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that documentation that starts with the proper text for multiple accessors will not produce this
        /// diagnostic when the property has an expression body.
        /// </summary>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="type">The type to use for the property.</param>
        /// <param name="summaryPrefix">The prefix to use in the summary text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("public", "int", "Gets or sets")]
        [InlineData("public", "bool", "Gets or sets a value indicating whether")]
        [InlineData("protected", "int", "Gets or sets")]
        [InlineData("protected internal", "int", "Gets or sets")]
        [InlineData("internal", "int", "Gets or sets")]
        public async Task VerifyThatInvalidDocumentationWillReportDiagnosticForExpressionBodyAsync(string accessibility, string type, string summaryPrefix)
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verify that no diagnostic will be reported when a public and a private accessor are present within a property that is defined in a contained class of a private class.
        /// </summary>
        /// <param name="typeKeyword">The type keyword to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            yield return PropertySummaryDocumentationAnalyzer.SA1623Descriptor.Id;
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
