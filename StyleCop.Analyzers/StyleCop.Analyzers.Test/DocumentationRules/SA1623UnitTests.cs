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
        /// <param name="type">The type to use for the property.</param>
        /// <param name="accessors">The accessors for the property.</param>
        /// <param name="expectedArgument">The expected argument for the diagnostic message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("int", "{ get; set; }", "Gets or sets")]
        [InlineData("int", "{ get; }", "Gets")]
        [InlineData("int", "{ get; private set; }", "Gets")]
        [InlineData("int", "{ set { } }", "Sets")]
        [InlineData("int", "{ private get { return 0; } set { } }", "Sets")]
        [InlineData("bool", "{ get; set; }", "Gets or sets a value indicating whether")]
        [InlineData("bool", "{ get; }", "Gets a value indicating whether")]
        [InlineData("bool", "{ get; private set; }", "Gets a value indicating whether")]
        [InlineData("bool", "{ set { } }", "Sets a value indicating whether")]
        [InlineData("bool", "{ private get { return false; } set { } }", "Sets a value indicating whether")]
        public async Task VerifyDocumentationWithWrongStartingTextWillProduceDiagnosticAsync(string type, string accessors, string expectedArgument)
        {
            var testCode = $@"
public class TestClass
{{
    /// <summary>
    /// The first test value.
    /// </summary>
    public {type} TestProperty {accessors}
}}
";

            var fixedTestCode = $@"
public class TestClass
{{
    /// <summary>
    /// {expectedArgument} the first test value.
    /// </summary>
    public {type} TestProperty {accessors}
}}
";

            var expected = this.CSharpDiagnostic(PropertySummaryDocumentationAnalyzer.SA1623Descriptor).WithLocation(7, 13 + type.Length).WithArguments(expectedArgument);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
