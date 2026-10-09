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
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess,
        StyleCop.Analyzers.OrderingRules.ElementOrderCodeFixProvider>;

    // Union declarations are only parsed with the preview language version. The reference assemblies used by these
    // tests do not define System.Runtime.CompilerServices.IUnion and UnionAttribute, so union declarations produce
    // CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1202CSharp15UnitTests : SA1202CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that union declarations are ordered by access. Like record structs, unions are compared with other
        /// declarations of the same kind only.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPublicUnionAfterInternalUnionAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public struct Struct1
    {
    }

    internal union Union1(int, string);

    public union {|#0:Union2|}(int, string);
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public struct Struct1
    {
    }

    public union Union2(int, string);

    internal union Union1(int, string);
}
";

            await new CSharpTest(LanguageVersion.Preview)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0).WithArguments("public", "internal") },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested union declarations are ordered by access.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestPublicNestedUnionAfterPrivateNestedUnionAsync()
        {
            var testCode = @"
public class OuterClass
{
    private union Union1(int, string);

    public union {|#0:Union2|}(int, string);
}
";

            var fixedCode = @"
public class OuterClass
{
    public union Union2(int, string);

    private union Union1(int, string);
}
";

            await new CSharpTest(LanguageVersion.Preview)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0).WithArguments("public", "private") },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that members inside a union body are ordered by access, that each misplaced member is reported
        /// exactly once, and that the code fix reorders them.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestMembersInsideUnionAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
    private void Feed()
    {
    }

    public void {|#0:Walk|}()
    {
    }
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public void Walk()
    {
    }

    private void Feed()
    {
    }
}
";

            await new CSharpTest(LanguageVersion.Preview)
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0).WithArguments("public", "private") },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
