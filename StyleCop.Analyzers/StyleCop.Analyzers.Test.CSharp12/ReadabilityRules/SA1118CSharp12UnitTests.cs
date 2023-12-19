// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.ReadabilityRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

    public partial class SA1118CSharp12UnitTests : SA1118CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3732, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3732")]
        public async Task TestCollectionExpressionAsync()
        {
            var testCode = @"
class Foo
{
    public void TestMethod()
    {
        AnotherMethod(
            42,
            [
                1,
                2,
                3
            ]);
    }

    public void AnotherMethod(int x, int[] y)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
