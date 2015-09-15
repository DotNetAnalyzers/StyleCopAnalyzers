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

    public class SA1113UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(4, 21),
                    this.CSharpDiagnostic().WithLocation(5, 21)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 37);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(6, 37),
                    this.CSharpDiagnostic().WithLocation(7, 37)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 30);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(9, 30),
                    this.CSharpDiagnostic().WithLocation(10, 30)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(4, 16),
                    this.CSharpDiagnostic().WithLocation(5, 16)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(4, 21),
                    this.CSharpDiagnostic().WithLocation(5, 21)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(13, 1),
                    this.CSharpDiagnostic().WithLocation(14, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(4, 1),
                    this.CSharpDiagnostic().WithLocation(5, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationCommaPlacedAtTheSameLineAsThePreviousParameterAsync()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str, int i);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(6, 17),
                    this.CSharpDiagnostic().WithLocation(7, 17)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic().WithLocation(5, 1),
                    this.CSharpDiagnostic().WithLocation(6, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(6, 9),
                this.CSharpDiagnostic().WithLocation(15, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1113CommaMustBeOnSameLineAsPreviousParameter();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
