// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1015ClosingGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1015CSharp10UnitTests : SA1015CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3624, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3624")]
        public async Task TestLambdaWithExplicitGenericReturnTypeAsync()
        {
            const string testCode = @"using System.Threading.Tasks;

public class TestClass
{
    public void TestMethod()
    {
        var _ = Task<int[|>|](int x) => Task.FromResult(x);
    }
}";

            const string fixedCode = @"using System.Threading.Tasks;

public class TestClass
{
    public void TestMethod()
    {
        var _ = Task<int> (int x) => Task.FromResult(x);
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
