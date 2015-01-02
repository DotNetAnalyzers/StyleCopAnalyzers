using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.ReadabilityRules;
using StyleCop.Analyzers.SpacingRules;
using TestHelper;

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    [TestClass]
    public class SA1110UnitTests : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        public string DiagnosticId { get; } = SA1110OpeningParenthesisMustBeOnDeclarationLine.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationOpeningBracketInTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar
                    ()
    {

    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
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
        public async Task TestConstructorDeclarationOpeningBracketInTheNextLine()
        {
            var testCode = @"
public class Foo
{
    public Foo
        ()
    {
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 5, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationOpeningBracketInTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void 
                    Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationOpeningBracketInTheSameLine()
        {
            var testCode = @"
public class Foo
{
    public Foo()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallUsingThisOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallUsingBaseOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallUsingVariableOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestStaticMethodCallOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 11, 13)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorCallOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorWithQualifiedNameCallOpeningBracketInTheNextLine()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The opening parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the method or indexer name.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 9)
                            }
                    }
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1110OpeningParenthesisMustBeOnDeclarationLine();
        }
    }
}