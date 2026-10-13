// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1642ConstructorSummaryDocumentationMustBeginWithStandardText,
        StyleCop.Analyzers.DocumentationRules.SA1642SA1643CodeFixProvider>;

    public partial class SA1642CSharp9UnitTests
    {
        /// <summary>
        /// Verifies that record class type kinds may be written as a langword see tag.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2144, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2144")]
        public async Task TestStaticConstructorRecordLangwordDocumentationAsync()
        {
            var testCode = @"public record TestRecord
{
    /// <summary>
    /// Initializes static members of the <see cref=""TestRecord""/> <see langword=""record""/>.
    /// </summary>
    static TestRecord() { }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
