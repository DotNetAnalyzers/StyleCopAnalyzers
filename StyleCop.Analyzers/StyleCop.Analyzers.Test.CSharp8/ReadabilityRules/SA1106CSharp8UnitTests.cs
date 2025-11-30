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
        [WorkItem(3004, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3004")]
        public async Task TestUsingDeclarationWithFollowingStatementAsync()
        {
            var testCode = @"
using System.IO;
public class Foo
{
    public int Method()
    {
        using var stream = new MemoryStream();
        return stream.ReadByte();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3004, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3004")]
        public async Task TestAwaitUsingDeclarationAsync()
        {
            var testCode = @"
using System;
using System.Threading.Tasks;

public class Foo
{
    public async Task MethodAsync()
    {
        await using var resource = new AsyncDisposable();
        await Task.Yield();
    }

    private sealed class AsyncDisposable : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3004, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3004")]
        public async Task TestMultipleUsingDeclarationsAsync()
        {
            var testCode = @"
using System.IO;
public class Foo
{
    public void Method()
    {
        using MemoryStream stream1 = new MemoryStream(), stream2 = new MemoryStream();
        _ = stream1.ReadByte() + stream2.ReadByte();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
