// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public partial class SA1101CSharp8UnitTests : SA1101CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3005")]
        public async Task TestStaticLocalFunctionCallAsync()
        {
            var testCode = @"public class TestClass
{
    public void Method()
    {
        static void LocalFunction()
        {
        }

        LocalFunction();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3472")]
        public async Task TestPropertyPatternAsync()
        {
            var testCode = @"public class Test
{
    public Test Inner;
    public string Value;

    public bool Method(Test arg)
    {
        return arg is { Value: """" };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
