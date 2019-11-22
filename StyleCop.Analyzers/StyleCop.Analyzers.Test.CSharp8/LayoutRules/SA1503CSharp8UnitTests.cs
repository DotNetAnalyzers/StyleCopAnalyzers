// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1503BracesMustNotBeOmitted,
        StyleCop.Analyzers.LayoutRules.SA1503CodeFixProvider>;

    public class SA1503CSharp8UnitTests : SA1503CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3074, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3074")]
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
