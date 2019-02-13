// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements,
        Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public class SA1106UnitTests
    {
        [Theory]
        [InlineData("if (true)")]
        [InlineData("if (true) { } else")]
        [InlineData("for (int i = 0; i < 10; i++)")]
        [InlineData("while (true)")]
        public async Task TestEmptyStatementAsBlockAsync(string controlFlowConstruct)
        {
            var testCode = $@"
class TestClass
{{
    public void TestMethod()
    {{
        {controlFlowConstruct}
            ;
    }}
}}";
            var fixedCode = $@"
class TestClass
{{
    public void TestMethod()
    {{
        {controlFlowConstruct}
        {{
        }}
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementAsBlockInDoWhileAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        do
            ;
        while (false);
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        do
        {
        }
        while (false);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementWithinBlockAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
        {
            var temp = i;
            ;
        }
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (int i = 0; i < 10; i++)
        {
            var temp = i;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 13);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementInForStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        for (;;)
        {
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        ;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementFollowedByEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        ;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabeledEmptyStatementFollowedByNonEmptyStatementAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        ;
        int x = 3;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    label:
        int x = 3;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConsecutiveLabelsAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
    label1:
    label2:
        ;
        int x = 3;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
    label1:
    label2:
        int x = 3;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchCasesAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        switch (default(int))
        {
        case 0:
            ;
            break;

        case 1:
        case 2:
            ;
            break;

        default:
            ;
            break;
        }
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        switch (default(int))
        {
        case 0:
            break;

        case 1:
        case 2:
            break;

        default:
            break;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(9, 13),
                Diagnostic().WithLocation(14, 13),
                Diagnostic().WithLocation(18, 13),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("class Foo { }")]
        [InlineData("struct Foo { }")]
        [InlineData("interface IFoo { }")]
        [InlineData("enum Foo { }")]
        [InlineData("namespace Foo { }")]
        public async Task TestMemberAsync(string declaration)
        {
            var testCode = declaration + ";";
            var fixedCode = declaration;

            DiagnosticResult expected = Diagnostic().WithLocation(1, declaration.Length + 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will remove all unnecessary whitespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1556, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1556")]
        public async Task VerifyCodeFixWillRemoveUnnecessaryWhitespaceAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod1()
    {
        throw new System.NotImplementedException(); ;
    }

    public void TestMethod2()
    {
        throw new System.NotImplementedException(); /* c1 */ ;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod1()
    {
        throw new System.NotImplementedException();
    }

    public void TestMethod2()
    {
        throw new System.NotImplementedException(); /* c1 */
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 53),
                Diagnostic().WithLocation(11, 62),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will not remove relevant trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCodeFixWillNotRemoveTriviaAsync()
        {
            var testCode = @"
class TestClass
{
    public void TestMethod()
    {
        /* do nothing */ ;
    }
}";
            var fixedCode = @"
class TestClass
{
    public void TestMethod()
    {
        /* do nothing */
    }
}";

            var expected = Diagnostic().WithLocation(6, 26);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
