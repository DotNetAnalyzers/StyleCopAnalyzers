// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1100UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestChildClassUsesBaseButNoOverrideAsync()
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
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        var s = this.Bar;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar();
    }
    protected void Bar(string param)
    {

    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar(string.Empty, 5);
    }
    protected void Bar(ref string s, int i)
    {

    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar(string.Empty, 5);
    }
    protected void Bar(out string s, int i)
    {
        s = string.Empty;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar(string.Empty, 5);
    }
    protected void Bar(int i, string s)
    {

    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
public struct S
{
    public string Baz()
    {
        return this.ToString();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(19, 9),
                    this.CSharpDiagnostic().WithLocation(24, 9)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this.Bar();
    }

    protected void Baz2()
    {
        this.Bar2();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(14, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
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
        this

        .Bar();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
public class Foo
{
    protected void Baz()
    {
        #if true
                this
        #else
            this
        #endif
        .ToString();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerAsync()
        {
            var testCode = @"
class ClassName : System.Collections.Generic.List<int>
{
  public int Property { get { return base[0]; } }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 38);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
class ClassName : System.Collections.Generic.List<int>
{
  public int Property { get { return this[0]; } }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithInvokationAsync()
        {
            var testCode = @"
class ClassName : System.Collections.Generic.List<System.Func<int>>
{
  public int Property { get { return base[0](); } }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 38);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedTest = @"
class ClassName : System.Collections.Generic.List<System.Func<int>>
{
  public int Property { get { return this[0](); } }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1100CodeFixProvider();
        }
    }
}
