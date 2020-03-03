// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// Verifies that no diagnostics are reported for the valid delegates defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDelegateValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    private delegate void MyDelegate();

    private void TestMethod(MyDelegate d)
    {
    }

    private void Bar()
    {
        // Valid delegate #1
        MyDelegate item1 = delegate { };
        
        // Valid delegate #2
        MyDelegate item2 = delegate { Debug.Indent(); };

        // Valid delegate #3
        MyDelegate item3 = delegate
        {
        };

        // Valid delegate #4
        MyDelegate item4 = delegate
        {
            Debug.Indent();
        };

        // Valid delegate #5
        MyDelegate item7 = delegate 
        { Debug.Indent(); };

        // Valid delegate #6
        this.TestMethod(delegate { });

        // Valid delegate #7
        this.TestMethod(delegate 
        { 
        });

        // Valid delegate #8
        this.TestMethod(delegate { Debug.Indent(); });

        // Valid delegate #9
        this.TestMethod(delegate 
        { 
            Debug.Indent(); 
        });

        // Valid delegate #10
        this.TestMethod(delegate 
        { Debug.Indent(); });
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies diagnostics and codefixes for all invalid delegate definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDelegateInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    private delegate void MyDelegate();

    private void TestMethod(MyDelegate d)
    {
    }

    private void Bar()
    {
        // Invalid delegate #1
        MyDelegate item1 = delegate {
        };
        
        // Invalid delegate #2
        MyDelegate item2 = delegate {
            Debug.Indent(); 
        };

        // Invalid delegate #3
        MyDelegate item3 = delegate {
            Debug.Indent(); };

        // Invalid delegate #4
        MyDelegate item4 = delegate { Debug.Indent();
        };

        // Invalid delegate #5
        MyDelegate item5 = delegate
        {
            Debug.Indent(); };

        // Invalid delegate #6
        MyDelegate item6 = delegate
        { Debug.Indent();
        };

        // Invalid delegate #7
        this.TestMethod(delegate {
        });

        // Invalid delegate #8
        this.TestMethod(delegate {
            Debug.Indent();
        });

        // Invalid delegate #9
        this.TestMethod(delegate {
            Debug.Indent(); });

        // Invalid delegate #10
        this.TestMethod(delegate { Debug.Indent();
        });

        // Invalid delegate #11
        this.TestMethod(delegate
        {
            Debug.Indent(); });

        // Invalid delegate #12
        this.TestMethod(delegate
        { Debug.Indent();
        });
    }
}";

            var fixedTestCode = @"using System.Diagnostics;

public class Foo
{
    private delegate void MyDelegate();

    private void TestMethod(MyDelegate d)
    {
    }

    private void Bar()
    {
        // Invalid delegate #1
        MyDelegate item1 = delegate
        {
        };
        
        // Invalid delegate #2
        MyDelegate item2 = delegate
        {
            Debug.Indent(); 
        };

        // Invalid delegate #3
        MyDelegate item3 = delegate
        {
            Debug.Indent();
        };

        // Invalid delegate #4
        MyDelegate item4 = delegate
        {
            Debug.Indent();
        };

        // Invalid delegate #5
        MyDelegate item5 = delegate
        {
            Debug.Indent();
        };

        // Invalid delegate #6
        MyDelegate item6 = delegate
        {
            Debug.Indent();
        };

        // Invalid delegate #7
        this.TestMethod(delegate
        {
        });

        // Invalid delegate #8
        this.TestMethod(delegate
        {
            Debug.Indent();
        });

        // Invalid delegate #9
        this.TestMethod(delegate
        {
            Debug.Indent();
        });

        // Invalid delegate #10
        this.TestMethod(delegate
        {
            Debug.Indent();
        });

        // Invalid delegate #11
        this.TestMethod(delegate
        {
            Debug.Indent();
        });

        // Invalid delegate #12
        this.TestMethod(delegate
        {
            Debug.Indent();
        });
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid delegate #1
                Diagnostic().WithLocation(14, 37),

                // Invalid delegate #2
                Diagnostic().WithLocation(18, 37),

                // Invalid delegate #3
                Diagnostic().WithLocation(23, 37),
                Diagnostic().WithLocation(24, 29),

                // Invalid delegate #4
                Diagnostic().WithLocation(27, 37),

                // Invalid delegate #5
                Diagnostic().WithLocation(33, 29),

                // Invalid delegate #6
                Diagnostic().WithLocation(37, 9),

                // Invalid delegate #7
                Diagnostic().WithLocation(41, 34),

                // Invalid delegate #8
                Diagnostic().WithLocation(45, 34),

                // Invalid delegate #9
                Diagnostic().WithLocation(50, 34),
                Diagnostic().WithLocation(51, 29),

                // Invalid delegate #10
                Diagnostic().WithLocation(54, 34),

                // Invalid delegate #11
                Diagnostic().WithLocation(60, 29),

                // Invalid delegate #12
                Diagnostic().WithLocation(64, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
