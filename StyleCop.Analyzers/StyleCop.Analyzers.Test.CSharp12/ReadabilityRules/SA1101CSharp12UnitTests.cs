// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp12UnitTests : SA1101CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that primary constructor parameters are not reported, because they cannot be accessed through
        /// <see langword="this"/>, but that a field with the same name, which shadows the parameter in member bodies, is.
        /// </summary>
        /// <param name="typeKeyword">The type keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorParameterAsync(string typeKeyword)
        {
            var testCode = $@"public {typeKeyword} TestType(int value, int other)
{{
    private readonly int value = value;

    public int Sum() => {{|#0:value|}} + other;

    public int Sum2() => this.value + other;
}}
";

            var fixedCode = $@"public {typeKeyword} TestType(int value, int other)
{{
    private readonly int value = value;

    public int Sum() => this.value + other;

    public int Sum2() => this.value + other;
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
