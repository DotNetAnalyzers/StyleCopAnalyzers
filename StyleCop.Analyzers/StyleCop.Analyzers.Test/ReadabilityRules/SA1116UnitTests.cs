namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1116UnitTests : DiagnosticVerifier
    {
        public static IEnumerable<object[]> GetTestDeclarations(string delimiter)
        {
            yield return new object[] { $"public Foo(int a,{delimiter} string s) {{ }}", 16 };
            yield return new object[] { $"public object Bar(int a,{delimiter} string s) => null;", 23 };
            yield return new object[] { $"public static Foo operator + (Foo a,{delimiter} Foo b) => null;", 35 };
            yield return new object[] { $"public object this[int a,{delimiter} string s] => null;", 24 };
            yield return new object[] { $"public delegate void Bar(int a,{delimiter} string s);", 30 };
        }

        public static IEnumerable<object[]> GetTestConstructorInitializers(string delimiter)
        {
            yield return new object[] { $"this(42,{delimiter} \"hello\")" };
            yield return new object[] { $"base(42,{delimiter} \"hello\")" };
        }

        public static IEnumerable<object[]> GetTestExpressions(string delimiter)
        {
            yield return new object[] { $"Bar(1,{delimiter} 2)", 13 };
            yield return new object[] { $"System.Action<int, int> func = (int x,{delimiter} int y) => Bar(x, y)", 41 };
            yield return new object[] { $"System.Action<int, int> func = delegate(int x,{delimiter} int y) {{ Bar(x, y); }}", 49 };
            yield return new object[] { $"new string('a',{delimiter} 2)", 20 };
            yield return new object[] { $"var arr = new string[2,{delimiter} 2];", 30 };
            yield return new object[] { $"char cc = (new char[3, 3])[2,{delimiter} 2];", 36 };
            yield return new object[] { $"char? c = (new char[3, 3])?[2,{delimiter} 2];", 37 };
            yield return new object[] { $"long ll = this[2,{delimiter} 2];", 24 };
        }

        public static IEnumerable<object[]> ValidTestExpressions()
        {
            yield return new object[] { $"System.Action func = () => Bar(0, 3)", 0 };
            yield return new object[] { $"System.Action<int> func = x => Bar(x, 3)", 0 };
            yield return new object[] { $"System.Action func = delegate {{ Bar(0, 0); }}", 0 };
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "")]
        public async Task TestValidDeclarationAsync(string declaration, int column)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestDeclarations), "\r\n")]
        public async Task TestInvalidDeclarationAsync(string declaration, int column)
        {
            var testCode = $@"
class Foo
{{
    {declaration}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, column);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "")]
        public async Task TestValidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, string s)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int i, string z)
        : base(i, z)
    {{
    }}
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestConstructorInitializers), "\r\n")]
        public async Task TestInvalidConstructorInitializerAsync(string initializer)
        {
            var testCode = $@"
class Base
{{
    public Base(int a, string s)
    {{
    }}
}}

class Derived : Base
{{
    public Derived()
        : {initializer}
    {{
    }}

    public Derived(int i, string z)
        : base(i, z)
    {{
    }}
}}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 16);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "")]
        [MemberData(nameof(ValidTestExpressions))]
        public async Task TestValidExpressionAsync(string expression, int column)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int i, int z)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int s] => a + s;
}}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(GetTestExpressions), "\r\n")]
        public async Task TestInvalidExpressionAsync(string expression, int column)
        {
            var testCode = $@"
class Foo
{{
    public void Bar(int i, int z)
    {{
    }}

    public void Baz()
    {{
        {expression};
    }}

    public long this[int a, int s] => a + s;
}}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, column);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitElementAccessAsync()
        {
            var testCode = @"
class Foo
{
    private System.Collections.Generic.Dictionary<int, string> dict = new System.Collections.Generic.Dictionary<int, string>
    {
        [1] = ""foo""
    };
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidAttributeAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(1, 2)]
class Foo
{
}

// This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1211
[System.Obsolete]
class ObsoleteType
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidAttributeAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(int a, int b)
    {
    }
}

[MyAttribute(1,
      2)]
class Foo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 14);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1116SplitParametersMustStartOnLineAfterDeclaration();
        }
    }
}
