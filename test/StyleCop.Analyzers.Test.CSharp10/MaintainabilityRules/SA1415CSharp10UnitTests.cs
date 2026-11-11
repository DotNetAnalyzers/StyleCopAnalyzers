// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp10UnitTests : SA1415CSharp9UnitTests
{
    [Fact]
    public async Task TestParameterlessStructConstructorIsNotReportedAsync()
    {
        var testCode = @"
internal struct TypeName
{
    public TypeName()
    {
    }
}
";

        await VerifyAsync(testCode, testCode, expectDiagnostic: false).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestParameterizedStructConstructorIsReportedAndFixedAsync()
    {
        var testCode = @"
internal struct TypeName
{
    private int value;

    {|#0:public|} TypeName(int value)
    {
        this.value = value;
    }
}
";
        var fixedCode = @"
internal struct TypeName
{
    private int value;

    internal TypeName(int value)
    {
        this.value = value;
    }
}
";

        await VerifyAsync(testCode, fixedCode, expectDiagnostic: true).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestRecordStructCopyConstructorIsReportedAndFixedAsync()
    {
        var testCode = @"
internal record struct TypeName
{
    {|#0:public|} TypeName(TypeName other)
    {
    }
}
";
        var fixedCode = @"
internal record struct TypeName
{
    internal TypeName(TypeName other)
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
