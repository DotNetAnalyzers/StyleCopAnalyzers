// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid lambda expressions defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLambdaExpressionValidAsync()
        {
            var testCode = @"using System;
using System.Diagnostics;

public class Foo
{
    private void TestMethod(Action action)
    {
    }

    private void Bar()
    {
        // Valid lambda expression #1
        Action item1 = () => { };
        
        // Valid lambda expression #2
        Action item2 = () => { Debug.Indent(); };

        // Valid lambda expression #3
        Action item3 = () =>
        {
        };

        // Valid lambda expression #4
        Action item4 = () =>
        {
            Debug.Indent();
        };

        // Valid lambda expression #5
        Action item5 = () =>
        { Debug.Indent(); };

        // Valid lambda expression #6
        this.TestMethod(() => { });

        // Valid lambda expression #7
        this.TestMethod(() =>
        { 
        });

        // Valid lambda expression #8
        this.TestMethod(() => { Debug.Indent(); });

        // Valid lambda expression #9
        this.TestMethod(() =>
        { 
            Debug.Indent(); 
        });

        // Valid lambda expression #10
        this.TestMethod(() =>
        { Debug.Indent(); });
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid lambda expression definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLambdaExpressionInvalidAsync()
        {
            var testCode = @"using System;
using System.Diagnostics;
public class Foo
{
    private void TestMethod(Action action)
    {
    }

    private void Bar()
    {
        // Invalid lambda expression #1
        Action item1 = () => {
        };
        
        // Invalid lambda expression #2
        Action item2 = () => {
            Debug.Indent();
        };

        // Invalid lambda expression #3
        Action item3 = () => {
            Debug.Indent(); };

        // Invalid lambda expression #4
        Action item4 = () => { Debug.Indent();
        };

        // Invalid lambda expression #5
        Action item5 = () =>
        {
            Debug.Indent(); };

        // Invalid lambda expression #6
        Action item6 = () =>
        { Debug.Indent();
        };

        // Invalid lambda expression #7
        this.TestMethod(() => {
        });

        // Invalid lambda expression #8
        this.TestMethod(() => {
            Debug.Indent();
        });

        // Invalid lambda expression #9
        this.TestMethod(() => {
            Debug.Indent(); });

        // Invalid lambda expression #10
        this.TestMethod(() => { Debug.Indent();
        });

        // Invalid lambda expression #11
        this.TestMethod(() =>
        {
            Debug.Indent(); });

        // Invalid lambda expression #12
        this.TestMethod(() =>
        { Debug.Indent();
        });
    }
}";

            var fixedTestCode = @"using System;
using System.Diagnostics;
public class Foo
{
    private void TestMethod(Action action)
    {
    }

    private void Bar()
    {
        // Invalid lambda expression #1
        Action item1 = () =>
        {
        };
        
        // Invalid lambda expression #2
        Action item2 = () =>
        {
            Debug.Indent();
        };

        // Invalid lambda expression #3
        Action item3 = () =>
        {
            Debug.Indent();
        };

        // Invalid lambda expression #4
        Action item4 = () =>
        {
            Debug.Indent();
        };

        // Invalid lambda expression #5
        Action item5 = () =>
        {
            Debug.Indent();
        };

        // Invalid lambda expression #6
        Action item6 = () =>
        {
            Debug.Indent();
        };

        // Invalid lambda expression #7
        this.TestMethod(() =>
        {
        });

        // Invalid lambda expression #8
        this.TestMethod(() =>
        {
            Debug.Indent();
        });

        // Invalid lambda expression #9
        this.TestMethod(() =>
        {
            Debug.Indent();
        });

        // Invalid lambda expression #10
        this.TestMethod(() =>
        {
            Debug.Indent();
        });

        // Invalid lambda expression #11
        this.TestMethod(() =>
        {
            Debug.Indent();
        });

        // Invalid lambda expression #12
        this.TestMethod(() =>
        {
            Debug.Indent();
        });
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid lambda expression #1
                Diagnostic().WithLocation(12, 30),

                // Invalid lambda expression #2
                Diagnostic().WithLocation(16, 30),

                // Invalid lambda expression #3
                Diagnostic().WithLocation(21, 30),
                Diagnostic().WithLocation(22, 29),

                // Invalid lambda expression #4
                Diagnostic().WithLocation(25, 30),

                // Invalid lambda expression #5
                Diagnostic().WithLocation(31, 29),

                // Invalid lambda expression #6
                Diagnostic().WithLocation(35, 9),

                // Invalid lambda expression #7
                Diagnostic().WithLocation(39, 31),

                // Invalid lambda expression #8
                Diagnostic().WithLocation(43, 31),

                // Invalid lambda expression #9
                Diagnostic().WithLocation(48, 31),
                Diagnostic().WithLocation(49, 29),

                // Invalid lambda expression #10
                Diagnostic().WithLocation(52, 31),

                // Invalid lambda expression #11
                Diagnostic().WithLocation(58, 29),

                // Invalid lambda expression #12
                Diagnostic().WithLocation(62, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
