// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1102UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(13, 13);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(13, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(15, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(18, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(21, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(23, 13)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(10, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(12, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(14, 13),
                    this.CSharpDiagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(16, 13)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA110xQueryClauses();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1102CodeFixProvider();
        }
    }
}
