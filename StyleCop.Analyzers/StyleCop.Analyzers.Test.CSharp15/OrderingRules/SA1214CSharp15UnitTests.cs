// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    // Union declarations are only parsed with the preview language version, which is the default for this test project.
    // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
    // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1214CSharp15UnitTests : SA1214CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that readonly fields inside a union body must appear before non-readonly fields, that the violation is
        /// reported exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionFieldsAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    private static int count = 0;

    private static readonly int {|#0:legs|} = 4;
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    private static readonly int legs = 4;

    private static int count = 0;
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
