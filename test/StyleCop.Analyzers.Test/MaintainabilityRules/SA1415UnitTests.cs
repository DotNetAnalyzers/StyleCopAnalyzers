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
    {|#0:public|} TypeName(int value)
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
    internal TypeName(int value)
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

    [Fact]
    public async Task TestParameterlessConstructorIsNotReportedAsync()
    {
        var testCode = @"
internal class TypeName
{
    public TypeName()
    {
    }
}

internal class Consumer
{
    internal void Use<T>()
        where T : new()
    {
        new T();
    }

    internal void Call()
    {
        this.Use<TypeName>();
    }
}
";

        await VerifyAsync(testCode, testCode).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEnumerationPatternMembersAreNotReportedAsync()
    {
        var testCode = @"
internal class Collection
{
    public Enumerator GetEnumerator()
    {
        return new Enumerator();
    }

    {|#0:public|} void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public bool MoveNext()
    {
        return false;
    }
}

internal class Consumer
{
    internal void M(Collection values)
    {
        foreach (var value in values)
        {
        }
    }
}
";
        var fixedCode = @"
internal class Collection
{
    public Enumerator GetEnumerator()
    {
        return new Enumerator();
    }

    internal void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public bool MoveNext()
    {
        return false;
    }
}

internal class Consumer
{
    internal void M(Collection values)
    {
        foreach (var value in values)
        {
        }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestEnumerationPatternNamesThatAreNotThePatternAreReportedAndFixedAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} int GetEnumerator()
    {
        return 0;
    }

    {|#1:public|} void MoveNext()
    {
    }

    {|#2:public|} int Current { get; }
}
";
        var fixedCode = @"
internal class TypeName
{
    internal int GetEnumerator()
    {
        return 0;
    }

    internal void MoveNext()
    {
    }

    internal int Current { get; }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestAttributeFieldAndSettablePropertyAreNotReportedAsync()
    {
        var testCode = @"
internal class AAttribute : System.Attribute
{
    public int Value;

    public int Property { get; set; }

    {|#0:public|} int ReadOnly { get; }
}

[A(Value = 1, Property = 2)]
internal class Consumer
{
}
";
        var fixedCode = @"
internal class AAttribute : System.Attribute
{
    public int Value;

    public int Property { get; set; }

    internal int ReadOnly { get; }
}

[A(Value = 1, Property = 2)]
internal class Consumer
{
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInheritedInterfaceImplementationIsNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void MethodName();
}

internal class BaseName
{
    public void MethodName()
    {
    }

    {|#0:public|} void OtherMethod()
    {
    }
}

internal class DerivedName : BaseName, IName
{
}
";
        var fixedCode = @"
internal interface IName
{
    void MethodName();
}

internal class BaseName
{
    public void MethodName()
    {
    }

    internal void OtherMethod()
    {
    }
}

internal class DerivedName : BaseName, IName
{
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestInternalAccessorModifierIsRemovedAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} int PropertyName { get; internal set; }

    {|#1:public|} int this[int index]
    {
        get { return 0; }
        internal set { }
    }
}
";
        var fixedCode = @"
internal class TypeName
{
    internal int PropertyName { get; set; }

    internal int this[int index]
    {
        get { return 0; }
        set { }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestPrivateAccessorModifierIsPreservedAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} int PropertyName { get; private set; }

    {|#1:public|} int this[int index]
    {
        get { return 0; }
        private set { }
    }
}
";
        var fixedCode = @"
internal class TypeName
{
    internal int PropertyName { get; private set; }

    internal int this[int index]
    {
        get { return 0; }
        private set { }
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestProtectedAccessorPreventsCodeFixAsync()
    {
        var testCode = @"
internal class TypeName
{
    {|#0:public|} int PropertyName { get; protected set; }

    {|#1:public|} int OtherProperty { get; protected internal set; }

    {|#2:public|} int this[int index]
    {
        get { return 0; }
        protected set { }
    }
}
";

        await VerifyNoFixAsync(testCode, Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestVirtualMethodAndOverrideAreUpdatedTogetherAsync()
    {
        var testCode = @"
internal class BaseName
{
    {|#0:public|} virtual void MethodName()
    {
    }
}

internal class DerivedName : BaseName
{
    public override void MethodName()
    {
    }
}
";
        var fixedCode = @"
internal class BaseName
{
    internal virtual void MethodName()
    {
    }
}

internal class DerivedName : BaseName
{
    internal override void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestAbstractMethodAndOverrideAreUpdatedTogetherAsync()
    {
        var testCode = @"
internal abstract class BaseName
{
    {|#0:public|} abstract void MethodName();
}

internal class DerivedName : BaseName
{
    public override void MethodName()
    {
    }
}
";
        var fixedCode = @"
internal abstract class BaseName
{
    internal abstract void MethodName();
}

internal class DerivedName : BaseName
{
    internal override void MethodName()
    {
    }
}
";

        await VerifyAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestOverrideInGeneratedCodePreventsCodeFixAsync()
    {
        var testCode = @"
internal class BaseName
{
    {|#0:public|} virtual void MethodName()
    {
    }
}
";
        var generatedCode = @"
// <auto-generated/>
internal class DerivedName : BaseName
{
    public override void MethodName()
    {
    }
}
";

        var test = new CSharpTest();
        test.TestState.Sources.Add(("BaseName.cs", testCode));
        test.TestState.Sources.Add(("DerivedName.generated.cs", generatedCode));
        test.FixedState.Sources.Add(("BaseName.cs", testCode));
        test.FixedState.Sources.Add(("DerivedName.generated.cs", generatedCode));
        test.ExplicitlyEnabledDiagnostics.Add(DiagnosticId);
        var expected = Diagnostic().WithLocation(0);
        test.ExpectedDiagnostics.Add(expected);
        test.RemainingDiagnostics.Add(expected);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    private static Task VerifyNoFixAsync(string testCode, params DiagnosticResult[] expected)
    {
        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = testCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(DiagnosticId);
        test.ExpectedDiagnostics.AddRange(expected);
        test.RemainingDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
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
