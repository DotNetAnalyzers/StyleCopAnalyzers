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
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp12UnitTests : SA1011CSharp11UnitTests
    {
        /// <summary>
        /// Verifies the spacing of the closing bracket of a collection expression.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4007")]
        public async Task TestCollectionExpressionAsync()
        {
            var testCode = @"
class TestClass
{
    int[] field1 = [1, 2 {|#0:]|};
    int[][] field2 = [[1], [2 {|#1:]|}];
    int[] field3 = [];

    int[] TestMethod(bool b) => b ? [1, 2] : [];
}
";

            var fixedCode = @"
class TestClass
{
    int[] field1 = [1, 2];
    int[][] field2 = [[1], [2]];
    int[] field3 = [];

    int[] TestMethod(bool b) => b ? [1, 2] : [];
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments(" not", "preceded"),
                Diagnostic().WithLocation(1).WithArguments(" not", "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
