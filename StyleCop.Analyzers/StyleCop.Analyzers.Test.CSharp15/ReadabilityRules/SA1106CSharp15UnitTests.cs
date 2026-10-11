// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

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

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
