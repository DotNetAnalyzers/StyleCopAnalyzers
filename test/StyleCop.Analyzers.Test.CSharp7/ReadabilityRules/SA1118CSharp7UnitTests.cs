// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1118ParameterMustNotSpanMultipleLines>;

    public partial class SA1118CSharp7UnitTests : SA1118UnitTests
    {
        [Fact]
        [WorkItem(3707, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3707")]
        public async Task TestTupleArgumentSpanningMultipleLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Method(int i, (string, string) pair)
    {
    }

    public void Test()
    {
        Method(
            1,
            (
              ""first"",
              ""second""));
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
