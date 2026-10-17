// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;

using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
using Xunit;

public partial class SA1118CSharp8UnitTests : SA1118CSharp7UnitTests
{
    /// <summary>
    /// Verifies that interpolated verbatim strings also support the reversed prefix order.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(4330, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4330")]
    public async Task TestMultilineVerbatimStringWithReversedPrefixArgumentAsync()
    {
        await VerifyMultilineStringArgumentAsync("@$\"first {1}\nsecond\"").ConfigureAwait(false);
    }
}
