// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1113CommaMustBeOnSameLineAsPreviousParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1113UnitTests
    {
        [Fact]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s
                    , int i)
    {
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar(string s,
                    int i)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 21);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s
                    , int i
                    , int i2)
    {
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar(string s,
                    int i,
                    int i2)
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 21),
                    Diagnostic().WithLocation(5, 21),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s,
                    int i)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty
                                    , string.Empty);
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty,
                                    string.Empty);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 37);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty
                                    , string.Empty
                                    , System.StringComparison.Ordinal);
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty,
                                    string.Empty,
                                    System.StringComparison.Ordinal);
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 37),
                    Diagnostic().WithLocation(7, 37),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty,
                                    string.Empty);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s1, string s2)
    {
    }  
    public void Bar()
    {
        var result = new Foo(string.Empty
                             , string.Empty);
    }
}";
            var fixedCode = @"public class Foo
{
    public Foo(string s1, string s2)
    {
    }  
    public void Bar()
    {
        var result = new Foo(string.Empty,
                             string.Empty);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 30);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s1, string s2, string s3)
    {
    }  
    public void Bar()
    {
        var result = new Foo(string.Empty
                             , string.Empty
                             , string.Empty);
    }
}";
            var fixedCode = @"public class Foo
{
    public Foo(string s1, string s2, string s3)
    {
    }  
    public void Bar()
    {
        var result = new Foo(string.Empty,
                             string.Empty,
                             string.Empty);
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(9, 30),
                    Diagnostic().WithLocation(10, 30),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s1, string s2)
    {
    }    
    public void Bar()
    {
        var result = new Foo(string.Empty,
                             string.Empty);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s
               , int i)
    {
    }
}";
            var fixedCode = @"public class Foo
{
    public Foo(string s,
               int i)
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 16);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s
               , int i
               , int i2)
    {
    }
}";
            var fixedCode = @"public class Foo
{
    public Foo(string s,
               int i,
               int i2)
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 16),
                    Diagnostic().WithLocation(5, 16),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public Foo(string s,
               int i)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s
                    , int i]
    {
        get
        {
            return 1;
        }
    }
}";
            var fixedCode = @"public class Foo
{
    public int this[string s,
                    int i]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 21);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s
                    , int i
                    , int i2]
    {
        get
        {
            return 1;
        }
    }
}";
            var fixedCode = @"public class Foo
{
    public int this[string s,
                    int i,
                    int i2]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 21),
                    Diagnostic().WithLocation(5, 21),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s,
                    int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s, int i]
    {
        get
        {
            return 1;
        }
    }
    public void Bar()
    {
        var i = this[string.Empty
, 5];
    }
}";
            var fixedCode = @"public class Foo
{
    public int this[string s, int i]
    {
        get
        {
            return 1;
        }
    }
    public void Bar()
    {
        var i = this[string.Empty,
5];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(13, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s, int i, int i2]
    {
        get
        {
            return 1;
        }
    }
    public void Bar()
    {
        var i = this[string.Empty
, 5
    ,4];
    }
}";
            var fixedCode = @"public class Foo
{
    public int this[string s, int i, int i2]
    {
        get
        {
            return 1;
        }
    }
    public void Bar()
    {
        var i = this[string.Empty,
5,
    4];
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(13, 1),
                    Diagnostic().WithLocation(14, 5),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public int this[string s,
                    int i]
    {
        get
        {
            return 1;
        }
    }
    public void Bar()
    {
        var i = this[string.Empty, 5];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
         System.Action<string,int> i = 
            delegate(string s
            , int j)
            {

            };
    }
}";
            var fixedCode = @"public class Foo
{
    public void Bar()
    {
         System.Action<string,int> i = 
            delegate(string s,
            int j)
            {

            };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
         System.Action<string,int> i = 
            delegate(string s, int j)
            {

            };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str
, int i);
}";
            var fixedCode = @"public class Foo
{
    delegate void Del(string str,
int i);
}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationWith3ParametersCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str
, int i
, long l);
}";
            var fixedCode = @"public class Foo
{
    delegate void Del(string str,
int i,
long l);
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 1),
                    Diagnostic().WithLocation(5, 1),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str, int i);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionWith3ParametersCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
        public void Bar()
        {
            System.Action<string, int, long> a = (s
                , i
                , l) => { };
        }
}";
            var fixedCode = @"public class Foo
{
        public void Bar()
        {
            System.Action<string, int, long> a = (s,
                i,
                l) => { };
        }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 17),
                    Diagnostic().WithLocation(7, 17),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
            public void Bar()
        {
            System.Action<string, int, long> a = (s, i, l) => { };
        }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionNoParametersAsync()
        {
            var testCode = @"public class Foo
{
            public void Bar()
        {
            System.Action a = () => { };
        }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"using System;
using System.Runtime.InteropServices;
public class SimpleApiOriginal
{
    [DllImport(""user32.dll""
, CharSet=CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
 }";
            var fixedCode = @"using System;
using System.Runtime.InteropServices;
public class SimpleApiOriginal
{
    [DllImport(""user32.dll"",
CharSet=CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
 }";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"using System;
using System.Runtime.InteropServices;
public class SimpleApiOriginal
{
    [DllImport(""user32.dll"", CharSet=CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
 }";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    [Conditional(""A"")
, Conditional(""B"")
, Conditional(""C"")]
        public void Bar()
        {
        }
    }";
            var fixedCode = @"using System.Diagnostics;
public class Foo
{
    [Conditional(""A""),
Conditional(""B""),
Conditional(""C"")]
        public void Bar()
        {
        }
    }";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(5, 1),
                    Diagnostic().WithLocation(6, 1),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    [Conditional(""A""), Conditional(""B""), Conditional(""C"")]
        public void Bar()
        {
        }
    }";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorDeclarationCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a
, Foo b)
    {
        return null;
    }
}";
            var fixedCode = @"
public class Foo
{
    public static Foo operator +(Foo a,
Foo b)
    {
        return null;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(5, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
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
        public async Task TestUnaryOperatorAsync()
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1
, 2];
    }
}";
            var fixedCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1,
2];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1, 2];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneDimensionalArrayDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayOfArraysDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
{
    new int[,] { {1,3}, {5,7} },
    new int[,] { {0,2}, {4,6}, {8,10} },
    new int[,] { {11,22}, {99,88}, {0,9} } 
};
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1,2];
        int i = a[0
, 0];
    }
}";
            var fixedCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1,2];
        int i = a[0,
0];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1,2];
        int i = a[0, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOneDimensionalArrayCallCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1];
        int i = a[0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallCommaPlacedAtTheNextLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo3
{
    public int this[int index1, int index2] => 0;
}

public class Foo4
{
    public void Bar()
    {
        var f = new Foo3();
        var i = f[0
, 0];
    }
}";
            var fixedCode = @"
public class Foo3
{
    public int this[int index1, int index2] => 0;
}

public class Foo4
{
    public void Bar()
    {
        var f = new Foo3();
        var i = f[0,
0];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(13, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo3
{
    public int this[int index1, int index2] => 0;
}

public class Foo4
{
    public void Bar()
    {
        var f = new Foo3();
        var i = f[0, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerWithOneArgumentCallCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"
public class Foo3
{
    public int this[int index1] => 0;
}

public class Foo4
{
    public void Bar()
    {
        var f = new Foo3();
        var i = f[0];
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
    public void TestMethod1(int a, int b
#if true
        , int c)
#endif
    {
    }

    public void TestMethod2()
    {
        TestMethod1(1, 2
#if true
            , 3
#else
            , 4
#endif
            );
    }
}
";

            var fixedCode = @"
public class TestClass
{
    public void TestMethod1(int a, int b
#if true
        , int c)
#endif
    {
    }

    public void TestMethod2()
    {
        TestMethod1(1, 2
#if true
            , 3
#else
            , 4
#endif
            );
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 9),
                Diagnostic().WithLocation(15, 13),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            test.RemainingDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a base constructor call with the comma on the wrong line is handled properly.
        /// This is a regression test for #1654.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyBaseConstructorCallAsync()
        {
            var testCode = @"
abstract class BaseClass
{
    protected BaseClass(int x, int y) { }
}

class ClassName : BaseClass
{
    protected ClassName(int x, int y)
        : base(
            x
            , y)
    {
    }
}
";
            var fixedCode = @"
abstract class BaseClass
{
    protected BaseClass(int x, int y) { }
}

class ClassName : BaseClass
{
    protected ClassName(int x, int y)
        : base(
            x,
            y)
    {
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(12, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a this constructor call with the comma on the wrong line is handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyThisConstructorCallAsync()
        {
            var testCode = @"
class ClassName
{
    private ClassName(int x, int y) { }

    protected ClassName(int x, int y, int z)
        : this(
            x
            , y)
    {
    }
}
";
            var fixedCode = @"
class ClassName
{
    private ClassName(int x, int y) { }

    protected ClassName(int x, int y, int z)
        : this(
            x,
            y)
    {
    }
}
";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
