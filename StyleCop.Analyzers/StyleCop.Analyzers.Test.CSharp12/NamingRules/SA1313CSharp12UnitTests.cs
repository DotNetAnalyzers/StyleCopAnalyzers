// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1313CSharp12UnitTests : SA1313CSharp11UnitTests
    {
        /// <summary>
        /// Verifies that the parameters of a class or struct primary constructor are checked. Unlike record parameters,
        /// they do not become properties.
        /// </summary>
        /// <param name="typeKeyword">The type keyword.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        [WorkItem(4006, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4006")]
        public async Task TestPrimaryConstructorParameterAsync(string typeKeyword)
        {
            var testCode = $@"public {typeKeyword} TestType(int {{|#0:Value|}}, int other)
{{
    public int Sum => Value + other;
}}
";

            var fixedCode = $@"public {typeKeyword} TestType(int value, int other)
{{
    public int Sum => value + other;
}}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0).WithArguments("Value"), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
