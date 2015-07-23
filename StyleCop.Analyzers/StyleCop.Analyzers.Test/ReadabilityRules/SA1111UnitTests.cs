namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1111UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithNoParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithNoParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithOneParameterThatSpansTwoLinesClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithNoParametersClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithTwoParametersClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallWithNoParametersClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithOneParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithThreeParameterClosingParanthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThisIndexerCallOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfTheFieldOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfThePropertyOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfTheParameterOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfClosureOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfMethodReturOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallOfObjectsPropertyReturOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultidimensionalArrayCallOneParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThisIndexerCallThreeParameterClosingBracketOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
        public async Task TestDelegateDeclarationLastParameterOnThePreviousLineAsClosingParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s
);
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationLastParameterOnTheSameLineAsClosingParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodLastParameterOnThePreviousLineAsClosingParenthesisAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodLastParameterOnTheSameLineAsClosingParenthesisAsync()
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
                delegate(
)
                {

                };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeLastParameterOnThePreviousLineAsClosingParenthesisAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeLastParameterOnTheSameLineAsClosingParenthesisAsync()
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
        public async Task TestAttributeNoParametersAsync()
        {
            var testCode = @"
[System.Serializable]
public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListLastParameterOnThePreviousLineAsClosingBracketAsync()
        {
            var testCode = @"
using System.Diagnostics;
[Conditional(""DEBUG""), Conditional(""TEST1"")
]
public class FooAttribute : System.Attribute
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeListLastParameterOnTheSameLineAsClosingBracketAsync()
        {
            var testCode = @"
using System.Diagnostics;
[Conditional(""DEBUG""), Conditional(""TEST1"")]
public class FooAttribute : System.Attribute
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeOneParameterOnTheSameLineAsClosingBracketAsync()
        {
            var testCode = @"
using System.Diagnostics;
[Conditional(""DEBUG"")]
public class FooAttribute: System.Attribute
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Asserts that an attribute with a single parameter does not report if the open and close brackets are on sepsrate lines
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAttributeOneParameterOnPreviousLineAsClosingBracketAsync()
        {
            var testCode = @"
[
System.Serializable
]
public class Foo
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventLastParameterOnThePreviousLineAsClosingParenthesisAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventLastParameterOnTheSameLineAsClosingParenthesisAsync()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventNoParametersAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionLastParameterOnThePreviousLineAsClosingParenthesisAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionInsideWhereLastParameterOnThePreviousLineAsClosingParenthesisAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionLastParameterOnTheSameLineAsClosingParenthesisAsync()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        System.Action a = () => { };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationLastParameterOnThePreviousLineAsClosingBracketAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayOfArraysCreationLastParameterOnThePreviousLineAsClosingBracketAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCreationLastParameterOnTheSameLineAsClosingBracketAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
        var arr = new int[1,2];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1111ClosingParenthesisMustBeOnLineOfLastParameter();
        }
    }
}