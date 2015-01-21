using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.ReadabilityRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    [TestClass]
    public class SA1111UnitTests : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        public string DiagnosticId { get; } = SA1111ClosingParenthesisMustBeOnLineOfLastParameter.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 12, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 13, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 12, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }


        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 13, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 15, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallOfTheParameterOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test(System.Collection.Generics.List<int> list)
    {
        var i = list[1
];
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallOfClosureOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void Test()
    {
        System.Collection.Generics.List<int> list = new System.Collection.Generics.List<int>();
        Action a = () => {
        var i = list[1
];
        };      
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 11, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallOfObjectsPropertyReturOneParameterClosingBracketOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Bar
{
    public System.Collections.Generic.List<int> MyLyst {get;set;}
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 12, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 17, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s
);
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationLastParameterOnTheSameLineAsClosingParenthesis()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del(int i, string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationNoParameters()
        {
            var testCode = @"
public class Foo
{
    public delegate void Del();
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributeLastParameterOnThePreviousLineAsClosingParenthesis()
        {
            var testCode = @"
[Conditional(""DEBUG""
), Conditional(""TEST1""
)]
public class Foo
{
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 3, 1)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the last parameter.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 1)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributeLastParameterOnTheSameLineAsClosingParenthesis()
        {
            var testCode = @"
[Conditional(""DEBUG""), Conditional(""TEST1"")]
public class Foo
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributenNoParameters()
        {
            var testCode = @"
[System.Serializable]
public class Foo
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1111ClosingParenthesisMustBeOnLineOfLastParameter();
        }
    }
}