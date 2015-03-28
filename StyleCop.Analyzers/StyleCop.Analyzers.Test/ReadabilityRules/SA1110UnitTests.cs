﻿namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1110UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1110OpeningParenthesisMustBeOnDeclarationLine.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBracketInTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar
                    ()
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningBracketInTheNextLine()
        {
            var testCode = @"
public class Foo
{
    public Foo
        ()
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

        }

        [Fact]
        public async Task TestMethodDeclarationOpeningBracketInTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void 
                    Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningBracketInTheSameLine()
        {
            var testCode = @"
public class Foo
{
    public Foo()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallUsingThisOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallUsingBaseOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallUsingVariableOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningBracketInTheNextLineAsClassName()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestStaticMethodCallWithAnotherStaticCallOpeningBracketInTheNextLine()
        {
            var testCode = @"
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestCreationOfObjectNoOpeningClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Collections.Generic.Dictionary<int, int> cache = new System.Collections.Generic.Dictionary<int, int> { { 3, 3 } };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithQualifiedNameCallOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationOpeningBracketInTheSameLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisOpeningBracketInTheNextLine()
        {
            var testCode = @"
class Foo
{
    public int this[index]
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(15, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCalOpeningBracketInTheNextLine()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisOpeningBracketInTheSameLine()
        {
            var testCode = @"
class Foo
{
    public int this[index]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOpeningBracketInTheSameLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCallOpeningBracketInTheNextLine()
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCallOpeningBracketInTheSameLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeOpeningParenthesisOnTheNextLine()
        {
            var testCode = @"
public class Foo
{
[Conditional
(""DEBUG""), Conditional
(""TEST1"")]
    public void Baz()
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(5, 1),
                    this.CSharpDiagnostic().WithLocation(6, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeOpeningParenthesisOnTheSameLine()
        {
            var testCode = @"
public class Foo
{
    [Conditional(""DEBUG""), Conditional(""TEST1"")]
    public void Baz()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationOpeningParenthesisOnTheNextLine()
        {
            var testCode = @"
public class Foo
{
    public delegate void MyDel
(int i);
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationOpeningParenthesisOnTheSameLine()
        {
            var testCode = @"
public class Foo
{
    public delegate void MyDel(int i);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodDelegateKeywordOnPreviousLineAsOpeningParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        Action<string,string> del = 
        delegate
(string s, string s2
        )
        {
        };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodDelegateKeywordOnTheSameLineAsOpeningParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        Action<string,string> del = 
        delegate(string s, string s2)
        {
        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodNoParameters()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
            Action del = 
                delegate()
                {
                };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCreationOpeningBracketOnTheNextLineAsTypeName()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayOfArraysCreationOpneningBracketOnTheSameLineAsTypeName()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCreationTypeOmitted()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperator()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestUnaryOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperator()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestOperatorDeclarationOpeningParenthesisOnTheSameLineAsOperator()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return null;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationOpeningParenthesisOnTheNextLineAsOperator()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConversionOperatorDeclarationOpeningParenthesisOnTheSameLineAsOperator()
        {
            var testCode = @"
public class Foo
{
        public static explicit operator Foo(int i)
        {
            return null;
        }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1110OpeningParenthesisMustBeOnDeclarationLine();
        }
    }
}
