// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.SpacingRules.SA1017ClosingAttributeBracketsMustBeSpacedCorrectly,
    StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

public partial class SA1017CSharp8UnitTests : SA1017CSharp7UnitTests
{
    /// <summary>
    /// Verifies spacing after a nullable flow-analysis attribute on an out parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [WorkItem(3118, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3118")]
    public async Task TestMissingSpaceAfterNotNullWhenAttributeAsync()
    {
        var testCode = """
            #nullable enable
            using System.Diagnostics.CodeAnalysis;

            class TestClass
            {
                bool Method([NotNullWhen(true){|#0:]|}out string? foo)
                {
                    foo = "";
                    return true;
                }
            }

            """;
        var fixedCode = """
            #nullable enable
            using System.Diagnostics.CodeAnalysis;

            class TestClass
            {
                bool Method([NotNullWhen(true)] out string? foo)
                {
                    foo = "";
                    return true;
                }
            }

            """;

        await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0).WithMessage("Closing attribute brackets should be followed by a space"), fixedCode, CancellationToken.None).ConfigureAwait(false);
    }
}
