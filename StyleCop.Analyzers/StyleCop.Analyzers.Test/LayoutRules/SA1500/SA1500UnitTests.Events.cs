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
        /// Verifies that no diagnostics are reported for the valid events defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventValidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private EventHandler test;

    // Valid event #1
    public event EventHandler Event1
    {
        add { this.test += value; }
        remove { this.test -= value; }
    }

    // Valid event #2
    public event EventHandler Event2
    {
        add 
        { 
            this.test += value; 
        }

        remove 
        { 
            this.test -= value; 
        }
    }

    // Valid event #3  (Valid for SA1500 only)
    public event EventHandler Event3
    {
        add { this.test += value; }
        
        remove 
        { 
        }
    }

    // Valid event #4  (Valid for SA1500 only)
    public event EventHandler Event4
    {
        add 
        { 
            this.test += value; 
        }

        remove { }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid event definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventInvalidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private EventHandler test;

    // Invalid event #1
    public event EventHandler Event1
    {
        add {
            this.test += value;
        }

        remove {
            this.test -= value;
        }
    }

    // Invalid event #2
    public event EventHandler Event2
    {
        add {
            this.test += value; }

        remove {
            this.test -= value; }
    }

    // Invalid event #3
    public event EventHandler Event3
    {
        add { this.test += value;
        }

        remove { this.test -= value;
        }
    }

    // Invalid event #4
    public event EventHandler Event4
    {
        add 
        {
            this.test += value; }

        remove 
        {
            this.test -= value; }
    }

    // Invalid event #5
    public event EventHandler Event5
    {
        add
        { this.test += value;
        }

        remove
        { this.test -= value;
        }
    }

    // Invalid event #6
    public event EventHandler Event6
    {
        add
        { this.test += value; }

        remove
        { this.test -= value; }
    }
}";

            var fixedTestCode = @"using System;

public class Foo
{
    private EventHandler test;

    // Invalid event #1
    public event EventHandler Event1
    {
        add
        {
            this.test += value;
        }

        remove
        {
            this.test -= value;
        }
    }

    // Invalid event #2
    public event EventHandler Event2
    {
        add
        {
            this.test += value;
        }

        remove
        {
            this.test -= value;
        }
    }

    // Invalid event #3
    public event EventHandler Event3
    {
        add
        {
            this.test += value;
        }

        remove
        {
            this.test -= value;
        }
    }

    // Invalid event #4
    public event EventHandler Event4
    {
        add 
        {
            this.test += value;
        }

        remove 
        {
            this.test -= value;
        }
    }

    // Invalid event #5
    public event EventHandler Event5
    {
        add
        {
            this.test += value;
        }

        remove
        {
            this.test -= value;
        }
    }

    // Invalid event #6
    public event EventHandler Event6
    {
        add { this.test += value; }

        remove { this.test -= value; }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid event #1
                this.CSharpDiagnostic().WithLocation(10, 13),
                this.CSharpDiagnostic().WithLocation(14, 16),

                // Invalid event #2
                this.CSharpDiagnostic().WithLocation(22, 13),
                this.CSharpDiagnostic().WithLocation(23, 33),
                this.CSharpDiagnostic().WithLocation(25, 16),
                this.CSharpDiagnostic().WithLocation(26, 33),

                // Invalid event #3
                this.CSharpDiagnostic().WithLocation(32, 13),
                this.CSharpDiagnostic().WithLocation(35, 16),

                // Invalid event #4
                this.CSharpDiagnostic().WithLocation(44, 33),
                this.CSharpDiagnostic().WithLocation(48, 33),

                // Invalid event #5
                this.CSharpDiagnostic().WithLocation(55, 9),
                this.CSharpDiagnostic().WithLocation(59, 9),

                // Invalid event #6 (Only report once for accessor statement on a single line)
                this.CSharpDiagnostic().WithLocation(67, 9),
                this.CSharpDiagnostic().WithLocation(70, 9)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
