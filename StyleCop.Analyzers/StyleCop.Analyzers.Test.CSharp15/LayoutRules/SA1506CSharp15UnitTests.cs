// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1506ElementDocumentationHeadersMustNotBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1506CodeFixProvider>;

    public partial class SA1506CSharp15UnitTests : SA1506CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the documentation header of a union must not be followed by a blank line, that the violation is
        /// reported exactly once, and that the code fix removes the blank line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>

public union Pet(int, string);
";

            var fixedCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string);
";

            var expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
