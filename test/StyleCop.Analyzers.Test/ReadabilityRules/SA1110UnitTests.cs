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
        StyleCop.Analyzers.ReadabilityRules.SA1110OpeningParenthesisMustBeOnDeclarationLine,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1110UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestMethodDeclarationOpeningParenthesisInTheNextLineAsync(string lineEnding)
        {
            var testCode = @"
class Foo
{
    public void Bar
                    {|#0:(|})
    {

    }
}".ReplaceLineEndings(lineEnding);
            var fixedCode = @"
class Foo
{
    public void Bar()
    {

    }
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationOpeningParenthesisInTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void 
                    Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationOpeningParenthesisInTheSameLineAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingThisOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingBaseOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallUsingVariableOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(11, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallOpeningParenthesisInTheNextLineAsClassNameAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStaticMethodCallWithAnotherStaticCallOpeningParenthesisInTheNextLineAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorWithQualifiedNameCallOpeningParenthesisInTheNextLineAsync()
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

            DiagnosticResult expected = Diagnostic().WithLocation(10, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 5);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(15, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("Conditional")]
        [InlineData("System.Diagnostics.Conditional")]
        public async Task TestAttributeOpeningParenthesisOnTheNextLineAsync(string attributeName)
        {
            var testCode = $@"
using System.Diagnostics;
public class Foo
{{
[{attributeName}
(""DEBUG""), {attributeName}
(""TEST1"")]
    public void Baz()
    {{
    }}
}}";
            var fixedCode = $@"
using System.Diagnostics;
public class Foo
{{
[{attributeName}(
""DEBUG""), {attributeName}(
""TEST1"")]
    public void Baz()
    {{
    }}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 1),
                    Diagnostic().WithLocation(7, 1),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("Conditional")]
        [InlineData("System.Diagnostics.Conditional")]
        public async Task TestAttributeOpeningParenthesisOnTheSameLineAsync(string attributeName)
        {
            var testCode = $@"
using System.Diagnostics;
public class Foo
{{
    [{attributeName}(""DEBUG""), {attributeName}(""TEST1"")]
    public void Baz()
    {{

    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationOpeningParenthesisOnTheSameLineAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void MyDel(int i);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(6, 9);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedCode,
                RemainingDiagnostics = { expected },
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConversionOperatorWithMissingTypeAsync()
        {
            var testCode = @"
public class TestClass
{
    public static implicit operator
        (TestClass value)
    {
        return 0;
    }
}
";

            var expected = this.GetExpectedResultTestConversionOperatorWithMissingType();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConversionOperatorWithMissingParameterListAsync()
        {
            var testCode = @"
public class TestClass
{
    public static implicit operator TestClass;
}
";

            var expected = this.GetExpectedResultTestConversionOperatorWithMissingParameterList();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorWithMissingOperatorTokenAsync()
        {
            var testCode = @"
public class TestClass
{
    public static TestClass operator {|CS1019:(|}{|CS1003:TestClass|} value)
    {
        return value;
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorWithMissingParameterListAsync()
        {
            var testCode = @"
public class TestClass
{
    public static TestClass operator +;
}
";

            var expected = this.GetExpectedResultTestOperatorWithMissingParameterList();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithMissingNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public delegate void{|CS1001:|}
        (int value);
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateWithMissingParameterListAsync()
        {
            var testCode = @"
public class TestClass
{
    public delegate void TestDelegate{|CS1003:|}{|CS1026:|}
        ;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithMissingParameterListAsync()
        {
            var testCode = @"
public class TestClass
{
    public int this;
}
";

            var expected = this.GetExpectedResultTestIndexerWithMissingParameterList();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithMissingOpenBracketAsync()
        {
            var testCode = @"
public class TestClass
{
    public int this{|CS1003:|}
        int value] { get { return value; } }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithMissingNameAsync()
        {
            var testCode = @"
public interface ITest
{
    void TestMethod();
}

public class TestClass : {|CS0535:ITest|}
{
    void ITest.{|CS1001:|}
{|CS0539:|}        ()
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodWithMissingParameterListAsync()
        {
            var testCode = @"
public class TestClass
{
    public void {|CS0501:TestMethod|}<T>{|CS1003:{|CS1026:;|}|}
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestObjectCreationWithMissingTypeAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        var value = new;
    }
}
";

            var expected = this.GetExpectedResultTestObjectCreationWithMissingType();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvocationWithMissingNameAsync()
        {
            var testCode = @"
public class TestClass
{
    public void TestMethod()
    {
        this.{|CS1001:|}
            ();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeWithMissingNameAsync()
        {
            var testCode = @"
[System.{|CS1001:|}
    ()]
public class TestClass
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingType()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1031").WithSpan(4, 36, 4, 36),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestConversionOperatorWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 37, 4, 46).WithArguments("TestClass.implicit operator TestClass()"),
                DiagnosticResult.CompilerError("CS1019").WithSpan(4, 46, 4, 46),
                DiagnosticResult.CompilerError("CS1001").WithSpan(4, 46, 4, 47),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 46, 4, 47).WithArguments("(", ";"),
                DiagnosticResult.CompilerError("CS1002").WithSpan(4, 47, 4, 47),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 47, 4, 47),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestOperatorWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0501").WithSpan(4, 38, 4, 39).WithArguments("TestClass.operator +()"),
                DiagnosticResult.CompilerError("CS1534").WithSpan(4, 38, 4, 39).WithArguments("+"),
                DiagnosticResult.CompilerError("CS1001").WithSpan(4, 39, 4, 40),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 39, 4, 40).WithArguments("(", ";"),
                DiagnosticResult.CompilerError("CS1002").WithSpan(4, 40, 4, 40),
                DiagnosticResult.CompilerError("CS1026").WithSpan(4, 40, 4, 40),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestIndexerWithMissingParameterList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0548").WithSpan(4, 16, 4, 20).WithArguments("TestClass.this"),
                DiagnosticResult.CompilerError("CS1001").WithSpan(4, 20, 4, 21),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 20, 4, 21).WithArguments("[", ";"),
                DiagnosticResult.CompilerError("CS1003").WithSpan(4, 21, 4, 21).WithArguments("]", "}"),
                DiagnosticResult.CompilerError("CS1514").WithSpan(4, 21, 4, 21),
                DiagnosticResult.CompilerError("CS1551").WithSpan(5, 1, 5, 1),
                DiagnosticResult.CompilerError("CS1513").WithSpan(5, 2, 5, 2),
            };
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestObjectCreationWithMissingType()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS1031").WithSpan(6, 24, 6, 25),
                DiagnosticResult.CompilerError("CS1526").WithSpan(6, 24, 6, 25),
            };
        }
    }
}
