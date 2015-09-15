// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid lambda expressions defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(12, 30),

                // Invalid lambda expression #2
                this.CSharpDiagnostic().WithLocation(16, 30),

                // Invalid lambda expression #3
                this.CSharpDiagnostic().WithLocation(21, 30),
                this.CSharpDiagnostic().WithLocation(22, 29),

                // Invalid lambda expression #4
                this.CSharpDiagnostic().WithLocation(25, 30),

                // Invalid lambda expression #5
                this.CSharpDiagnostic().WithLocation(31, 29),

                // Invalid lambda expression #6
                this.CSharpDiagnostic().WithLocation(35, 9),

                // Invalid lambda expression #7
                this.CSharpDiagnostic().WithLocation(39, 31),

                // Invalid lambda expression #8
                this.CSharpDiagnostic().WithLocation(43, 31),

                // Invalid lambda expression #9
                this.CSharpDiagnostic().WithLocation(48, 31),
                this.CSharpDiagnostic().WithLocation(49, 29),

                // Invalid lambda expression #10
                this.CSharpDiagnostic().WithLocation(52, 31),

                // Invalid lambda expression #11
                this.CSharpDiagnostic().WithLocation(58, 29),

                // Invalid lambda expression #12
                this.CSharpDiagnostic().WithLocation(62, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
