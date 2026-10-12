// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1604ElementDocumentationMustHaveSummary>;

    public partial class SA1604CSharp15UnitTests : SA1604CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that union documentation without a summary is reported exactly once.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionDeclarationAsync()
        {
            var testCode = @"
/// <remarks>A union.</remarks>
public union {|#0:Pet|}(int, string);
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
