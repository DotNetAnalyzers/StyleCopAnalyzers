// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.CSharp13.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp14UnitTests : SA1415CSharp13UnitTests
{
    [Fact]
    public async Task TestPublicExtensionMemberOfInternalClassIsReportedAndFixedAsync()
    {
        var testCode = @"
internal static class Extensions
{
    extension(int value)
    {
        {|#0:public|} int Doubled => value * 2;
    }
}
";
        var fixedCode = @"
internal static class Extensions
{
    extension(int value)
    {
        internal int Doubled => value * 2;
    }
}
";

        await VerifyAsync(testCode, fixedCode, expectDiagnostic: true).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicExtensionMemberOfPublicClassIsNotReportedAsync()
    {
        var testCode = @"
public static class Extensions
{
    extension(int value)
    {
        public int Doubled => value * 2;
    }
}
";

        await VerifyAsync(testCode, testCode, expectDiagnostic: false).ConfigureAwait(false);
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
