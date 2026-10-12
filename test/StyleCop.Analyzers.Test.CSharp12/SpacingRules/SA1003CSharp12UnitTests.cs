// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    public partial class SA1003CSharp12UnitTests : SA1003CSharp11UnitTests
    {
        /// <summary>
        /// Verifies the spacing of the <c>=</c> of a default value of a lambda parameter.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4009, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4009")]
        public async Task TestLambdaParameterDefaultValueAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod()
    {
        var a = (int x = 1) => x;
        var b = (int x{|#0:=|}1) => x;
        var c = (int x, int y {|#1:=|}2) => x + y;
    }
}
";

            var fixedCode = @"
class TestClass
{
    void TestMethod()
    {
        var a = (int x = 1) => x;
        var b = (int x = 1) => x;
        var c = (int x, int y = 2) => x + y;
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(0).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(0).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(1).WithArguments("="),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
