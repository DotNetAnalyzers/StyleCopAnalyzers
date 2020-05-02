// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1024CSharp8UnitTests : SA1024CSharp7UnitTests
    {
        /// <summary>
        /// Verifies the behavior of colons in case patterns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        /// <seealso cref="SA1013CSharp8UnitTests.TestSpacingAroundClosingBraceInPatternAsync"/>
        [Fact]
        [WorkItem(3053, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3053")]
        public async Task TestColonAfterClosingBraceInPatternAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        case Exception { Message: { } message } :
            break;
        }
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public void TestMethod(object value)
    {
        switch (value)
        {
        case Exception { Message: { } message }:
            break;
        }
    }
}";

            var expected = Diagnostic(DescriptorNotPreceded).WithSpan(9, 49, 9, 50);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
