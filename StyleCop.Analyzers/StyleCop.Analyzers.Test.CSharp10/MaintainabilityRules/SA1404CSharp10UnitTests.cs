// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.MaintainabilityRules;
    using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1404CodeAnalysisSuppressionMustHaveJustification,
        StyleCop.Analyzers.MaintainabilityRules.SA1404CodeFixProvider>;

    public class SA1404CSharp10UnitTests : SA1404CSharp9UnitTests
    {
        [Fact]
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

            var test = new CSharpTest
            {
                RemainingDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test1.cs", 4, 32),
                },
                NumberOfIncrementalIterations = 2,
                NumberOfFixAllIterations = 2,
            };
            test.TestState.Sources.Add(testCode1);
            test.TestState.Sources.Add(testCode2);
            test.FixedState.Sources.Add(testCode1);
            test.FixedState.Sources.Add(fixedCode2);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
