// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp8UnitTests : SA1415CSharp7UnitTests
{
    [Fact]
    public async Task TestExplicitPublicMembersOfInternalInterfaceAreNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void ImplicitPublicMethod();
    public void ExplicitPublicMethod();
}
";

        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = testCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
