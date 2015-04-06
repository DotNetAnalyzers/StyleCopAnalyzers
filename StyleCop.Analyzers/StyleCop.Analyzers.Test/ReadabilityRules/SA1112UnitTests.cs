using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.ReadabilityRules;
using TestHelper;
using Xunit;

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    public class SA1112UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithNoParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar(
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithNoParametersClosingParanthesisOnTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodWithParametersClosingParanthesisOnTheNextLine()
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

        [Fact]
        public async Task TestConstructorWithNoParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public Foo(
)
    {

    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithNoParametersClosingParanthesisOnTheSameLine()
        {
            var testCode = @"
class Foo
{
    public Foo()
    {

    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorWithParametersClosingParanthesisOnTheNextLine()
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

        [Fact]
        public async Task TestMethodCallNoParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString(
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallWithParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = this.Equals(new Foo()
);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallNoParametersClosingParanthesisOnTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var s = ToString();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var o = new object(
);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallWithParametersClosingParanthesisOnTheNextLine()
        {
            var testCode = @"
public class CtorWithParams
{
    public CtorWithParams(string s)
    {
    }
}
class Foo
{
    public void Bar()
    {
        var o = new CtorWithParams(string.Empty
);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersClosingParanthesisOnTheSameLine()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var o = new object();
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIndexerWithParametersClosingParanthesisOnTheNextLine()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis();
        }
    }
}