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
    public class SA1113UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1113CommaMustBeOnSameLineAsPreviousParameter.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s
                    , int i)
    {
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 21)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
        {
            var testCode = @"public class Foo
{
    public void Bar(string s,
                    int i)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 6, 37)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs",6, 37)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 7, 37)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 9, 30)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs",9, 30)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 10, 30)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameter()
        {
            var testCode = @"public class Foo
{
    public Foo(string s
               , int i)
    {
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 16)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 16)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 16)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameter()
        {
            var testCode = @"public class Foo
{
    public Foo(string s,
               int i)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 4, 21)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 21)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 13, 1)
                        }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 14, 5)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationCommaPlacedAtTheNextLineAsThePreviousParameter()
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

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "Comma must be on same line as previous parameter.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 7, 13)
                        }
                },
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationCommaPlacedAtTheSameLineAsThePreviousParameter()
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1113CommaMustBeOnSameLineAsPreviousParameter();
        }
    }
}