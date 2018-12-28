// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;

    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis,
        StyleCop.Analyzers.MaintainabilityRules.SA1119CodeFixProvider>;

    public class SA1119CSharp8UnitTests : SA1119CSharp7UnitTests
    {
        /// <summary>
        /// Verifies that a type cast followed by a switch expression is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3171, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3171")]
        public async Task TestTypeCastFollowedBySwitchExpressionIsHandledCorrectlyAsync()
        {
            const string testCode = @"
public class Foo
{
    public string TestMethod(int n, object a, object b)
    {
        return (string)(n switch { 1 => a, 2 => b });
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type cast followed by a switch expression with unnecessary parenthesis is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3171, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3171")]
        public async Task TestTypeCastFollowedBySwitchExpressionWithUnnecessaryParenthesisIsHandledCorrectlyAsync()
        {
            const string testCode = @"
public class Foo
{
    public string TestMethod(int n, object a, object b)
    {
        return (string)((n switch { 1 => a, 2 => b }));
    }
}
";

            const string fixedCode = @"
public class Foo
{
    public string TestMethod(int n, object a, object b)
    {
        return (string)(n switch { 1 => a, 2 => b });
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 24, 6, 55),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 24),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 54),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a switch expression with unnecessary parenthesis is handled correcly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3171, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3171")]
        public async Task TestSwitchExpressionWithUnnecessaryParenthesisAsync()
        {
            const string testCode = @"
public class Foo
{
    public void TestMethod(int n, object a, object b)
    {
        var test = (n switch { 1 => a, 2 => b });
    }
}
";

            const string fixedCode = @"
public class Foo
{
    public void TestMethod(int n, object a, object b)
    {
        var test = n switch { 1 => a, 2 => b };
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 20, 6, 49),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 48),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(".ToString()")]
        [InlineData("?.ToString()")]
        [InlineData("[0]")]
        [InlineData("?[0]")]
        [WorkItem(3171, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3171")]
        public async Task TestSwitchExpressionFollowedByDereferenceAsync(string operation)
        {
            string testCode = $@"
public class Foo
{{
    public object TestMethod(int n, string a, string b)
    {{
        return (n switch {{ 1 => a, 2 => b }}){operation};
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3171, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3171")]
        public async Task TestSwitchExpressionFollowedByPointerDereferenceAsync()
        {
            string testCode = @"
public class Foo
{
    public unsafe string TestMethod(int n, byte* a, byte* b)
    {
        return (n switch { 1 => a, 2 => b })->ToString();
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStackAllocExpressionInExpressionAsync()
        {
            const string testCode = @"public class TestClass
{
    public unsafe void TestMethod()
    {
        var ptr = stackalloc byte[1];
        var span = (stackalloc byte[1]);
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
