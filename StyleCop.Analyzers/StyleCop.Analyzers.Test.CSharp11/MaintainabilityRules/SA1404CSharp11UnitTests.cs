// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.MaintainabilityRules;
    using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1404CodeAnalysisSuppressionMustHaveJustification,
        StyleCop.Analyzers.MaintainabilityRules.SA1404CodeFixProvider>;

    public partial class SA1404CSharp11UnitTests : SA1404CSharp10UnitTests
    {
        // NOTE: This tests a fix for a c# 10 feature, but the Roslyn API used to solve it wasn't available in the version
        // we use in the c# 10 test project, so the test was added here instead.
        [Fact]
        [WorkItem(3594, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3594")]
        public async Task TestUsingNameChangeInGlobalUsingInAnotherFileAsync()
        {
            var testCode1 = @"
global using MySuppressionAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;";

            var testCode2 = @"
public class Foo
{
    [[|MySuppression(null, null)|]]
    public void Bar()
    {

    }
}";

            var fixedCode2 = @"
public class Foo
{
    [MySuppression(null, null, Justification = """ + SA1404CodeAnalysisSuppressionMustHaveJustification.JustificationPlaceholder + @""")]
    public void Bar()
    {

    }
}";

            await new CSharpTest()
            {
                TestSources = { testCode1, testCode2 },
                FixedSources = { testCode1, fixedCode2 },
                RemainingDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test1.cs", 4, 32),
                },
                NumberOfIncrementalIterations = 2,
                NumberOfFixAllIterations = 2,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
