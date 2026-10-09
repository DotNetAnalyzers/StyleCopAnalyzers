// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp14.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1300ElementMustBeginWithUpperCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToUpperCaseCodeFixProvider>;

    // Union declarations are only parsed with the preview language version, which is the default for this test project.
    // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
    // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1300CSharp15UnitTests : SA1300CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that a union whose name begins with a lower-case letter is reported once, and that a method in a
        /// union whose name begins with a lower-case letter is reported once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
public union {|#0:pet|}(int, string)
{
    public void {|#1:feed|}()
    {
    }
}

public class Outer
{
    public union {|#2:nestedPet|}(int, string);
}
";

            var fixedCode = @"
public union Pet(int, string)
{
    public void Feed()
    {
    }
}

public class Outer
{
    public union NestedPet(int, string);
}
";

            await new CSharpTest()
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithArguments("pet").WithLocation(0),
                    Diagnostic().WithArguments("feed").WithLocation(1),
                    Diagnostic().WithArguments("nestedPet").WithLocation(2),
                },
                CompilerDiagnostics = CompilerDiagnostics.None,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
