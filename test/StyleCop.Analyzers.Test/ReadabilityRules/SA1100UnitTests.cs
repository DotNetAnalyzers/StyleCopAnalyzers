// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists,
        StyleCop.Analyzers.ReadabilityRules.SA1100CodeFixProvider>;

    public class SA1100UnitTests
    {
        /// <summary>
        /// Verifies that changing a base call does not introduce recursive virtual dispatch through a subclass.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2938, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2938")]
        public async Task TestSubclassOverrideCallsIntermediateHelperAsync()
        {
            var testCode = @"
class A : B
{
    public override void DoSomething()
    {
        OtherMethod();
    }
}

class B : C
{
    public void OtherMethod()
    {
        {|#0:base|}.DoSomething();
    }
}

class C
{
    public virtual void DoSomething()
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), testCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that virtual dispatch is preserved for inherited overrides of each supported member kind.
        /// </summary>
        /// <param name="sealedType">Whether the receiver type is sealed.</param>
        /// <param name="sealedMember">Whether the inherited override is sealed.</param>
        /// <param name="member">The member declaration.</param>
        /// <param name="alternateUse">Whether to use a method group, setter, or event removal.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [CombinatorialData]
        [WorkItem(2938, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2938")]
        public async Task TestInheritedOverrideFixSafetyAsync(
            bool sealedType,
            bool sealedMember,
            [CombinatorialValues(
                "void M() { }",
                "int P { get { return 0; } set { } }",
                "int this[int i] { get { return 0; } set { } }",
                "event System.Action E { add { } remove { } }")] string member,
            [CombinatorialValues(false, true)] bool alternateUse)
        {
            string use;
            if (member.StartsWith("void"))
            {
                use = alternateUse ? "System.Action action = RECEIVER.M;" : "RECEIVER.M();";
            }
            else if (member.StartsWith("int P"))
            {
                use = alternateUse ? "RECEIVER.P = 1;" : "var value = RECEIVER.P;";
            }
            else if (member.StartsWith("int this"))
            {
                use = alternateUse ? "RECEIVER[0] = 1;" : "var value = RECEIVER[0];";
            }
            else
            {
                use = alternateUse ? "RECEIVER.E -= () => { };" : "RECEIVER.E += () => { };";
            }

            var testCode = @"
class C
{
    public virtual MEMBER
}

class B : C
{
    public OVERRIDE MEMBER
}

TYPE class A : B
{
    public void Helper()
    {
        USE
    }
}"
                .Replace("MEMBER", member)
                .Replace("OVERRIDE", sealedMember ? "sealed override" : "override")
                .Replace("TYPE", sealedType ? "sealed" : string.Empty)
                .Replace("USE", use.Replace("RECEIVER", "{|#0:base|}"));

            var fixedCode = sealedType || sealedMember ? testCode.Replace("{|#0:base|}", "this") : testCode;
            if (member.StartsWith("void") && alternateUse)
            {
                // The analyzer does not report method groups.
                testCode = testCode.Replace("{|#0:base|}", "base");
                await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that Fix All changes safe calls without changing virtual dispatch or losing comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2938, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2938")]
        public async Task TestFixAllPreservesUnsafeVirtualCallsAsync()
        {
            var testCode = @"
class C
{
    public virtual void VirtualMethod() { }
    public void Method() { }
}

class B : C
{
    public void Helper()
    {
        {|#0:base|} /* keep virtual dispatch */ .VirtualMethod();
        {|#1:base|} /* keep this comment */ .Method();
        {|#2:base|}.Method();
    }
}";

            var fixedCode = testCode.Replace("{|#1:base|}", "this").Replace("{|#2:base|}", "this");
            var unsafeDocument = @"
class D : C
{
    public void Helper()
    {
        {|#3:base|}.VirtualMethod();
    }
}";

            var test = new CSharpTest
            {
                TestSources = { testCode, unsafeDocument },
                FixedSources = { fixedCode, unsafeDocument },
                ExpectedDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1), Diagnostic().WithLocation(2), Diagnostic().WithLocation(3) },
                RemainingDiagnostics = { Diagnostic().WithLocation(0), Diagnostic().WithLocation(3) },
            };
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestChildClassUsesBaseButNoOverrideAsync(string lineEnding)
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        {|#0:base|}.Bar();
    }
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseButMethodIsNotVirtualAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar();
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseBaseAndChildHaveMethodWithSameNameAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar();
    }
    protected void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodWithSameNameButDifferentParametersExistAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }
    protected void Bar(string param)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar();
    }
    protected void Bar(string param)
    {

    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseBaseIsVirtualChildHidesBaseAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar();
    }
    protected void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBasePropertyButNoOverrideAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual string Bar
    {
        get;set;
    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        var s = base.Bar;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(14, 17);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameButDifferentParametersExistAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }
    protected void Bar(string param)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar();
    }
    protected void Bar(string param)
    {

    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameButDifferentParameterTypeAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar("""");
    }
    protected void Bar(long l)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar("""");
    }
    protected void Bar(long l)
    {

    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameRefUsedAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar("""");
    }
    protected void Bar(ref string s)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar("""");
    }
    protected void Bar(ref string s)
    {

    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameOutUsedAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar("""");
    }
    protected void Bar(out string s)
    {
        s = string.Empty;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            var fixedTest = @"
public class Foo
{
    protected void Bar(string s)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.Bar("""");
    }
    protected void Bar(out string s)
    {
        s = string.Empty;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithDifferentParametersExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }
    protected void Bar(string param)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithRefExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar(string s, int i)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar(string.Empty, 5);
    }
    protected void Bar(ref string s, int i)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithOutExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar(string s, int i)
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar(string.Empty, 5);
    }
    protected void Bar(out string s, int i)
    {
        s = string.Empty;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithDifferentParameterTypeExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar(string s, int i)
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar(string.Empty, 5);
    }
    protected void Bar(int i, string s)
    {

    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseOverrideExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }
    protected override void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithFewParametersOverrideExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar(string s, int i)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar(string.Empty,5);
    }
    protected override void Bar(string s, int i)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithFewParametersHidingMethodExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Bar(string s, ref int i)
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        int five = 5;
        base.Bar(string.Empty, ref five);
    }
    protected void Bar(string s, ref int i)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBasePropertyOverrideExistsAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual string Bar
    {
        get;set;
    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        var s = base.Bar;
    }

    protected override string Bar
    {
        get;set;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildEventNoOverrideAsync()
        {
            var testCode = @"
using System;

public class Foo
{
    protected event Action MyEvent;
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.MyEvent += () => { };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(13, 9);

            var fixedTest = @"
using System;

public class Foo
{
    protected event Action MyEvent;
}

public class FooChild : Foo
{
    protected void Baz()
    {
        this.MyEvent += () => { };
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildEventOverrideExistsAsync()
        {
            var testCode = @"
using System;

public class Foo
{
    protected virtual event Action MyEvent;
}

public class FooChild : Foo
{
    protected override event Action MyEvent;

    protected void Baz()
    {
        base.MyEvent += () => { };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildEventHidingEventExistsAsync()
        {
            var testCode = @"
using System;

public class Foo
{
    protected virtual event Action MyEvent;
}

public class FooChild : Foo
{
    protected new event Action MyEvent;

    protected void Baz()
    {
        base.MyEvent += () => { };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStructAsync()
        {
            var testCode = @"
public struct S
{
    public string Baz()
    {
        return base.ToString();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 16);

            var fixedTest = @"
public struct S
{
    public string Baz()
    {
        return this.ToString();
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseButNoOverrideTwoIssuesAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }

    protected virtual void Bar2()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base.Bar();
    }

    protected void Baz2()
    {
        base.Bar2();
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(19, 9),
                    Diagnostic().WithLocation(24, 9),
                };

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithExtraLinesButNoOverrideTwoIssuesAsync()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected void Baz()
    {
        base

        .Bar();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(14, 9);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithPreprocessorDirectivesButNoOverrideTwoIssuesAsync()
        {
            var testCode = @"
public class Foo
{
    protected void Baz()
    {
        #if true
                base
        #else
            this
        #endif
        .ToString();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 17);

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithLocalDefinitionAsync()
        {
            var testCode = @"
class ClassName : System.Collections.Generic.List<int>
{
  public new int this[int index] { get { return base[index]; } }
  public int Property { get { return base[0]; } }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerAsync()
        {
            var testCode = @"
class ClassName : System.Collections.Generic.List<int>
{
  public int Property { get { return base[0]; } }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 38);

            var fixedTest = @"
class ClassName : System.Collections.Generic.List<int>
{
  public int Property { get { return this[0]; } }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithInvokationAsync()
        {
            var testCode = @"
class ClassName : System.Collections.Generic.List<System.Func<int>>
{
  public int Property { get { return base[0](); } }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 38);

            var fixedTest = @"
class ClassName : System.Collections.Generic.List<System.Func<int>>
{
  public int Property { get { return this[0](); } }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
