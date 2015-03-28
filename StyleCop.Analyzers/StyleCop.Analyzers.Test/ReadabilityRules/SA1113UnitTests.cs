namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1113UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1113CommaMustBeOnSameLineAsPreviousParameter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s
                    , int i)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s
                    , int i
                    , int i2)
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 21),
                    this.CSharpDiagnostic().WithLocation(5, 21)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s,
                    int i)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty
                                    , string.Empty);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 37);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty
                                    , string.Empty
                                    , StringComparison.Ordinal);
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 37),
                    this.CSharpDiagnostic().WithLocation(7, 37)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var result = string.Compare(string.Empty,
                                    string.Empty);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 30);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
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

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(9, 30),
                    this.CSharpDiagnostic().WithLocation(10, 30)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            var testCode = @"public class Foo
{
    public Foo(string s
               , int i)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 16);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
        {
            var testCode = @"public class Foo
{
    public Foo(string s
               , int i
               , int i2)
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 16),
                    this.CSharpDiagnostic().WithLocation(5, 16)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
        {
            var testCode = @"public class Foo
{
    public Foo(string s,
               int i)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
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

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 21),
                    this.CSharpDiagnostic().WithLocation(5, 21)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
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
, 5);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameter()
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
    ,4);
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(13, 1),
                    this.CSharpDiagnostic().WithLocation(14, 5)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallUsingThisWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
         Action<string,int> i = 
            delegate(string s
            , int j)
            {

            };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
         Action<string,int> i = 
            delegate(string s, int j)
            {

            };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str
, int i);
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationWith3ParametersCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str
, int i
, long l);
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 1),
                    this.CSharpDiagnostic().WithLocation(5, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    delegate void Del(string str, int i);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionWith3ParametersCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
        public void Bar()
        {
            Action<string, int, long> a = (s
                , i
                , l) => { };
        }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 17),
                    this.CSharpDiagnostic().WithLocation(7, 17)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
            public void Bar()
        {
            Action<string, int, long> a = (s, i, l) => { };
        }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionNoParameters()
        {
            var testCode = @"public class Foo
{
            public void Bar()
        {
            Action a = () => { };
        }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class SimpleApiOriginal
{
    [DllImport(""user32.dll""
, CharSet=CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
 }";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeListCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"public class SimpleApiOriginal
{
    [DllImport(""user32.dll"", CharSet=CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
 }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeListCommaPlacedAtTheNextLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    [Conditional(""A"")
, Conditional(""B"")
, Conditional(""C"")]
        public void Bar()
        {
        }
    }";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(4, 1),
                    this.CSharpDiagnostic().WithLocation(5, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"public class Foo
{
    [Conditional(""A""), Conditional(""B""), Conditional(""C"")]
        public void Bar()
        {
        }
    }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOperatorDeclarationCommaPlacedAtTheNextLineAsThePreviousParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestOperatorDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
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
        public async Task TestUnaryOperator()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayDeclarationCommaPlacedAtTheNextLineAsThePreviousParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1, 2];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOneDimensionalArrayDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var a = new int[1];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayOfArraysDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCallCommaPlacedAtTheNextLineAsThePreviousParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCallCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestOneDimensionalArrayCallCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallCommaPlacedAtTheNextLineAsThePreviousParameter()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerWithOneArgumentCallCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1113CommaMustBeOnSameLineAsPreviousParameter();
        }
    }
}