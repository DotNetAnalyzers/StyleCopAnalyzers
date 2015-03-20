namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1102UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1102QueryClauseMustFollowPreviousClause.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSelectOnSeparateLineWithAdditionalEmptyLine()
        {
            var testCode = @"
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = from m in source
                    where m > 0

                    select m;
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(11, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhereSelectOnSameLine()
        {
            var testCode = @"
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = from m in source
                    where m > 0 select m;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 33);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhereOnTheSameLineAsFrom()
        {
            var testCode = @"
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var query = from m in source where m > 0
                    select m;
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 38);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }


        [Fact]
        public async Task TestComplexQueryWithAdditionalEmptyLine()
        {
            var testCode = @"
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = from m in source

                    let z  = source.Take(10)

                    join f in source2  
                    on m equals f

                    where m > 0 && 
                    m < 1

                    group m by m into g

                    select new {g.Key, Sum = g.Sum()};
    }
}";
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(11, 21),
                    this.CSharpDiagnostic().WithLocation(13, 21),
                    this.CSharpDiagnostic().WithLocation(16, 21),
                    this.CSharpDiagnostic().WithLocation(19, 21),
                    this.CSharpDiagnostic().WithLocation(21, 21),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplexQueryInOneLine()
        {
            var testCode = @"
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = from m in source let z  = source.Take(10) join f in source2 on m equals f where m > 0 && m < 1 group m by m into g select new {g.Key, Sum = g.Sum()};
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQueryInsideQuery()
        {
            var testCode = @"        var query = from m in (from s in Enumerable.Empty<int>()
                where s > 0 select s)

                where m > 0

                orderby m descending 
                select m;";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(2, 29),
                    this.CSharpDiagnostic().WithLocation(4, 17),
                    this.CSharpDiagnostic().WithLocation(6, 17),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task QueryInsideQueryComplex()
        {
            var testCode = @"            var query = from m in (from s in Enumerable.Empty<int>()
                where s > 0 select s)

                where m > 0 && (from zz in Enumerable.Empty<int>()


                    select zz).Max() > m

                orderby m descending
                select (from pp in new[] {m}

                    select pp);";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(2, 29),
                    this.CSharpDiagnostic().WithLocation(4, 17),
                    this.CSharpDiagnostic().WithLocation(7, 21),
                    this.CSharpDiagnostic().WithLocation(9, 17),
                    this.CSharpDiagnostic().WithLocation(12, 21),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1102QueryClauseMustFollowPreviousClause();
        }
    }
}
