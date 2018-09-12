// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA110xQueryClauses,
        StyleCop.Analyzers.ReadabilityRules.SA1102CodeFixProvider>;

    public class SA1102UnitTests
    {
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

        var query = 
            from m in source
            where m > 0

            select m;
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = 
            from m in source
            where m > 0
            select m;
    }
}";

            DiagnosticResult expected = Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(13, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

        var query = 
            from m in source

            let z  = source.Take(10)

            join f in source2  
            on m equals f

            where m > 0 && 
            m < 1

            group m by m into g

            select new {g.Key, Sum = g.Sum()};
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = 
            from m in source
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
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(13, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(15, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(18, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(21, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(23, 13),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        var query = from m in
            (from s in Enumerable.Empty<int>()

            where s > 0

            select s)

            where m > 0

            orderby m descending 
            select m;
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var query = from m in
            (from s in Enumerable.Empty<int>()
            where s > 0
            select s)
            where m > 0
            orderby m descending 
            select m;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(10, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(12, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(14, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(16, 13),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
