// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly;
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

        /// <summary>
        /// Verifies that spacing before a range expression double dots isn't required.
        /// </summary>
        /// <remarks>
        /// <para>Double dots of range expressions already provide enough spacing for readability so there is no
        /// need to suffix the closing parenthesis with a space.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3064, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3064")]
        public async Task TestBeforeRangeExpressionAsync()
        {
            var testCode = SpecialTypeDefinitions.IndexAndRange + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public string TestMethod()
        {
            string str = ""test"";
            int startLen = 4;
            return str[(startLen - 1) ..];
        }
    }
}
";

            var fixedCode = SpecialTypeDefinitions.IndexAndRange + @"
namespace TestNamespace
{
    using System;
    public class TestClass
    {
        public string TestMethod()
        {
            string str = ""test"";
            int startLen = 4;
            return str[(startLen - 1)..];
        }
    }
}
";
            DiagnosticResult expected = Diagnostic(DescriptorNotFollowed).WithSpan(28, 37, 28, 38);
            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
