// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using TestHelper;
    using Xunit;

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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(8, 31),

                // Invalid local function #2
                this.CSharpDiagnostic().WithLocation(12, 31),

                // Invalid local function #3
                this.CSharpDiagnostic().WithLocation(17, 31),
                this.CSharpDiagnostic().WithLocation(18, 29),

                // Invalid local function #4
                this.CSharpDiagnostic().WithLocation(21, 31),

                // Invalid local function #5
                this.CSharpDiagnostic().WithLocation(27, 29),

                // Invalid local function #6
                this.CSharpDiagnostic().WithLocation(31, 9),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
