// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1504AllAccessorsMustBeSingleLineOrMultiLine,
        StyleCop.Analyzers.LayoutRules.SA1504CodeFixProvider>;

    public class SA1504ArrowAccessorCSharp7UnitTests
    {
        /// <summary>
        /// Verifies that a single-line expression-bodied accessor and a multi-line block-bodied accessor are reported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2402, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2402")]
        public async Task TestExpressionBodiedGetterAndMultiLineSetterAsync()
        {
            var testCode = @"
public class Foo
{
    private int value;

    public int Value
    {
        {|#0:get|} => this.value;
        set
        {
            this.value = value;
        }
    }
}";

            var fixedTestCodeSingle = @"
public class Foo
{
    private int value;

    public int Value
    {
        get => this.value;
        set { this.value = value; }
    }
}";

            var fixedTestCodeMultiple = @"
public class Foo
{
    private int value;

    public int Value
    {
        get =>
            this.value;

        set
        {
            this.value = value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeSingle,
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that expression-bodied and block-bodied accessors are classified by their line count.
        /// </summary>
        /// <param name="accessors">The accessors in the property.</param>
        /// <param name="shouldReport">A value indicating whether SA1504 should be reported.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("        {|#0:get|} => this.value;\n        set\n        {\n            this.value = value;\n        }", true)]
        [InlineData("        {|#0:get|} =>\n            this.value;\n        set { this.value = value; }", true)]
        [InlineData("        {|#0:get|} => this.value\n        ;\n        set => this.value = value;", true)]
        [InlineData("        get => this.value;\n        set => this.value = value;", false)]
        [InlineData("        get =>\n            this.value;\n        set =>\n            this.value = value;", false)]
        public async Task TestExpressionBodiedAndBlockBodiedAccessorLineCountsAsync(string accessors, bool shouldReport)
        {
            var testCode = $@"
public class Foo
{{
    private int value;

    public int Value
    {{
{accessors}
    }}
}}";
            DiagnosticResult[] expected = shouldReport
                ? new[] { Diagnostic().WithLocation(0) }
                : DiagnosticResult.EmptyDiagnosticResults;

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the single-line fix moves a line-broken expression-body semicolon.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2402, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2402")]
        public async Task TestExpressionBodySemicolonOnSeparateLineAsync()
        {
            var testCode = @"
public class Foo
{
    private int value;

    public int Value
    {
        {|#0:get|} => this.value
            ;
        set => this.value = value;
    }
}";

            var fixedTestCode = @"
public class Foo
{
    private int value;

    public int Value
    {
        get => this.value;
        set => this.value = value;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCode,
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an attribute on an expression-bodied accessor does not affect its line count.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAttributeDoesNotHideExpressionBodiedAccessorAsync()
        {
            var testCode = @"
public class Foo
{
    private int value;

    public int Value
    {
        [System.Diagnostics.DebuggerStepThrough]
        {|#0:get|} => this.value;
        set
        {
            this.value = value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that expression-bodied indexer accessors are analyzed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestExpressionBodiedIndexerAccessorAsync()
        {
            var testCode = @"
public class Foo
{
    private int value;

    public int this[int index]
    {
        {|#0:get|} => index;
        set
        {
            this.value = value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that expression-bodied event accessors are analyzed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestExpressionBodiedEventAccessorAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler changed;

    public event System.EventHandler Changed
    {
        {|#0:add|} => this.changed += value;
        remove
        {
            this.changed -= value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
