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
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    // Union declarations are only parsed with the preview language version, which is the default for this test project.
    // The reference assemblies used by these tests do not define System.Runtime.CompilerServices.IUnion and
    // UnionAttribute, so union declarations produce CS0518 and CS0656, and compiler diagnostics are therefore ignored.
    public partial class SA1106CSharp15UnitTests : SA1106CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that an empty statement after a union body is reported exactly once and removed by the code fix, and
        /// that the semicolon ending a union without a body is not reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
public union Pet(int, string)
{
}{|#0:;|}

public union Shape(int, string);
";

            var fixedCode = @"
public union Pet(int, string)
{
}

public union Shape(int, string);
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
