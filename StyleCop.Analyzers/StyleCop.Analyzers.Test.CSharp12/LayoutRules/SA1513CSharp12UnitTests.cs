// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.LayoutRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1513ClosingBraceMustBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1513CodeFixProvider>;

    public partial class SA1513CSharp12UnitTests : SA1513CSharp11UnitTests
    {
        [Fact]
        [WorkItem(3720, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3720")]
        public async Task TestObjectInitializerInCollectionExpressionAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo[] TestMethod() =>
    [
        new Foo
        {
        }
    ];
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestJoinIntoClauseSyntaxQueryExpressionAsync()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Linq;

public class JoinIntoClauseSyntaxQueryExpressionTest
{
    public JoinIntoClauseSyntaxQueryExpressionTest()
    {
        List<Foo> fooList =
        [
            new(1, ""First""),
            new(2, ""Second""),
            new(3, ""Third""),
            new(1, ""Fourth"")
        ];

        List<Bar> barList =
        [
            new(1, ""First""),
            new(2, ""Second""),
            new(3, ""Third"")
        ];

        var query =
            from foo in fooList
            join bar in barList
                on new
                {
                    foo.Id,
                    foo.Name
                }
                equals new
                {
                    bar.Id,
                    bar.Name
                }
            into grouping
            from joined in grouping.DefaultIfEmpty()
            select new
            {
                joined.Id,
                BarName = joined.Name,
                FooName = foo.Name
            };
    }

    private class Foo(
        int id,
        string name)
    {
        public int Id { get; } = id;

        public string Name { get; } = name;
    }

    private class Bar(
        int id,
        string name)
    {
        public int Id { get; } = id;

        public string Name { get; } = name;
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
