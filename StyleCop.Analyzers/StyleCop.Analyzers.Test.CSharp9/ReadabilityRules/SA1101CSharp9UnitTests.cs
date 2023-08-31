// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public class SA1101CSharp9UnitTests : SA1101CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3201, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3201")]
        public async Task TestRecordWithExpressionAsync()
        {
            var testCode = @"public class Test
{
    public record A
    {
        public string Prop { get; init; }
    }

    public A UpdateA(A value)
    {
        return value with { Prop = ""newValue"" };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
