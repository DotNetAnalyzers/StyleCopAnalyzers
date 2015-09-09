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
        /// Verifies that no diagnostics are reported for the valid delegates defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(14, 37),

                // Invalid delegate #2
                this.CSharpDiagnostic().WithLocation(18, 37),

                // Invalid delegate #3
                this.CSharpDiagnostic().WithLocation(23, 37),
                this.CSharpDiagnostic().WithLocation(24, 29),

                // Invalid delegate #4
                this.CSharpDiagnostic().WithLocation(27, 37),

                // Invalid delegate #5
                this.CSharpDiagnostic().WithLocation(33, 29),

                // Invalid delegate #6
                this.CSharpDiagnostic().WithLocation(37, 9),

                // Invalid delegate #7
                this.CSharpDiagnostic().WithLocation(41, 34),

                // Invalid delegate #8
                this.CSharpDiagnostic().WithLocation(45, 34),

                // Invalid delegate #9
                this.CSharpDiagnostic().WithLocation(50, 34),
                this.CSharpDiagnostic().WithLocation(51, 29),

                // Invalid delegate #10
                this.CSharpDiagnostic().WithLocation(54, 34),

                // Invalid delegate #11
                this.CSharpDiagnostic().WithLocation(60, 29),

                // Invalid delegate #12
                this.CSharpDiagnostic().WithLocation(64, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
