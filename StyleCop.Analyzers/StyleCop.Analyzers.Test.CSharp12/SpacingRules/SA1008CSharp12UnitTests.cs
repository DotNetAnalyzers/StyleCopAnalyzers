// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1008OpeningParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1008CSharp12UnitTests : SA1008CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3743, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3743")]
        public async Task TestTupleUsingAliasAsync()
        {
            const string testCode = @"
using TestAlias ={|#0:(|}string X, bool Y);";

            const string fixedCode = @"
using TestAlias = (string X, bool Y);";

            var expected = Diagnostic(DescriptorPreceded).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3931")]
        public async Task TestParenthesizedLambdaInCollectionExpressionAsync()
        {
            var testCode = @"
class TestClass
{
    private System.Action[] actions = [ [|(|]) => {}];
}
";

            var fixedCode = @"
class TestClass
{
    private System.Action[] actions = [() => {}];
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3931")]
        public async Task TestLambdaAfterCommaInCollectionExpressionAsync()
        {
            var testCode = @"
class TestClass
{
    private System.Func<int, int, int>[] functions = [(x, y) => x + y,{|#0:(|}x, y) => x - y];
}
";

            var fixedCode = @"
class TestClass
{
    private System.Func<int, int, int>[] functions = [(x, y) => x + y, (x, y) => x - y];
}
";

            var expected = Diagnostic(DescriptorPreceded).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3931")]
        public async Task TestLambdaAfterSpreadInCollectionExpressionAsync()
        {
            var testCode = @"
class TestClass
{
    private static System.Func<int, int, int>[] existing = [(x, y) => x + y];
    private System.Func<int, int, int>[] functions = [..existing,{|#0:(|}x, y) => x - y];
}
";

            var fixedCode = @"
class TestClass
{
    private static System.Func<int, int, int>[] existing = [(x, y) => x + y];
    private System.Func<int, int, int>[] functions = [..existing, (x, y) => x - y];
}
";

            var expected = Diagnostic(DescriptorPreceded).WithLocation(0);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3931, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3931")]
        public async Task TestLambdaWithBracketOnPreviousLineAsync()
        {
            var testCode = @"
class TestClass
{
    private System.Func<int, int, int>[] functions = [
        (x, y) => x + y
    ];
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
