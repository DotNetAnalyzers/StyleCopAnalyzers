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
        /// Verifies that no diagnostics are reported for the valid destructors defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Valid destructor #1
    public class TestClass1
    {
        ~TestClass1()
        {
        }
    }

    // Valid destructor #2
    public class TestClass2
    {
        ~TestClass2()
        {
            Debug.Indent();
        }
    }

    // Valid destructor #3 (Valid only for SA1500)
    public class TestClass3
    {
        ~TestClass3() { }
    }

    // Valid destructor #4 (Valid only for SA1500)
    public class TestClass4
    {
        ~TestClass4() { Debug.Indent(); }
    }

    // Valid destructor #5 (Valid only for SA1500)
    public class TestClass5
    {
        ~TestClass5() 
        { Debug.Indent(); }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics and codefixes for all invalid destructor definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    // Invalid destructor #1
    public class TestClass1
    {
        ~TestClass1() {
        }
    }

    // Invalid destructor #2
    public class TestClass2
    {
        ~TestClass2() {
            Debug.Indent();
        }
    }

    // Invalid destructor #3
    public class TestClass3
    {
        ~TestClass3() {
            Debug.Indent(); }
    }

    // Invalid destructor #4
    public class TestClass4
    {
        ~TestClass4() { Debug.Indent();
        }
    }

    // Invalid destructor #5
    public class TestClass5
    {
        ~TestClass5()
        {
            Debug.Indent(); }
    }

    // Invalid destructor #6
    public class TestClass6
    {
        ~TestClass6()
        { Debug.Indent();
        }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;

public class Foo
{
    // Invalid destructor #1
    public class TestClass1
    {
        ~TestClass1()
        {
        }
    }

    // Invalid destructor #2
    public class TestClass2
    {
        ~TestClass2()
        {
            Debug.Indent();
        }
    }

    // Invalid destructor #3
    public class TestClass3
    {
        ~TestClass3()
        {
            Debug.Indent();
        }
    }

    // Invalid destructor #4
    public class TestClass4
    {
        ~TestClass4()
        {
            Debug.Indent();
        }
    }

    // Invalid destructor #5
    public class TestClass5
    {
        ~TestClass5()
        {
            Debug.Indent();
        }
    }

    // Invalid destructor #6
    public class TestClass6
    {
        ~TestClass6()
        {
            Debug.Indent();
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid destructor #1
                this.CSharpDiagnostic().WithLocation(8, 23),

                // Invalid destructor #2
                this.CSharpDiagnostic().WithLocation(15, 23),

                // Invalid destructor #3
                this.CSharpDiagnostic().WithLocation(23, 23),
                this.CSharpDiagnostic().WithLocation(24, 29),

                // Invalid destructor #4
                this.CSharpDiagnostic().WithLocation(30, 23),

                // Invalid destructor #5
                this.CSharpDiagnostic().WithLocation(39, 29),

                // Invalid destructor #6
                this.CSharpDiagnostic().WithLocation(46, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
