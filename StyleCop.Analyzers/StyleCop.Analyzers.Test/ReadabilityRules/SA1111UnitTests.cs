namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1111UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1111ClosingParenthesisMustBeOnLineOfLastParameter.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s,
                    int i,
                    object o
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithNoParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(
)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s,
                    int i,
                    object o
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithNoParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(
)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }

    public void Baz()
    {
        Bar(string.Empty
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithOneParameterThatSpansTwoLinesClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }

    public void Baz()
    {
        Bar(string
.Empty);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s, int i)
    {

    }

    public void Baz()
    {
        Bar(string.Empty,
            5
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s, int i)
    {

    }

    public void Baz()
    {
        Bar(string.Empty,
            5);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithNoParametersClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {

    }

    public void Baz()
    {
        Bar(
);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }

    public void Baz()
    {
        var f = new Foo(string.Empty
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s, int i)
    {

    }

    public void Baz()
    {
        var f = new Foo(string.Empty,
            5
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(13, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s, int i)
    {

    }

    public void Baz()
    {
        var f = new Foo(string.Empty,
            5);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithNoParametersClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo()
    {

    }

    public void Baz()
    {
        var f = new Foo(
);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public int this[int index
]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public int this[int index,
                    int index2,
                    int index3
]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
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
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestThisIndexerCallOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
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

    public void Test()
    {
        var i = this[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(15, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test()
    {
        var list = new System.Collections.Generic.List<int>();
        var i = list[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfTheFieldOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    private System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();

    public void Test()
    {
        var i = list[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfThePropertyOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    private System.Collections.Generic.List<int> List2 {get;set;}

    public void Test()
    {
        var i = List2[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfTheParameterOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test(System.Collections.Generic.List<int> list)
    {
        var i = list[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfClosureOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        System.Action a = () => {
        var i = list[1
];
        };      
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfMethodReturOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public System.Collections.Generic.List<int> Get()
    {
        return new System.Collections.Generic.List<int>();
    }
    public void Test()
    {
        var i = Get()[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerCallOfObjectsPropertyReturOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Bar
{
    public System.Collections.Generic.List<int> MyList {get;set;}
}
class Foo
{
    public void Test()
    {
        var bar = new Bar();
        var i = bar.MyList[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCallOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test()
    {
        var arr = new int[10];
        var i = arr[1
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMultidimensionalArrayCallOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test()
    {
        var arr = new int[10,15];
        var i = arr[1,3
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestThisIndexerCallThreeParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public int this[int index, int index2, int index3]
    {
        get
        {
            return 1;
        }
    }

    public void Test()
    {
        var i = this[1,    
                  2,
                  3
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(17, 1);

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
        public async Task TestDelegateDeclarationLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s
);
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationLastParameterOnTheSameLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestDelegateDeclarationNoParameters()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> del = 
        delegate(string s, string s2
        )
        {

        };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAnonymousMethodLastParameterOnTheSameLineAsClosingParenthesis()
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
            System.Action del = 
                delegate(
)
                {

                };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
using System.Diagnostics;
public class Foo
{
[Conditional(""DEBUG""
), Conditional(""TEST1""
)]
    public void Baz()
    {
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 1),
                    this.CSharpDiagnostic().WithLocation(7, 1)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeLastParameterOnTheSameLineAsClosingParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeNoParameters()
        {
            var testCode = @"
[System.Serializable]
public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeListLastParameterOnThePreviousLineAsClosingBracket()
        {
            var testCode = @"
using System.Diagnostics;
[Conditional(""DEBUG""), Conditional(""TEST1"")
]
public class FooAttribute : System.Attribute
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeListLastParameterOnTheSameLineAsClosingBracket()
        {
            var testCode = @"
using System.Diagnostics;
[Conditional(""DEBUG""), Conditional(""TEST1"")]
public class FooAttribute : System.Attribute
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAttributeOneParameterOnTheSameLineAsClosingBracket()
        {
            var testCode = @"
[System.Serializable]
public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    private event System.Action<int, int> MyEvent;

    public void Bar()
    {
        MyEvent(1,2
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventLastParameterOnTheSameLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    private event System.Action<int, int> MyEvent;

    public void Bar()
    {
        MyEvent(1,2);
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestEventNoParameters()
        {
            var testCode = @"
public class Foo
{
    private event System.Action MyEvent;

    public void Bar()
    {
        MyEvent();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> act = (string s, string s2
) =>
        {
                    
        };
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionInsideWhereLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
namespace Test
{
    using System.Linq;

    public class Foo
    {
        public void Bar()
        {
            var arr = new int[0];
            var r = arr.Where((int a
                ) => a > 0);
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 17);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionLastParameterOnTheSameLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action<string,string> act = (string s, string s2) =>
        {
                    
        };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaExpressionNoParameters()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action a = () => { };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCreationLastParameterOnThePreviousLineAsClosingBracket()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int[1,2
];
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayOfArraysCreationLastParameterOnThePreviousLineAsClosingBracket()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayCreationLastParameterOnTheSameLineAsClosingBracket()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int[1,2];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1111ClosingParenthesisMustBeOnLineOfLastParameter();
        }
    }
}