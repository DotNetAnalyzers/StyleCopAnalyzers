// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1101PrefixLocalCallsWithThis,
        StyleCop.Analyzers.ReadabilityRules.SA1101CodeFixProvider>;

    public class SA1101CSharp10UnitTests : SA1101CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3472, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3472")]
        public async Task TestExtendedPropertyPatternAsync()
        {
            var testCode = @"public class Test
{
    public Test Inner;
    public string Value;

    public bool Method(Test arg)
    {
        return arg is { Inner.Value: """" };
    }
}";

            await new CSharpTest(LanguageVersion.CSharp10)
            {
                ReferenceAssemblies = GenericAnalyzerTest.ReferenceAssembliesNet60,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
