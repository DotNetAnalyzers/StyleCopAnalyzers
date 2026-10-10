// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1600ElementsMustBeDocumented,
        StyleCop.Analyzers.DocumentationRules.SA1600CodeFixProvider>;

    public partial class SA1600CSharp12UnitTests : SA1600CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that non-record primary constructor parameters are not treated as properties.
        /// </summary>
        /// <param name="keyword">The type declaration keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(3780, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3780")]
        public async Task TestNonRecordPrimaryConstructorParameterAsync(string keyword)
        {
            var testCode = $@"/// <summary>Type.</summary>
public {keyword} MyType(int parameter)
{{
    /// <summary>Value.</summary>
    public int Value => parameter;
}}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
