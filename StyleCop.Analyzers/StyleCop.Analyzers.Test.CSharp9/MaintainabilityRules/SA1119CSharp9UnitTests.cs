// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis,
        StyleCop.Analyzers.MaintainabilityRules.SA1119CodeFixProvider>;

    public partial class SA1119CSharp9UnitTests : SA1119CSharp8UnitTests
    {
        /// <summary>
        /// Verifies that a type cast followed by a <c>with</c> expression is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3239, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3239")]
        public async Task TestTypeCastFollowedByWithExpressionIsHandledCorrectlyAsync()
        {
            const string testCode = @"
record Foo(int Value)
{
    public object TestMethod(Foo n, int a)
    {
        return (object)(n with { Value = a });
    }
}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type cast followed by a <c>with</c> expression with unnecessary parentheses is handled
        /// correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3239, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3239")]
        public async Task TestTypeCastFollowedByWithExpressionWithUnnecessaryParenthesesIsHandledCorrectlyAsync()
        {
            const string testCode = @"
record Foo(int Value)
{
    public object TestMethod(Foo n, int a)
    {
        return (object){|#0:{|#1:(|}(n with { Value = a }){|#2:)|}|};
    }
}
";

            const string fixedCode = @"
record Foo(int Value)
{
    public object TestMethod(Foo n, int a)
    {
        return (object)(n with { Value = a });
    }
}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DiagnosticId).WithLocation(0),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(1),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(2),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a <c>with</c> expression with unnecessary parentheses is handled correcly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3239, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3239")]
        public async Task TestWithExpressionWithUnnecessaryParenthesesAsync()
        {
            const string testCode = @"
record Foo(int Value)
{
    public void TestMethod(Foo n, int a)
    {
        var test = {|#0:{|#1:(|}n with { Value = a }{|#2:)|}|};
    }
}
";

            const string fixedCode = @"
record Foo(int Value)
{
    public void TestMethod(Foo n, int a)
    {
        var test = n with { Value = a };
    }
}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DiagnosticId).WithLocation(0),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(1),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(2),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(".ToString()")]
        [InlineData("?.ToString()")]
        [InlineData("[0]")]
        [InlineData("?[0]")]
        [WorkItem(3239, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3239")]
        public async Task TestWithExpressionFollowedByDereferenceAsync(string operation)
        {
            string testCode = $@"
record Foo(int Value)
{{
    public object this[int index] => null;

    public object TestMethod(Foo n, int a)
    {{
        return (n with {{ Value = a }}){operation};
    }}
}}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3974, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3974")]
        public async Task TestConditionalExpressionWithTargetTypedNewAsync()
        {
            const string testCode = @"public class TestClass
{
    public object GetValue(bool flag)
    {
        return {|#0:{|#1:(|}flag ? null : new(){|#2:)|}|};
    }
}";

            const string fixedCode = @"public class TestClass
{
    public object GetValue(bool flag)
    {
        return flag ? null : new();
    }
}";

            await new CSharpTest()
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DiagnosticId).WithLocation(0),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(1),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(2),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
