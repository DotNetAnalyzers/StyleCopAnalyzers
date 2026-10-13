// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText,
        StyleCop.Analyzers.DocumentationRules.SA1642SA1643CodeFixProvider>;

    public partial class SA1642CSharp10UnitTests : SA1642CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that record type kinds may be written as a langword see tag.
        /// </summary>
        /// <param name="typeKind">The record declaration kind.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("record")]
        [InlineData("record struct")]
        [WorkItem(2144, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2144")]
        public async Task TestStaticConstructorRecordLangwordDocumentationAsync(string typeKind)
        {
            var testCode = $@"public {typeKind} TestRecord
{{
    /// <summary>
    /// Initializes static members of the <see cref=""TestRecord""/> <see langword=""record""/>.
    /// </summary>
    static TestRecord() {{ }}
}}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3980, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3980")]
        public async Task TestParameterlessStructConstructorAsync()
        {
            var testCode = @"
public struct TestStruct
{
    /// [|<summary>
    /// Creates a new value.
    /// </summary>|]
    public TestStruct()
    {
    }
}
";

            var fixedCode = @"
public struct TestStruct
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestStruct""/> struct.
    /// Creates a new value.
    /// </summary>
    public TestStruct()
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
