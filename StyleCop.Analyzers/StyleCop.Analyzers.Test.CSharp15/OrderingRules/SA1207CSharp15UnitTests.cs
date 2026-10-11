// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1207ProtectedMustComeBeforeInternal,
        StyleCop.Analyzers.OrderingRules.SA1207CodeFixProvider>;

    public partial class SA1207CSharp15UnitTests : SA1207CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the protected keyword must come before internal on a union declaration, that the violation is
        /// reported exactly once, and that the code fix reorders the keywords.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestNestedUnionAsync()
        {
            var testCode = @"
public class Outer
{
    internal {|#0:protected|} union Pet(int, string);
}
";

            var fixedCode = @"
public class Outer
{
    protected internal union Pet(int, string);
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("protected", "internal");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
