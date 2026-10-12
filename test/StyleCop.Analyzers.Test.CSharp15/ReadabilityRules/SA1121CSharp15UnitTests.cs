// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp15UnitTests : SA1121CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that a type name in a union member is reported exactly once. The analyzer driver in Roslyn 5.9 ran
        /// syntax node actions on union members three times (dotnet/roslyn#84570), which reported this diagnostic
        /// three times.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestTypeInUnionMemberReportedOnceAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public void Feed()
    {
        {|#0:System.Int32|} amount = 3;
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public void Feed()
    {
        int amount = 3;
    }
}
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
