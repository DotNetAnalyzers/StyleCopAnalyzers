namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1115UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i, int z,

string s,

int j,
int k)
    {
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(6, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new []{ expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i,
string s)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i, string s)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public Foo(int i, int z,


string s)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorInitializerEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public Foo() : this(1,

2) 
    {
    }

    public Foo(int i, int z)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorInitializerBaseEmptyLinesBetweenParameters()
        {
            var testCode = @"
class FooParent
{
    public FooParent(int i, int j) {}
}
class Foo : FooParent
{
    public Foo(int i, int z) : base(i,

z)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public Foo(int i,
string s)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public Foo(int i, string s)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i, int z)
    {
    }

    public void Baz()
    {
        Bar(1,

2);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i, int z)
    {
    }

    public void Baz()
    {
        Bar(1,
            2);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar(int i, int z)
    {
    }

    public void Baz()
    {
        Bar(1, 2);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public Foo(int i, int z, int j)
    {
    }

    public void Baz()
    {
        var f = new Foo(1,

2,

3);
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(12, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(14, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new [] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public Foo(int i, int z, int j)
    {
    }

    public void Baz()
    {
        var f = new Foo(1,
                        2,
                        3);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public Foo(int i, int z, int j)
    {
    }

    public void Baz()
    {
        var f = new Foo(1, 2, 3);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var b = new Foo()[0,

1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallInConditionalExpressionEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo();
        var b = f?[0,

1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var b = this[0,

1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var b = new Foo()[0,
1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallInConditionalExpressionSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo();
        var b = f?[0,
1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallInObjectInitializerBlankLineBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo() {[0,

1] =3}; 
    }

    public int this[int i, int j]
    {
        get { return 0; }
        set {}
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallInObjectInitializerSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo() {[0,
1] =3}; 
    }

    public int this[int i, int j]
    {
        get { return 0; }
        set {}
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var b = new Foo()[0, 1];
    }

    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public int this[int i, 

int j]
    {
        get { return 0; }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public int this[int i, 
int j]
    {
        get { return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public int this[int i, int j]
    {
        get { return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = new int[3, 

2] { {1, 2}, {3, 4}, {5, 6} };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = new int[3, 
                                 2] { {1, 2}, {3, 4}, {5, 6} };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = new int[3, 2] { {1, 2}, {3, 4}, {5, 6} };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = { {1, 2}, {3, 4}, {5, 6}, {7, 8}, {9, 10} };
        numbers[1, 

1] = 5;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = { {1, 2}, {3, 4}, {5, 6}, {7, 8}, {9, 10} };
        numbers[1, 
                1] = 5;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = { {1, 2}, {3, 4}, {5, 6}, {7, 8}, {9, 10} };
        numbers[1, 1] = 5;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeEmptyLinesBetweenParameters()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeSecondParameterOnTheNextLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersAtTheSameLine()
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
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListEmptyLinesBetweenParameters()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
}

[MyAttribute,

MyAttribute]
class Foo
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListSecondParameterOnTheNextLine()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
}

[MyAttribute,
MyAttribute]
class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListParametersAtTheSameLine()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
}

[MyAttribute, MyAttribute]
class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = delegate(int i,

int j,
int z,

int h) 
{};
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(8, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(11, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = delegate(int i,
                                                        int j,
                                                        int z,
                                                        int h) 
                                                        {};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = delegate(int i, int j, int z, int h) {};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = (i,


j,

k,

z) => {};
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(9, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(11, 1);
            DiagnosticResult expected3 = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2, expected3 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExplicitParametersTypesEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = (int i,


int j,

int k,

int z) => {};
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(9, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(11, 1);
            DiagnosticResult expected3 = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2, expected3 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = (int i,
                                                int j,
                                                int k,
                                                int z) => {};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, int, int, int> a = (int i, int j, int k, int z) => {};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationEmptyLinesBetweenParameters()
        {
            var testCode = @"
class Foo
{
    delegate void Del(int i,

int j,


int z);
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(6, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationSecondParameterOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    delegate void Del(int i,
                        int j,
                        int z);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersAtTheSameLine()
        {
            var testCode = @"
class Foo
{
    delegate void Del(int i, int j, int z);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadEmptyLinesBetweenParameters()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, 

Foo b)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadSecondParameterOnTheNextLine()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, 
                                 Foo b)
    {
        return new Foo();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadParametersAtTheSameLine()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1115ParameterMustFollowComma();
        }
    }
}