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
        new Foo(string.Empty
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
        new Foo(string.Empty,
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
        new Foo(string.Empty,
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
        new Foo(
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
        return 1;
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
        return 1;
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
        public async Task TestIndexerDeclarationWithNoParameterClosingParanthesisOnTheNextLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public void this[
]
    {
        return 1;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerDeclarationWithOneParameterClosingParanthesisOnTheSameLineAsTheLastParameter()
        {
            var testCode = @"
class Foo
{
    public int this[int index]
    {
        return 1;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1111ClosingParenthesisMustBeOnLineOfLastParameter();
        }
    }
}