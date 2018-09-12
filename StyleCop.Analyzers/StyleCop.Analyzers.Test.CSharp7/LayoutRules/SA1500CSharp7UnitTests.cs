// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    public class SA1500CSharp7UnitTests : SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid local functions defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Method()
    {
        // Valid local function #1
        void LocalFunction1()
        {
        }

        // Valid local function #2
        void LocalFunction2()
        {
            Debug.Indent();
        }

        // Valid local function #3 (Valid only for SA1500)
        void LocalFunction3() { }

        // Valid local function #4 (Valid only for SA1500)
        void LocalFunction4() { Debug.Indent(); }

        // Valid local function #5 (Valid only for SA1500)
        void LocalFunction5() 
        { Debug.Indent(); }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid local function definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Method()
    {
        // Invalid local function #1
        void LocalFunction1() {
        }

        // Invalid local function #2
        void LocalFunction2() {
            Debug.Indent();
        }

        // Invalid local function #3
        void LocalFunction3() {
            Debug.Indent(); }

        // Invalid local function #4
        void LocalFunction4() { Debug.Indent();
        }

        // Invalid local function #5
        void LocalFunction5()
        {
            Debug.Indent(); }

        // Invalid local function #6
        void LocalFunction6()
        { Debug.Indent();
        }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;

public class Foo
{
    public void Method()
    {
        // Invalid local function #1
        void LocalFunction1()
        {
        }

        // Invalid local function #2
        void LocalFunction2()
        {
            Debug.Indent();
        }

        // Invalid local function #3
        void LocalFunction3()
        {
            Debug.Indent();
        }

        // Invalid local function #4
        void LocalFunction4()
        {
            Debug.Indent();
        }

        // Invalid local function #5
        void LocalFunction5()
        {
            Debug.Indent();
        }

        // Invalid local function #6
        void LocalFunction6()
        {
            Debug.Indent();
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid local function #1
                Diagnostic().WithLocation(8, 31),

                // Invalid local function #2
                Diagnostic().WithLocation(12, 31),

                // Invalid local function #3
                Diagnostic().WithLocation(17, 31),
                Diagnostic().WithLocation(18, 29),

                // Invalid local function #4
                Diagnostic().WithLocation(21, 31),

                // Invalid local function #5
                Diagnostic().WithLocation(27, 29),

                // Invalid local function #6
                Diagnostic().WithLocation(31, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
