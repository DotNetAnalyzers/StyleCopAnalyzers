// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp12UnitTests : SA1001CSharp11UnitTests
    {
        /// <summary>
        /// Verifies the spacing of commas in a collection expression.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4007")]
        public async Task TestCollectionExpressionAsync()
        {
            var testCode = @"
class TestClass
{
    static int[] field1 = [1 {|#0:,|} 2];
    static int[] field2 = [1{|#1:,|}2];
    static int[] field3 = [.. field1{|#2:,|}.. field2];
}
";

            var fixedCode = @"
class TestClass
{
    static int[] field1 = [1, 2];
    static int[] field2 = [1, 2];
    static int[] field3 = [.. field1, .. field2];
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(1).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(2).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
