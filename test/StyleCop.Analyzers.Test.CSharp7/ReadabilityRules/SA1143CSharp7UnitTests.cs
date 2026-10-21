// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.Test.ReadabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.ReadabilityRules.SA1143ConditionalExpressionsMustBeFormattedConsistently,
    StyleCop.Analyzers.ReadabilityRules.SA1143CodeFixProvider>;

/// <summary>
/// Tests formatting of conditional expressions with C# 7 syntax.
/// </summary>
public partial class SA1143CSharp7UnitTests : SA1143UnitTests
{
    /// <summary>
    /// Verifies that the expanded fix indents conditional operators beyond a multiline condition.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task TestExpandedFixWithMultilineConditionAsync()
    {
        const string source = """
            public class Base
            {
                public bool Foo(object invocation)
                    => invocation is string
                        && invocation is "hi"
                        {|#0:?|} true
                        : false;
            }
            """;
        const string fixedSource = """
            public class Base
            {
                public bool Foo(object invocation)
                    => invocation is string && invocation is "hi"
                        ? true
                        : false;
            }
            """;
        var test = new CSharpTest
        {
            TestCode = source,
            FixedCode = fixedSource,
            CodeActionEquivalenceKey = "Expanded",
        };
        test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(0));
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Verifies that a multiline condition is allowed when the conditional operators are indented beyond its last line.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    [WorkItem(4367, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4367")]
    public async Task TestMultilineConditionAsync()
    {
        const string source = """
            public class Base
            {
                public bool Foo(object invocation)
                    => invocation is string
                        && invocation is "hi"
                            ? true
                            : false;
            }
            """;
        await VerifyCSharpDiagnosticAsync(source, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
    }
}
