// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1204StaticElementsMustAppearBeforeInstanceElements,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    // Union declarations are only parsed with the preview language version. The reference assemblies used by these
    // tests do not define System.Runtime.CompilerServices.IUnion and UnionAttribute, so union declarations produce
    // CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1204CSharp15UnitTests : SA1204CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that static members inside a union body must appear before instance members, that each misplaced
        /// static member is reported exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestMembersInsideUnionAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public void Feed()
    {
    }

    public static Pet {|#0:Create|}()
    {
        return default;
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public static Pet Create()
    {
        return default;
    }

    public void Feed()
    {
    }
}
";

            await new CSharpTest(LanguageVersion.Preview)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
