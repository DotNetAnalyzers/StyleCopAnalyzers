// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1519BracesMustNotBeOmittedFromMultiLineChildStatement,
        StyleCop.Analyzers.LayoutRules.SA1503CodeFixProvider>;

    public partial class SA1519CSharp8UnitTests : SA1519CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3007")]
        public async Task TestAwaitForEachMultiLineChildRequiresBracesAsync()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestClass
{
    public async Task TestAsync(IAsyncEnumerable<int> values)
    {
        await foreach (var value in values)
            {|#0:System.Console.WriteLine(
                value);|}
    }
}
";
            var fixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestClass
{
    public async Task TestAsync(IAsyncEnumerable<int> values)
    {
        await foreach (var value in values)
        {
            System.Console.WriteLine(
                value);
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
