// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public partial class SA1117CSharp15UnitTests : SA1117CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that the case types of a union must all be on the same line or each on its own line, and that the
        /// violation is reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
public union Pet(
    int, string,
    {|#0:long|});
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
