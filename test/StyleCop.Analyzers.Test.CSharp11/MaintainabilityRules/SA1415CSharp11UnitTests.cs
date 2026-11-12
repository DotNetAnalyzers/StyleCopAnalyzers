// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp11UnitTests : SA1415CSharp10UnitTests
{
    [Fact]
    public async Task TestPublicMemberOfFileTypeIsReportedAndFixedAsync()
    {
        var testCode = @"
file class TypeName
{
    {|#0:public|} void MethodName()
    {
    }
}
";
        var fixedCode = @"
file class TypeName
{
    internal void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, fixedCode, expectDiagnostic: true).ConfigureAwait(false);
    }

    private static Task VerifyAsync(string testCode, string fixedCode, bool expectDiagnostic)
    {
        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);
        if (expectDiagnostic)
        {
            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        }

        return test.RunAsync(CancellationToken.None);
    }
}
