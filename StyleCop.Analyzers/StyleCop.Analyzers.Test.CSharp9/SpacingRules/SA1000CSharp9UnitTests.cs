// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
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
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        public async Task TestIsBeforeRelationalPatternAsync()
        {
            var statementWithoutSpace = "_ = 1 {|#0:is|}>1;";
            var statementWithSpace = "_ = 1 is >1;";

            var expected = Diagnostic().WithArguments("is", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        public async Task TestNotBeforeRelationalPatternAsync()
        {
            var statementWithoutSpace = "_ = 1 is {|#0:not|}>1;";
            var statementWithSpace = "_ = 1 is not >1;";

            var expected = Diagnostic().WithArguments("not", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        public async Task TestAndBeforeRelationalPatternAsync()
        {
            var statementWithoutSpace = "_ = 1 is 1 {|#0:and|}>0;";
            var statementWithSpace = "_ = 1 is 1 and >0;";

            var expected = Diagnostic().WithArguments("and", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3508, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3508")]
        public async Task TestOrBeforeRelationalPatternAsync()
        {
            var statementWithoutSpace = "_ = 1 is 1 {|#0:or|}>1;";
            var statementWithSpace = "_ = 1 is 1 or >1;";

            var expected = Diagnostic().WithArguments("or", string.Empty, "followed").WithLocation(0);
            await this.TestKeywordStatementAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3976, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3976")]
        public async Task TestForeachWithExtensionEnumeratorAsync()
        {
            var testCode = @"
using System.Collections.Generic;

public class TestClass
{
    public void TestMethod()
    {
        foreach (var value in new ExtensionEnumerable())
        {
        }
    }
}

public class ExtensionEnumerable
{
}

public static class ExtensionEnumerableExtensions
{
    public static ExtensionEnumerator GetEnumerator(this ExtensionEnumerable value) => new();
}

public struct ExtensionEnumerator
{
    public int Current => 0;

    public bool MoveNext() => false;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
