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
        /// Verifies that no diagnostics are reported for the valid properties defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyValidAsync()
        {
            var testCode = @"using System;
using System.Collections.Generic;

public class Foo
{
    private bool test;

    // Valid property #1
    public bool Property1
    {
        get { return this.test; }
        set { this.test = value; }
    }

    // Valid property #2
    public bool Property2
    {
        get 
        { 
            return this.test; 
        }

        set 
        { 
            this.test = value; 
        }
    }

    // Valid property #3  (Valid for SA1500 only)
    public bool Property3
    {
        get { return this.test; }
        
        set 
        { 
        }
    }

    // Valid property #4  (Valid for SA1500 only)
    public bool Property4
    {
        get 
        { 
            return this.test; 
        }

        set { }
    }

    // Valid property #5  (Valid for SA1500 only)
    public bool Property5 { get { return this.test; } }

    // Valid property #6  (Valid for SA1500 only)
    public bool Property6 
    { get { return this.test; } }

    // Valid property #7
    public int[] Property7 { get; set; } = 
    { 
        0, 
        1, 
        2 
    };

    // Valid property #8  (Valid for SA1500 only)
    public int[] Property8 { get; set; } = { 0, 1, 2 };

    // Valid property #9  (Valid for SA1500 only)
    public int[] Property9 { get; set; } = 
    { 0, 1, 2 };
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid property definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyInvalidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private bool test;

    // Invalid property #1
    public bool Property1
    {
        get {
            return this.test;
        }

        set {
            this.test = value;
        }
    }

    // Invalid property #2
    public bool Property2
    {
        get {
            return this.test; }

        set {
            this.test = value; }
    }

    // Invalid property #3
    public bool Property3
    {
        get { return this.test;
        }

        set { this.test = value;
        }
    }

    // Invalid property #4
    public bool Property4
    {
        get
        {
            return this.test; }

        set
        {
            this.test = value; }
    }

    // Invalid property #5
    public bool Property5
    {
        get
        { return this.test;
        }

        set
        { this.test = value;
        }
    }

    // Invalid property #6
    public bool Property6
    {
        get
        { return this.test; }

        set
        { this.test = value; }
    }

    // Invalid property #7
    public bool Property7
    {
        get { return this.test; } }

    // Invalid property #8
    public bool Property8 {
        get { return this.test; } 
    }

    // Invalid property #9
    public bool Property9 {
        get { return this.test; } }

    // Invalid property #10
    public bool Property10 { get { return this.test; }
    }

    // Invalid property #11
    public bool Property11
    { get { return this.test; }
    }

    // Invalid property #12
    public int[] Property12 { get; set; } =
    {
        0,
        1,
        2 };

    // Invalid property #13
    public int[] Property13 { get; set; } = {
        0,
        1,
        2
    };

    // Invalid property #14
    public int[] Property14 { get; set; } = { 0, 1, 2
    };
}";

            var fixedTestCode = @"using System;

public class Foo
{
    private bool test;

    // Invalid property #1
    public bool Property1
    {
        get
        {
            return this.test;
        }

        set
        {
            this.test = value;
        }
    }

    // Invalid property #2
    public bool Property2
    {
        get
        {
            return this.test;
        }

        set
        {
            this.test = value;
        }
    }

    // Invalid property #3
    public bool Property3
    {
        get
        {
            return this.test;
        }

        set
        {
            this.test = value;
        }
    }

    // Invalid property #4
    public bool Property4
    {
        get
        {
            return this.test;
        }

        set
        {
            this.test = value;
        }
    }

    // Invalid property #5
    public bool Property5
    {
        get
        {
            return this.test;
        }

        set
        {
            this.test = value;
        }
    }

    // Invalid property #6
    public bool Property6
    {
        get { return this.test; }

        set { this.test = value; }
    }

    // Invalid property #7
    public bool Property7
    {
        get { return this.test; }
    }

    // Invalid property #8
    public bool Property8
    {
        get { return this.test; } 
    }

    // Invalid property #9
    public bool Property9
    {
        get { return this.test; }
    }

    // Invalid property #10
    public bool Property10
    {
        get { return this.test; }
    }

    // Invalid property #11
    public bool Property11
    {
        get { return this.test; }
    }

    // Invalid property #12
    public int[] Property12 { get; set; } =
    {
        0,
        1,
        2
    };

    // Invalid property #13
    public int[] Property13 { get; set; } =
    {
        0,
        1,
        2
    };

    // Invalid property #14
    public int[] Property14 { get; set; } =
    {
        0, 1, 2
    };
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid property #1
                this.CSharpDiagnostic().WithLocation(10, 13),
                this.CSharpDiagnostic().WithLocation(14, 13),

                // Invalid property #2
                this.CSharpDiagnostic().WithLocation(22, 13),
                this.CSharpDiagnostic().WithLocation(23, 31),
                this.CSharpDiagnostic().WithLocation(25, 13),
                this.CSharpDiagnostic().WithLocation(26, 32),

                // Invalid property #3
                this.CSharpDiagnostic().WithLocation(32, 13),
                this.CSharpDiagnostic().WithLocation(35, 13),

                // Invalid property #4
                this.CSharpDiagnostic().WithLocation(44, 31),
                this.CSharpDiagnostic().WithLocation(48, 32),

                // Invalid property #5
                this.CSharpDiagnostic().WithLocation(55, 9),
                this.CSharpDiagnostic().WithLocation(59, 9),

                // Invalid property #6 (Only report once for accessor statements on a single line)
                this.CSharpDiagnostic().WithLocation(67, 9),
                this.CSharpDiagnostic().WithLocation(70, 9),

                // Invalid property #7
                this.CSharpDiagnostic().WithLocation(76, 35),

                // Invalid property #8
                this.CSharpDiagnostic().WithLocation(79, 27),

                // Invalid property #9
                this.CSharpDiagnostic().WithLocation(84, 27),
                this.CSharpDiagnostic().WithLocation(85, 35),

                // Invalid property #10
                this.CSharpDiagnostic().WithLocation(88, 28),

                // Invalid property #11
                this.CSharpDiagnostic().WithLocation(93, 5),

                // Invalid property #12
                this.CSharpDiagnostic().WithLocation(101, 11),

                // Invalid property #13
                this.CSharpDiagnostic().WithLocation(104, 45),

                // Invalid property #14
                this.CSharpDiagnostic().WithLocation(111, 45)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line accessor with an embedded block will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineAccessorWithEmbeddedBlockAsync()
        {
            var testCode = @"
public class TestClass
{
    public int[] TestProperty
    {
        get {
            {
                return new[] { 1, 2, 3 }; } }
    }
}
";

            var fixedTestCode = @"
public class TestClass
{
    public int[] TestProperty
    {
        get
        {
            {
                return new[] { 1, 2, 3 };
            }
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 13),
                this.CSharpDiagnostic().WithLocation(8, 43),
                this.CSharpDiagnostic().WithLocation(8, 45)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
