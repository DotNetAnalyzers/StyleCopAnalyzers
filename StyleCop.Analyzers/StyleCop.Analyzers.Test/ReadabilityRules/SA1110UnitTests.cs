// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.SpacingRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1110UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestMethodDeclarationOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar
                    ()
    {

    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo
        ()
    {
    }
}";
            var fixedCode = @"
public class Foo
{
    public Foo()
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void 
                    Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString
            ();
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingThisOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = this.ToString
            ();
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var s = this.ToString();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingBaseOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = base.ToString
            ();
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var s = base.ToString();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingVariableOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var name = ""qwe"";
        var s = name.ToString
            ();
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var name = ""qwe"";
        var s = name.ToString();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGenericMethodCallAsync()
        {
            var testCode = @"
class Foo
{
    public void Fun<T>(T param)
    {
    }

    public void Bar()
    {
        var f = new Foo();
        f.Fun<string>(null);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public static void Baz()
    {
    }

    public void Bar()
    {
        Foo.Baz
            ();
    }
}";
            var fixedCode = @"
class Foo
{
    public static void Baz()
    {
    }

    public void Bar()
    {
        Foo.Baz();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningBracketInTheNextLineAsClassNameAsync()
        {
            var testCode = @"
class Foo
{
    public static void Baz()
    {
    }

    public void Bar()
    {
        Foo.
Baz();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallWithAnotherStaticCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    public static Foo Baz(int i)
    {
        return new Foo();
    }

    public Foo Bar()
    {
        return Foo.Baz(
            Int32.Parse(""5"")
);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo
        ();
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCreationOfObjectNoOpeningClosingParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Collections.Generic.Dictionary<int, int> cache = new System.Collections.Generic.Dictionary<int, int> { { 3, 3 } };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithQualifiedNameCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    class FooInner
    {
    }
    public void Bar()
    {
        var f = new Foo.FooInner
        ();
    }
}";
            var fixedCode = @"
class Foo
{
    class FooInner
    {
    }
    public void Bar()
    {
        var f = new Foo.FooInner();
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGenericTypeConstructorCallAsync()
        {
            var testCode = @"
class Foo<T>
{
    Foo(System.Func<int> i) 
    {    
    }

    int GetInt(string s)
    {
        return 1;
    }

    public void Bar()
    {
        var f = new Foo<string>(
                () => GetInt(null));
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    internal string this
    [int index]
    {
        get
        {
            return string.Empty;
        }
    }
}";
            var fixedCode = @"
class Foo
{
    internal string this[
    int index]
    {
        get
        {
            return string.Empty;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    internal string this[int index]
    {
        get
        {
            return string.Empty;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public int this[int index]
    {
        get
        {
            return 1;
        }
    }

    public void Bar()
    {
        var s = this
[1];
    }
}";
            var fixedCode = @"
class Foo
{
    public int this[int index]
    {
        get
        {
            return 1;
        }
    }

    public void Bar()
    {
        var s = this[
1];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(15, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCalOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var r = list
[0];
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var r = list[
0];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public int this[int index]
    {
        get
        {
            return 1;
        }
    }

    public void Bar()
    {
        var s = this[1];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var r = list[0];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallOpeningBracketInTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var arr = new int[10];
        var s = arr
[1];
    }
}";
            var fixedCode = @"
class Foo
{
    public void Bar()
    {
        var arr = new int[10];
        var s = arr[
1];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallOpeningBracketInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var arr = new int[10];
        var s = arr[
1];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeOpeningParenthesisOnTheNextLineAsync()
        {
            var testCode = @"
using System.Diagnostics;
public class Foo
{
[Conditional
(""DEBUG""), Conditional
(""TEST1"")]
    public void Baz()
    {
    }
}";
            var fixedCode = @"
using System.Diagnostics;
public class Foo
{
[Conditional(
""DEBUG""), Conditional(
""TEST1"")]
    public void Baz()
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 1),
                    this.CSharpDiagnostic().WithLocation(7, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeOpeningParenthesisOnTheSameLineAsync()
        {
            var testCode = @"
using System.Diagnostics;
public class Foo
{
    [Conditional(""DEBUG""), Conditional(""TEST1"")]
    public void Baz()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationOpeningParenthesisOnTheNextLineAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void MyDel
(int i);
}";
            var fixedCode = @"
public class Foo
{
    public delegate void MyDel(
int i);
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationOpeningParenthesisOnTheSameLineAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void MyDel(int i);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDelegateKeywordOnPreviousLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> del = 
        delegate
(string s, string s2
        )
        {
        };
    }
}";
            var fixedCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> del = 
        delegate(
string s, string s2
        )
        {
        };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDelegateKeywordOnTheSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> del = 
        delegate(string s, string s2)
        {
        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
            System.Action del = 
                delegate()
                {
                };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodNoOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
            System.Action del = 
                delegate
                {
                };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationOpeningBracketOnTheNextLineAsTypeNameAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int
[1,2
];
    }
}";
            var fixedCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int[
1,2
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayOfArraysCreationOpneningBracketOnTheSameLineAsTypeNameAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int[0
    ][];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationTypeOmittedAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var s = new[]
            {
                1
            };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperatorAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +
(Foo a, Foo b)
    {
        return null;
    }
}";
            var fixedCode = @"
public class Foo
{
    public static Foo operator +(
Foo a, Foo b)
    {
        return null;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperatorAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +
(Foo a)
    {
        return null;
    }
}";
            var fixedCode = @"
public class Foo
{
    public static Foo operator +(
Foo a)
    {
        return null;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorDeclarationOpeningParenthesisOnTheSameLineAsOperatorAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return null;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperatorAsync()
        {
            var testCode = @"
public class Foo
{
        public static explicit operator Foo
(int i)
        {
            return null;
        }
}";
            var fixedCode = @"
public class Foo
{
        public static explicit operator Foo(
int i)
        {
            return null;
        }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationOpeningParenthesisOnTheSameLineAsOperatorAsync()
        {
            var testCode = @"
public class Foo
{
        public static explicit operator Foo(int i)
        {
            return null;
        }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestGenericMethodOpeningParenthesisAsync()
        {
            var testCode = @"using System.Collections.Generic;
using System.Linq;

public class Foo
{
        public void TestMethod()
        {
            IEnumerable<object> x = null;
            x
            .Reverse<object>();
        }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDirectiveTriviaAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod1
#if true
        ()
#endif
    {
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void TestMethod1
#if true
        ()
#endif
    {
    }
}
";

            var expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1110OpeningParenthesisMustBeOnDeclarationLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
