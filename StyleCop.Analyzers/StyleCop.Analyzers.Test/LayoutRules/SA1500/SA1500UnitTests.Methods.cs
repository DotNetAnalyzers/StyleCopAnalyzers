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
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid methods defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Valid method #1
    public void Method1()
    {
    }

    // Valid method #2
    public void Method2()
    {
        Debug.Fail(null);
    }

    // Valid method #3 (Valid only for SA1500)
    public void Method3() { }

    // Valid method #4 (Valid only for SA1500)
    public void Method4() { Debug.Fail(null); }

    // Valid method #5 (Valid only for SA1500)
    public void Method5() 
    { Debug.Fail(null); }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid method definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMethodInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Invalid method #1
    public void Method1() {
    }

    // Invalid method #2
    public void Method2() {
        Debug.Fail(null);
    }

    // Invalid method #3
    public void Method3() {
        Debug.Fail(null); }

    // Invalid method #4
    public void Method4() { Debug.Fail(null);
    }

    // Invalid method #5
    public void Method5()
    {
        Debug.Fail(null); }

    // Invalid method #6
    public void Method6()
    { Debug.Fail(null);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;

public class Foo
{
    // Invalid method #1
    public void Method1()
    {
    }

    // Invalid method #2
    public void Method2()
    {
        Debug.Fail(null);
    }

    // Invalid method #3
    public void Method3()
    {
        Debug.Fail(null);
    }

    // Invalid method #4
    public void Method4()
    {
        Debug.Fail(null);
    }

    // Invalid method #5
    public void Method5()
    {
        Debug.Fail(null);
    }

    // Invalid method #6
    public void Method6()
    {
        Debug.Fail(null);
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid method #1
                this.CSharpDiagnostic().WithLocation(6, 27),

                // Invalid method #2
                this.CSharpDiagnostic().WithLocation(10, 27),

                // Invalid method #3
                this.CSharpDiagnostic().WithLocation(15, 27),
                this.CSharpDiagnostic().WithLocation(16, 27),

                // Invalid method #4
                this.CSharpDiagnostic().WithLocation(19, 27),

                // Invalid method #5
                this.CSharpDiagnostic().WithLocation(25, 27),

                // Invalid method #6
                this.CSharpDiagnostic().WithLocation(29, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
