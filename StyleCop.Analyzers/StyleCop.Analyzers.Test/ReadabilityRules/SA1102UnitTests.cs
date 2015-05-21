namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1102UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSelectOnSeparateLineWithAdditionalEmptyLineAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = from m in source
                    where m > 0

                    select m;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(12, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhereSelectOnSameLineAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = from m in source
                    where m > 0 select m;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(10, 33);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhereOnTheSameLineAsFromAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var query = from m in source where m > 0
                    select m;
    }
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 38);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplexQueryWithAdditionalEmptyLineAsync()
        {
            var testCode = @"
using System.Linq;
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
                    this.CSharpDiagnostic().WithLocation(12, 21),
                    this.CSharpDiagnostic().WithLocation(14, 21),
                    this.CSharpDiagnostic().WithLocation(17, 21),
                    this.CSharpDiagnostic().WithLocation(20, 21),
                    this.CSharpDiagnostic().WithLocation(22, 21),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplexQueryInOneLineAsync()
        {
            var testCode = @"
using System.Linq;
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
        public async Task TestQueryInsideQueryAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
                var query = from m in (from s in Enumerable.Empty<int>()
                where s > 0 select s)

                where m > 0

                orderby m descending 
                select m;
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 29),
                    this.CSharpDiagnostic().WithLocation(10, 17),
                    this.CSharpDiagnostic().WithLocation(12, 17),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task QueryInsideQueryComplexAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
                var query = from m in (from s in Enumerable.Empty<int>()
                where s > 0 select s)

                where m > 0 && (from zz in Enumerable.Empty<int>()


                    select zz).Max() > m

                orderby m descending
                select (from pp in new[] {m}

                    select pp);
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(8, 29),
                    this.CSharpDiagnostic().WithLocation(10, 17),
                    this.CSharpDiagnostic().WithLocation(13, 21),
                    this.CSharpDiagnostic().WithLocation(15, 17),
                    this.CSharpDiagnostic().WithLocation(18, 21),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1102QueryClauseMustFollowPreviousClause();
        }
    }
}
