// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1316TupleElementNamesShouldUseCorrectCasing,
        StyleCop.Analyzers.NamingRules.SA1316CodeFixProvider>;

    public partial class SA1316CSharp10UnitTests : SA1316CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionIsIgnoredAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var tuple = (ValueA: 1, ValueB: 2);
        int existing = 0;
        (existing, int ValueC) = tuple;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that deconstruction into a mix of new and existing variables does not produce diagnostics,
        /// regardless of settings.
        /// </summary>
        /// <param name="settings">The test settings to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(DefaultTestSettings)]
        [InlineData(CamelCaseTestSettings)]
        [InlineData(PascalCaseTestSettings)]
        [InlineData(CamelCaseInferredTestSettings)]
        [InlineData(PascalCaseInferredTestSettings)]
        [InlineData(CamelCaseExplicitOnlyTestSettings)]
        [InlineData(PascalCaseExplicitOnlyTestSettings)]
        [WorkItem(4418, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4418")]
        public async Task TestTupleDeconstructionIntoMixedTargetsAsync(string settings)
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        string Value, other;
        (var name, Value) = (""a"", ""b"");
        ((var first, Value), other) = ((""a"", ""b""), ""c"");
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
