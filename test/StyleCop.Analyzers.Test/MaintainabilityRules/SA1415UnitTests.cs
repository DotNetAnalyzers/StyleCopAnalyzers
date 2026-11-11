// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public class SA1415UnitTests
{
    private const string DiagnosticId = SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId;

    [Fact]
    public void TestDisabledByDefault()
    {
        var analyzer = new SA1415DoNotUsePublicOnMembersOfInternalTypes();
        Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
    }

    [Fact]
    public async Task TestNotReportedWhenDiagnosticIsDisabledAsync()
    {
        var testCode = @"
internal class TypeName
{
    public void MethodName()
    {
    }
}
";

        var test = new CSharpTest
        {
            TestCode = testCode,
        };
        test.DisabledDiagnostics.Add(DiagnosticId);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfInternalClassIsReportedAndFixedAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} void MethodName()
    {
    }
}
";
        var fixedCode = @"
internal class TypeName
{
    internal void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfPrivateNestedClassIsReportedAndFixedAsync()
    {
        var testCode = @"
public class Outer
{
    private class Inner
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
    private class Inner
    {
        internal void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicNestedTypeOfInternalClassIsReportedAndFixedAsync()
    {
        var testCode = @"
internal class Outer
{
    {|#0:public|} class Inner
    {
        {|#1:public|} void MethodName()
        {
        }
    }
}
";
        var fixedCode = @"
internal class Outer
{
    internal class Inner
    {
        internal void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfPublicClassIsNotReportedAsync()
    {
        var testCode = @"
public class TypeName
{
    public void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInternalMemberOfInternalClassIsNotReportedAsync()
    {
        var testCode = @"
internal class TypeName
{
    internal void MethodName()
    {
    }

    private void PrivateMethod()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfProtectedNestedTypeInPublicTypeIsNotReportedAsync()
    {
        var testCode = @"
public class Outer
{
    protected class Inner
    {
        public void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfProtectedNestedTypeInInternalTypeIsReportedAndFixedAsync()
    {
        var testCode = @"
internal class Outer
{
    protected class Inner
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
    protected class Inner
    {
        internal void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfPublicTypeNestedInInternalTypeIsReportedAndFixedAsync()
    {
        var testCode = @"
internal class Outer
{
    {|#0:public|} class Inner
    {
        {|#1:public|} int Field;
    }
}
";
        var fixedCode = @"
internal class Outer
{
    internal class Inner
    {
        internal int Field;
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInterfaceImplementationIsNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void MethodName();
    int PropertyName { get; }
    int this[int index] { get; }
    event System.EventHandler EventName;
}

internal class TypeName : IName
{
    public void MethodName()
    {
    }

    public int PropertyName { get { return 0; } }

    public int this[int index] { get { return 0; } }

    public event System.EventHandler EventName;
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestExplicitInterfaceImplementationIsNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void MethodName();
}

internal class TypeName : IName
{
    void IName.MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestOverrideOfPublicMemberIsNotReportedAsync()
    {
        var testCode = @"
public class BaseName
{
    public virtual void MethodName()
    {
    }
}

internal class TypeName : BaseName
{
    public override void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestOperatorsAreNotReportedAsync()
    {
        var testCode = @"
internal class TypeName
{
    public static TypeName operator +(TypeName left, TypeName right)
    {
        return left;
    }

    public static explicit operator int(TypeName value)
    {
        return 0;
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMembersOfInternalInterfaceAreNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void ImplicitPublicMethod();
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicConstructorPropertyEventAndDelegateAreReportedAndFixedAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} TypeName()
    {
    }

    {|#1:public|} int PropertyName { get; set; }

    {|#2:public|} event System.EventHandler EventName
    {
        add { }
        remove { }
    }

    {|#3:public|} delegate void Handler();
}
";
        var fixedCode = @"
internal class TypeName
{
    internal TypeName()
    {
    }

    internal int PropertyName { get; set; }

    internal event System.EventHandler EventName
    {
        add { }
        remove { }
    }

    internal delegate void Handler();
}
";

        await VerifyAsync(
            testCode,
            fixedCode,
            Diagnostic().WithLocation(0),
            Diagnostic().WithLocation(1),
            Diagnostic().WithLocation(2),
            Diagnostic().WithLocation(3)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicStructEnumAndNestedInterfaceAreReportedAndFixedAsync()
    {
        var testCode = @"
internal class Outer
{
    {|#0:public|} struct NestedStruct
    {
    }

    {|#1:public|} enum NestedEnum
    {
        Value
    }

    {|#2:public|} interface INested
    {
        void MethodName();
    }
}
";
        var fixedCode = @"
internal class Outer
{
    internal struct NestedStruct
    {
    }

    internal enum NestedEnum
    {
        Value
    }

    internal interface INested
    {
        void MethodName();
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfInternalStructIsReportedAndFixedAsync()
    {
        var testCode = @"
internal struct TypeName
{
    {|#0:public|} int Field;
}
";
        var fixedCode = @"
internal struct TypeName
{
    internal int Field;
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestProtectedInternalAndProtectedMembersOfInternalTypeAreNotReportedAsync()
    {
        var testCode = @"
internal class TypeName
{
    protected void ProtectedMethod()
    {
    }

    protected internal void ProtectedInternalMethod()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicMemberOfProtectedInternalNestedTypeInPublicTypeIsNotReportedAsync()
    {
        var testCode = @"
public class Outer
{
    protected internal class Inner
    {
        public void MethodName()
        {
        }
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestImplicitlyPrivateMemberIsNotReportedAsync()
    {
        var testCode = @"
internal class TypeName
{
    void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPublicTopLevelTypeIsNotReportedAsync()
    {
        var testCode = @"
public class TypeName
{
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestMethodThatDoesNotImplementInterfaceIsReportedAndFixedAsync()
    {
        var testCode = @"
internal interface IName
{
    void MethodName();
}

internal class TypeName : IName
{
    public void MethodName()
    {
    }

    {|#0:public|} void OtherMethod()
    {
    }
}
";
        var fixedCode = @"
internal interface IName
{
    void MethodName();
}

internal class TypeName : IName
{
    public void MethodName()
    {
    }

    internal void OtherMethod()
    {
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    private static Task VerifyAsync(string testCode, string fixedCode, params DiagnosticResult[] expected)
    {
        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(DiagnosticId);
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}
