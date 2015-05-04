namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1118UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersSecondSpansMoreThanOneLine()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            +4);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersSecondSpansMultipleLinesThirdSpansMultipleLinesButIsAnonymousFunction()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j, System.Action<int> action)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            + 4,
            delegate(int i)
            {
                    
            });
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithThreeParametersSecondSpansMultipleLinesThirdSpansMultipleLinesButIsLambdaExpression()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j, System.Action<int> action)
    {
    }

    public void Bar()
    {
        Fun(1,
            3
            + 4,
            i =>
            {

            });
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallWithTwoParametersFirstIsMultilineSecondIsOneLine()
        {
            var testCode = @"
class Foo
{
    public void Fun(int i, int j)
    {
    }

    public void Bar()
    {
        Fun(1
            +2,
            3);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCallSecondParameterSpansTwoLines()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> d = delegate(int i, int j)
                                    {

                                    };
        d(1,            
          2
          +3);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 11);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodCallSecondParameterIsLambda()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,System.Action<int>> d = delegate(int i, System.Action<int> a)
                                    {

                                    };
        d(1,            
          (k) => 
            {
            });
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaCallSecondParameterSpansTwoLines()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> d = (i, j) => { };
        d(1,            
          2
          +3);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 11);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaCallSecondParameterIsAnonynousMethod()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int, System.Action<int>> d = (i, j) => { };
        d(1,            
          delegate(int param)
        {});
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallSecondParameterSpansTwoLines()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var f = new Foo();
        var result = f[1,
                       3
                       +4];
    }

    public int this[int i, int j]
    {
        get
        {
            return 0;
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 24);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallSecondParameterSpansTwoLines()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] numbers = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 }, { 9, 10 } };
        numbers[1, 1
            +1] = 5;
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 20);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1118ParameterMustNotSpanMultipleLines();
        }
    }
}