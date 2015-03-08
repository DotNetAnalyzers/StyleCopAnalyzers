namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    [TestClass]
    public class SA1114UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1114ParameterListMustFollowDeclaration.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar(

string s)
    {

    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodDeclarationParametersListOnSameLineAsOpeningParenthesis()
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

        [TestMethod]
        public async Task TestMethodDeclarationNoParameters()
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

        [TestMethod]
        public async Task TestMethodCallParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallParametersListOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMethodCallNoParameters()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public Foo(

string s)
    {

    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public Foo(
string s)
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorDeclarationParametersListOnSameLineAsOpeningParenthesis()
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

        [TestMethod]
        public async Task TestConstructorDeclarationNoParameters()
        {
            var testCode = @"
class Foo
{
    public Foo () 
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorCallParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 12, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorallParametersListOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorCallParametersListOnSameLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestConstructorCallNoParameters()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerDeclarationParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerDeclarationParametersListOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerDeclarationParametersListOnSameLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayDeclarationSizes2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestMultidimensionalArrayDeclarationSizes2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 14, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayDeclarationSizesOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayDeclarationSizesOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallParameters2LinesAfterOpeningBracket()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collection.Generics.List<int> list = new System.Collection.Generics.List<int>();
        var i = list[

1];
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallParametersOnNextLineAsOpeningBracket()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collection.Generics.List<int> list = new System.Collection.Generics.List<int>();
        var i = list[
1];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestIndexerCallParametersOnSameLineAsOpeningBracket()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collection.Generics.List<int> list = new System.Collection.Generics.List<int>();
        var i = list[1];
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayCallParameters2LinesAfterOpeningBracket()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 14, 1)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 16, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayCallParametersOnNextLineAsOpeningBracket()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestArrayCallParametersOnSameLineAsOpeningBracket()
        {
            var testCode = @"
class Foo
{
    public void Bar()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributeParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [Conditional(

""DEBUG"")]
    public void Bar()
    {
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributeParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [Conditional(
""DEBUG"")]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAtributeParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [Conditional(""DEBUG"")]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributeNoParameters()
        {
            var testCode = @"
[Serializable]
class Foo
{

}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributesListParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [

Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAttributesListParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [
Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAtributesListParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    [Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(

string s);
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(
string s);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(string s);
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestDelegateDeclarationNoParameters()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar();
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAnonymousMethodDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = delegate(

int z, int j)
        {

        };
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAnonymousMethodDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = delegate(
int z, int j)
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAnonymousMethodDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = delegate(int z, int j)
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestAnonymousMethodDeclarationNoParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action c = delegate()
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLambdaExpressionDeclarationParametersList2LinesAfterOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = (

z,j) =>
        {

        };
    }
}";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLambdaExpressionDeclarationParametersListOnNextLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = (
z,j) =>
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLambdaExpressionDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action<int,int> c = (z,j) =>
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestLambdaExpressionDeclarationNoParameters()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        Action c = () => 
        {

        };
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCastOperatorDeclarationParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCastOperatorDeclarationDeclarationParametersListOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCastOperatorDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(int i)
    {
        return new Foo();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestUnaryOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesis()
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

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = "Parameter list must follow declaration.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 1)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOperatorOverloadDeclarationParametersListOnNextLineAsOpeningParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestOperatorOverloadDeclarationParametersListOnSameLineAsOpeningParenthesis()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1114ParameterListMustFollowDeclaration();
        }
    }
}