// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp15.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp14.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.DocumentationRules.SA1615ElementReturnValueMustBeDocumented>;

    public partial class SA1615CSharp15UnitTests : SA1615CSharp14UnitTests
    {
        /// <summary>
        /// Verifies that a union method without return value documentation is reported exactly once. The analyzer
        /// driver in Roslyn 5.9 ran syntax node actions on union members three times (dotnet/roslyn#84570), which
        /// reported this diagnostic three times.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(4182, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4182")]
        public async Task TestUnionMethodReportedOnceAsync()
        {
            var testCode = @"
/// <summary>
/// A union.
/// </summary>
public union Pet(int, string)
{
    /// <summary>
    /// Counts the legs.
    /// </summary>
    public {|#0:int|} Legs()
    {
        return 4;
    }
}
";

            var expected = Diagnostic().WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
