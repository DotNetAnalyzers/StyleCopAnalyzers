namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1100UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId;

        [Fact]
        public async Task TestChildClassUsesBaseButNoOverride()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseButMethodIsNotVirtual()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseBaseAndChildHaveMethodWithSameName()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodWithSameNameButDifferentParametersExist()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseBaseIsVirtualChildHidesBase()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBasePropertyButNoOverride()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameButDifferentParametersExist()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameButDifferentParameterType()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameRefUsed()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodMethodWithSameNameOutUsed()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithDifferentParametersExists()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithRefExists()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithOutExists()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseMethodOverrideWithDifferentParameterTypeExists()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseOverrideExists()
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
    protected override void Baz()
    {
        base.Bar();
    }
    protected override void Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);

        }

        [Fact]
        public async Task TestChildClassUsesBaseWithFewParametersOverrideExists()
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
    protected override void Baz()
    {
        base.Bar(string.Empty,5);
    }
    protected override void Bar(string s, int i)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);

        }

        [Fact]
        public async Task TestChildClassUsesBaseWithFewParametersHidingMethodExists()
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
    protected override void Baz()
    {
        base.Bar(string.Empty,5);
    }
    protected void Bar(string s, ref int i)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);

        }

        [Fact]
        public async Task TestChildClassUsesBasePropertyOverrideExists()
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
    protected override void Baz()
    {
        var s = base.Bar;
    }

    protected override string Bar
    {
        get;set;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestChildEventNoOverride()
        {
            var testCode = @"
public class Foo
{
    protected event Action MyEvent;
}

public class FooChild : Foo
{
    protected void Baz()
    {
        if(base.MyEvent != null)
        {

        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 12);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTest = @"
public class Foo
{
    protected event Action MyEvent;
}

public class FooChild : Foo
{
    protected void Baz()
    {
        if(this.MyEvent != null)
        {

        }
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildEventOverrideExists()
        {
            var testCode = @"
public class Foo
{
    protected virtual event Action MyEvent;
}

public class FooChild : Foo
{
    protected override event Action MyEvent;

    protected void Baz()
    {
        if(base.MyEvent != null)
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestChildEventHidingEventExists()
        {
            var testCode = @"
public class Foo
{
    protected virtual event Action MyEvent;
}

public class FooChild : Foo
{
    protected new event Action MyEvent;

    protected void Baz()
    {
        if(base.MyEvent != null)
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestStruct()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedTest = @"
public struct S
{
    public string Baz()
    {
        return this.ToString();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseButNoOverrideTwoIssues()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithExtraLinesButNoOverrideTwoIssues()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestChildClassUsesBaseWithPreprocessorDirectivesButNoOverrideTwoIssues()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1100CodeFixProvider();
        }
    }
}
