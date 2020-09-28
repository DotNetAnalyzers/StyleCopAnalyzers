// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using StyleCop.Analyzers.Lightup;
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
        /// Verifies that no diagnostics are reported for the valid properties defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics.</para>
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
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

    // Invalid property #15
    public int[] Property15 { get; set; } = 
    { 0, 1, 2 };
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

    // Invalid property #15
    public int[] Property15 { get; set; } = 
    {
        0, 1, 2
    };
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid property #1
                Diagnostic().WithLocation(10, 13),
                Diagnostic().WithLocation(14, 13),

                // Invalid property #2
                Diagnostic().WithLocation(22, 13),
                Diagnostic().WithLocation(23, 31),
                Diagnostic().WithLocation(25, 13),
                Diagnostic().WithLocation(26, 32),

                // Invalid property #3
                Diagnostic().WithLocation(32, 13),
                Diagnostic().WithLocation(35, 13),

                // Invalid property #4
                Diagnostic().WithLocation(44, 31),
                Diagnostic().WithLocation(48, 32),

                // Invalid property #5
                Diagnostic().WithLocation(55, 9),
                Diagnostic().WithLocation(59, 9),

                // Invalid property #6 (Only report once for accessor statements on a single line)
                Diagnostic().WithLocation(67, 9),
                Diagnostic().WithLocation(70, 9),

                // Invalid property #7
                Diagnostic().WithLocation(76, 35),

                // Invalid property #8
                Diagnostic().WithLocation(79, 27),

                // Invalid property #9
                Diagnostic().WithLocation(84, 27),
                Diagnostic().WithLocation(85, 35),

                // Invalid property #10
                Diagnostic().WithLocation(88, 28),

                // Invalid property #11
                Diagnostic().WithLocation(93, 5),

                // Invalid property #12
                Diagnostic().WithLocation(101, 11),

                // Invalid property #13
                Diagnostic().WithLocation(104, 45),

                // Invalid property #14
                Diagnostic().WithLocation(111, 45),

                // Invalid property #15
                Diagnostic().WithLocation(116, 5),
                Diagnostic().WithLocation(116, 15),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(6, 13),
                Diagnostic().WithLocation(8, 43),
                Diagnostic().WithLocation(8, 45),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a property declaration missing the opening brace will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAccessorMissingOpeningBraceAsync()
        {
            var testCode = @"
class ClassName
{
    int Property
    {
        get
        }
    }
}";

            DiagnosticResult accessorError;
            if (LightupHelpers.SupportsCSharp7)
            {
                accessorError = DiagnosticResult.CompilerError("CS8180").WithMessage("{ or ; or => expected");
            }
            else
            {
                accessorError = DiagnosticResult.CompilerError("CS1043").WithMessage("{ or ; expected");
            }

            DiagnosticResult[] expected =
            {
                accessorError.WithLocation(6, 12),
                DiagnosticResult.CompilerError("CS1022").WithMessage("Type or namespace definition, or end-of-file expected").WithLocation(9, 1),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a property declaration missing the closing brace at the end of the source file will be handled
        /// correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAccessorMissingClosingBraceAtEndOfFileAsync()
        {
            var testCode = @"
class ClassName
{
    int Property
    {
        get
        {";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS0161").WithMessage("'ClassName.Property.get': not all code paths return a value").WithLocation(6, 9),
                DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(7, 10),
                DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(7, 10),
                DiagnosticResult.CompilerError("CS1513").WithMessage("} expected").WithLocation(7, 10),
            };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
