// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp9UnitTests : SA1000CSharp8UnitTests
    {
        [Fact]
        public async Task TestTargetTypedNewAsync()
        {
            string statementWithoutSpace = "int a = new();";

            await this.TestKeywordStatementAsync(statementWithoutSpace, DiagnosticResult.EmptyDiagnosticResults, statementWithoutSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3974")]
        public async Task TestTargetTypedNewInConditionalExpressionAsync()
        {
            string statement = "bool flag = true; object value = flag ? null : new();";

            await this.TestKeywordStatementAsync(statement, DiagnosticResult.EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        public async Task TestIsBeforeRelationalPatternAsync(string @operator)
        {
            var statementWithoutSpace = $"_ = 1 {{|#0:is|}}{@operator}1;";
            var statementWithSpace = $"_ = 1 is {@operator}1;";

            var expected = Diagnostic().WithArguments("is", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        [InlineData("<")]
        [InlineData("<=")]
        [InlineData(">")]
        [InlineData(">=")]
        public async Task TestNotBeforeRelationalPatternAsync(string relationalOperator)
        {
            var statementWithoutSpace = $"_ = 1 is {{|#0:not|}}{relationalOperator}1;";
            var statementWithSpace = $"_ = 1 is not {relationalOperator}1;";

            var expected = Diagnostic().WithArguments("not", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Theory]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        [CombinatorialData]
        public async Task TestAndBeforeRelationalPatternAsync(
            [CombinatorialValues("and", "or")] string logicalOperator,
            [CombinatorialValues("<", "<=", ">", ">=")] string relationalOperator)
        {
            var statementWithoutSpace = $"_ = (int?)1 is not null {{|#0:{logicalOperator}|}}{relationalOperator}1;";
            var statementWithSpace = $"_ = (int?)1 is not null {logicalOperator} {relationalOperator}1;";

            var expected = Diagnostic().WithArguments(logicalOperator, string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3968")]
        public async Task TestNotBeforeConstantPatternMissingSpaceAsync()
        {
            var statementWithoutSpace = "_ = new object() is {|#0:not|}(null);";
            var statementWithSpace = "_ = new object() is not (null);";

            var expected = Diagnostic().WithArguments("not", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }
    }
}
