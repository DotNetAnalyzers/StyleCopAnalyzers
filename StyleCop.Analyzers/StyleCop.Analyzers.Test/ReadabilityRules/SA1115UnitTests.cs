// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1115UnitTests : DiagnosticVerifier
    {
        [Fact]
        public async Task TestMethodDeclarationEmptyLinesBetweenParametersAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationSecondParameterOnTheNextLineAsync()
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
        public async Task TestMethodDeclarationParametersAtTheSameLineAsync()
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
        public async Task TestConstructorEmptyLinesBetweenParametersAsync()
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
        public async Task TestConstructorInitializerEmptyLinesBetweenParametersAsync()
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
        public async Task TestConstructorInitializerBaseEmptyLinesBetweenParametersAsync()
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
        public async Task TestConstructorSecondParameterOnTheNextLineAsync()
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
        public async Task TestConstructorParametersAtTheSameLineAsync()
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
        public async Task TestMethodCallEmptyLinesBetweenParametersAsync()
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
        public async Task TestMethodCallSecondParameterOnTheNextLineAsync()
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
        public async Task TestMethodCallFirstParameterIsMultiLineAsync()
        {
            var testCode = @"
class Foo
{
    public int Bar(int i, int z)
    {
        return 1;
    }

    public void Baz()
    {
        Bar(
            Bar(
                1,
                2),
            2);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallSecondParameterIsMultiLineAsync()
        {
            var testCode = @"
class Foo
{
    public int Bar(int a, int i, int z)
    {
        return 1;
    }

    public void Baz()
    {
        Bar(
            0,
            Bar(
                0,
                1,
                2),
            2);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersAtTheSameLineAsync()
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
        public async Task TestConstructorCallEmptyLinesBetweenParametersAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallSecondParameterOnTheNextLineAsync()
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
        public async Task TestConstructorCallParametersAtTheSameLineAsync()
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
        public async Task TestIndexerCallEmptyLinesBetweenParametersAsync()
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
        public async Task TestIndexerCallInConditionalExpressionEmptyLinesBetweenParametersAsync()
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
        public async Task TestIndexerCallUsingThisEmptyLinesBetweenParametersAsync()
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
        public async Task TestIndexerCallSecondParameterOnTheNextLineAsync()
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
        public async Task TestIndexerCallInConditionalExpressionSecondParameterOnTheNextLineAsync()
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
        public async Task TestIndexerCallInObjectInitializerBlankLineBetweenParametersAsync()
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
        public async Task TestIndexerCallInObjectInitializerSecondParameterOnTheNextLineAsync()
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
        public async Task TestIndexerCallParametersAtTheSameLineAsync()
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
        public async Task TestIndexerDeclarationEmptyLinesBetweenParametersAsync()
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
        public async Task TestIndexerDeclarationSecondParameterOnTheNextLineAsync()
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
        public async Task TestIndexerDeclarationParametersAtTheSameLineAsync()
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
        public async Task TestArrayCreationEmptyLinesBetweenParametersAsync()
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
        public async Task TestArrayCreationSecondParameterOnTheNextLineAsync()
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
        public async Task TestArrayCreationParametersAtTheSameLineAsync()
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
        public async Task TestArrayCallEmptyLinesBetweenParametersAsync()
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
        public async Task TestArrayCallSecondParameterOnTheNextLineAsync()
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
        public async Task TestArrayCallParametersAtTheSameLineAsync()
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
        public async Task TestAttributeEmptyLinesBetweenParametersAsync()
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
        public async Task TestAttributeSecondParameterOnTheNextLineAsync()
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
        public async Task TestAttributeParametersAtTheSameLineAsync()
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
        public async Task TestAttributeParametersFirstParameterIsMultiLineAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class)]
public class MyAttribute : System.Attribute
{
    public MyAttribute(string a, int b)
    {
    }
}

[MyAttribute(
    @""Some
    long string"",
    2)]
class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListEmptyLinesBetweenParametersAsync()
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
        public async Task TestAttributeListFirstAttributeParametersAreMultiLineAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
    public MyAttribute() { }

    public MyAttribute(int a, int b) { }
}

[MyAttribute(
    1,
    2),
MyAttribute]
class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesFirstAttributeParametersAreMultiLineAsync()
        {
            var testCode = @"
[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class MyAttribute : System.Attribute
{
    public MyAttribute() { }

    public MyAttribute(int a, int b) { }
}

[MyAttribute(
    1,
    2)]
[MyAttribute]
class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListSecondParameterOnTheNextLineAsync()
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
        public async Task TestAttributeListParametersAtTheSameLineAsync()
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
        public async Task TestAnonymousMethodDeclarationEmptyLinesBetweenParametersAsync()
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
        public async Task TestAnonymousMethodDeclarationSecondParameterOnTheNextLineAsync()
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
        public async Task TestAnonymousMethodDeclarationParametersAtTheSameLineAsync()
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
        public async Task TestLambdaEmptyLinesBetweenParametersAsync()
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
        public async Task TestLambdaExplicitParametersTypesEmptyLinesBetweenParametersAsync()
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
        public async Task TestLambdaSecondParameterOnTheNextLineAsync()
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
        public async Task TestLambdaParametersAtTheSameLineAsync()
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
        public async Task TestDelegateDeclarationEmptyLinesBetweenParametersAsync()
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
        public async Task TestDelegateDeclarationSecondParameterOnTheNextLineAsync()
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
        public async Task TestDelegateDeclarationParametersAtTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    delegate void Del(int i, int j, int z);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadEmptyLinesBetweenParametersAsync()
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
        public async Task TestOperatorOverloadSecondParameterOnTheNextLineAsync()
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
        public async Task TestOperatorOverloadParametersAtTheSameLineAsync()
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

        /// <summary>
        /// Verifies that directive trivia will not result in diagnostics.
        /// This is a regression test for #1623
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestWithDirectiveTriviaAsync()
        {
            var testCode = @"
public interface ITestInterface1 { }

public interface ITestInterface2 { }

public class TestClass
{
    public void TestMethod(
        int parameter1,
#if TESTSYMBOL
        ITestInterface1 instance)
#else
        ITestInterface2 instance)
#endif
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1115ParameterMustFollowComma();
        }
    }
}