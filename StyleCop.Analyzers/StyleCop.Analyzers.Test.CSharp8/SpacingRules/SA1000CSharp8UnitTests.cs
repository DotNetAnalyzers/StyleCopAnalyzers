// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp8UnitTests : SA1000CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3007")]
        public async Task TestAwaitForEachMissingSpaceAsync()
        {
            string statementWithoutSpace = @"await {|#0:foreach|}(var value in GetValues())
{
    _ = value;
}

async IAsyncEnumerable<int> GetValues()
{
    yield return 1;
}";
            string statementWithSpace = @"await foreach (var value in GetValues())
{
    _ = value;
}

async IAsyncEnumerable<int> GetValues()
{
    yield return 1;
}";

            DiagnosticResult expected = Diagnostic().WithArguments("foreach", string.Empty, "followed").WithLocation(0);

            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace, returnType: "Task", asyncMethod: true).ConfigureAwait(false);
        }
    }
}
