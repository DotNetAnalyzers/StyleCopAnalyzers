// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1009CSharp8UnitTests : SA1009CSharp7UnitTests
    {
        [Fact]
        [WorkItem(2991, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2991")]
        public async Task TestFollowedBySuppressionOperatorAsync()
        {
            const string testCode = @"
public class Foo
{
    public void TestMethod<T>()
    {
        if (default(T[|)|] ! is null)
        {
        }
    }
}";
            const string fixedCode = @"
public class Foo
{
    public void TestMethod<T>()
    {
        if (default(T)! is null)
        {
        }
    }
}";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2968")]
        public async Task TestExpressionBodyEndsWithSuppressionAsync()
        {
            const string testCode = @"using System;
#nullable enable
public class Foo
{
    public T? TestMethod<T>() where T : class => throw null;

    public IDisposable Service => this.TestMethod<IDisposable>([|)|] !;
}";
            const string fixedCode = @"using System;
#nullable enable
public class Foo
{
    public T? TestMethod<T>() where T : class => throw null;

    public IDisposable Service => this.TestMethod<IDisposable>()!;
}";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2968, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2968")]
        public async Task TestBlockBodyEndsWithSuppressionAsync()
        {
            const string testCode = @"using System;
#nullable enable
public class Foo
{
    public T? TestMethod<T>() where T : class => throw null;

    public IDisposable Service()
    {
        return this.TestMethod<IDisposable>([|)|] !;
    }
}";
            const string fixedCode = @"using System;
#nullable enable
public class Foo
{
    public T? TestMethod<T>() where T : class => throw null;

    public IDisposable Service()
    {
        return this.TestMethod<IDisposable>()!;
    }
}";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
