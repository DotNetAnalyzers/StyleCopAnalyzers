// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1203ConstantsMustAppearBeforeFields,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    public partial class SA1203CSharp15UnitTests : SA1203CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that constants inside a union body must appear before fields, that each misplaced constant is
        /// reported exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestMembersInsideUnionAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public static int Count = 0;

    public const int {|#0:Legs|} = 4;
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public const int Legs = 4;

    public static int Count = 0;
}
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
