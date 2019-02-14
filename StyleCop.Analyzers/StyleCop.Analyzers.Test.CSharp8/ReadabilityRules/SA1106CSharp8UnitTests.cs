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
        Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public class SA1106CSharp8UnitTests : SA1106CSharp7UnitTests
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
    }
}
