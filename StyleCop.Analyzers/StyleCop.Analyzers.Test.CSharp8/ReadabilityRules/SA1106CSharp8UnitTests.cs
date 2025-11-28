// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public partial class SA1106CSharp8UnitTests : SA1106CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3075, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3075")]
        public async Task TestNoDiagnosticForUsingDeclarationStatementAsync()
        {
            var testCode = @"
using System.IO;
public class Foo
{
    public void Method()
    {
        using var v = new MemoryStream();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3007")]
        public async Task TestAwaitForEachEmptyStatementAsync()
        {
            var testCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestClass
{
    public async Task TestAsync(IAsyncEnumerable<int> values)
    {
        await foreach (var value in values){|#0:;|}
    }
}
";
            var fixedCode = @"
using System.Collections.Generic;
using System.Threading.Tasks;

public class TestClass
{
    public async Task TestAsync(IAsyncEnumerable<int> values)
    {
        await foreach (var value in values)
        {
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
