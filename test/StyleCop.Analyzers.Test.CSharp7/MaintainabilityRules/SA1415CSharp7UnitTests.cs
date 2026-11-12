// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.Helpers;
using StyleCop.Analyzers.Test.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp7UnitTests : SA1415UnitTests
{
    [Fact]
    public async Task TestPublicMemberOfPrivateProtectedNestedTypeInInternalTypeIsReportedAndFixedAsync()
    {
        var testCode = @"
internal class Outer
{
    private protected class Inner
    {
        {|#0:public|} void MethodName()
        {
        }
    }
}
";
        var fixedCode = @"
internal class Outer
{
    private protected class Inner
    {
        internal void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfPrivateProtectedNestedTypeInPublicTypeIsReportedAndFixedAsync()
    {
        var testCode = @"
public class Outer
{
    private protected class Inner
    {
        {|#0:public|} void MethodName()
        {
        }
    }
}
";
        var fixedCode = @"
public class Outer
{
    private protected class Inner
    {
        internal void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
    }

    private static Task VerifyAsync(string testCode, string fixedCode, bool expectDiagnostic = true)
    {
        var test = new CSharpTest(LanguageVersion.CSharp7_2.OrLaterDefault())
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
