// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
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
        Debug.Indent();
    }

    // Valid method #3 (Valid only for SA1500)
    public void Method3() { }

    // Valid method #4 (Valid only for SA1500)
    public void Method4() { Debug.Indent(); }

    // Valid method #5 (Valid only for SA1500)
    public void Method5() 
    { Debug.Indent(); }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
        Debug.Indent();
    }

    // Invalid method #3
    public void Method3() {
        Debug.Indent(); }

    // Invalid method #4
    public void Method4() { Debug.Indent();
    }

    // Invalid method #5
    public void Method5()
    {
        Debug.Indent(); }

    // Invalid method #6
    public void Method6()
    { Debug.Indent();
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
        Debug.Indent();
    }

    // Invalid method #3
    public void Method3()
    {
        Debug.Indent();
    }

    // Invalid method #4
    public void Method4()
    {
        Debug.Indent();
    }

    // Invalid method #5
    public void Method5()
    {
        Debug.Indent();
    }

    // Invalid method #6
    public void Method6()
    {
        Debug.Indent();
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid method #1
                Diagnostic().WithLocation(6, 27),

                // Invalid method #2
                Diagnostic().WithLocation(10, 27),

                // Invalid method #3
                Diagnostic().WithLocation(15, 27),
                Diagnostic().WithLocation(16, 25),

                // Invalid method #4
                Diagnostic().WithLocation(19, 27),

                // Invalid method #5
                Diagnostic().WithLocation(25, 25),

                // Invalid method #6
                Diagnostic().WithLocation(29, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }
    }
}
