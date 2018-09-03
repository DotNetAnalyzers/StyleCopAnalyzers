// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1114ParameterListMustFollowDeclaration>;

    public class SA1114UnitTests
    {
        [Fact]
        public async Task TestMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(

1);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(
1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var i = 1.ToString(
                
            );
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public Foo () 
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(

1,2);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(12, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(
1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
       var f = new Foo(

);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[

int i]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[
int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[

4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultidimensionalArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
var a = new int[

1][

,]
            {
                new int[

1, 1]
                {
                    {1}
                }
            };
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 1),
                    Diagnostic().WithLocation(14, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[
4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[

1];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[
1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[

0][

1, 0];
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(14, 1),
                    Diagnostic().WithLocation(16, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[
0][
1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[0][1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(

""DEBUG"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(
""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributeParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeNoParametersAsync()
        {
            var testCode = @"
[System.Serializable]
class Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [

Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [
Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributesListParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(

string s);
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(
string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar();
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(

int z, int j)
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(
int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate()
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (

z,j) =>
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (
z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = () => 
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(

int i)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(
int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a, Foo b)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(
Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestObjectCreationNoArgumentListAsync()
        {
            var testCode = @"
public class Foo
{
    public static void Bar()
    {
        var list = new System.Collections.Generic.List<int> { 42 };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPragmaDirectivesAsync()
        {
            var testCode = @"using System;

public class SomeOtherClass
{
    private void SomeMethod()
    {
        this.SomeOtherMethod(
#pragma warning disable 618
                this.SomeObsoleteMethod());
#pragma warning restore 618
    }

    [Obsolete(""Don't use me!"")]
    private int SomeObsoleteMethod()
    {
        return 0;
    }

    private void SomeOtherMethod(int someParameter)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that directive trivia will not result in diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1623")]
        public async Task TestWithDirectiveTriviaAsync()
        {
            var testCode = @"
public interface ITestInterface1 { }

public interface ITestInterface2 { }

public class TestClass
{
    public void TestMethod(
#if TESTSYMBOL
        ITestInterface1 instance)
#else
        ITestInterface2 instance)
#endif
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
