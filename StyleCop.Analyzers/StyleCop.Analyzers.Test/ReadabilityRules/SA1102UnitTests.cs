namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;

    public class SA1102UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1102QueryClauseMustFollowPreviousClause.DiagnosticId;

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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
            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 11, 21)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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
            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 33)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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
            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 38)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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
            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 11, 21)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 13, 21)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 16, 21)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 19, 21)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 21, 21)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestQueryInsideQuery()
        {
            var testCode = @"        var query = from m in (from s in Enumerable.Empty<int>()
                where s > 0 select s)

                where m > 0

                orderby m descending 
                select m;";

            var expected = new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 29)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 2, 29)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 4, 17)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Query clause must follow previous clause.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 6, 17)
                            }
                    }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1102QueryClauseMustFollowPreviousClause();
        }
    }
}