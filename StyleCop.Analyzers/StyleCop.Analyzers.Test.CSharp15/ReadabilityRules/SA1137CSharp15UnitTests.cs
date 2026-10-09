// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1137ElementsShouldHaveTheSameIndentation,
        StyleCop.Analyzers.ReadabilityRules.IndentationCodeFixProvider>;

    // Union declarations are only parsed with the preview language version, which is the default for this test project.
    // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
    // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1137CSharp15UnitTests : SA1137CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the members of a union must have the same indentation, that the violation is reported exactly
        /// once, and that the code fix corrects it.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionMembersAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    public int Legs => 4;

{|#0:  |}public int Ears => 2;
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public int Legs => 4;

    public int Ears => 2;
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

        /// <summary>
        /// Verifies that the attribute lists of a union are checked along with the other elements of the containing type,
        /// that the violation is reported exactly once, and that the code fix corrects it.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionWithAttributeAsync()
        {
            var testCode = @"
public class Outer
{
    public int Legs => 4;

{|#0:  |}[System.Obsolete]
    public union Pet(int, string);
}
";

            var fixedCode = @"
public class Outer
{
    public int Legs => 4;

    [System.Obsolete]
    public union Pet(int, string);
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
