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
        /// Verifies that no diagnostics are reported for the valid indexers defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerValidAsync()
        {
            var testCode = @"public class Foo
{
    private bool test;

    // Valid indexer #1
    public bool this[byte index]
    {
        get { return this.test; }
        set { this.test = value; }
    }

    // Valid indexer #2
    public bool this[sbyte index]
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

    // Valid indexer #3 (Valid only for SA1500)
    public bool this[short index]
    {
        get { return this.test; }

        set 
        { 
            this.test = value; 
        }
    }

    // Valid indexer #4 (Valid only for SA1500)
    public bool this[ushort index]
    {
        get 
        { 
            return this.test; 
        }

        set { this.test = value; }
    }

    // Valid indexer #5 (Valid only for SA1500)
    public bool this[int index] { get { return this.test; }  set { this.test = value; } }

    // Valid indexer #6 (Valid only for SA1500)
    public bool this[uint index]
    { get { return this.test; }  set { this.test = value; } }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid indexer definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerInvalidAsync()
        {
            var testCode = @"public class Foo
{
    private bool test;

    // Invalid indexer #1
    public bool this[byte index]
    {
        get {
            return this.test;
        }

        set {
            this.test = value;
        }
    }

    // Invalid indexer #2
    public bool this[sbyte index]
    {
        get {
            return this.test; }

        set {
            this.test = value; }
    }

    // Invalid indexer #3
    public bool this[short index]
    {
        get { return this.test;
        }

        set { this.test = value;
        }
    }

    // Invalid indexer #4
    public bool this[ushort index]
    {
        get
        {
            return this.test; }

        set
        {
            this.test = value; }
    }

    // Invalid indexer #5
    public bool this[int index]
    {
        get
        { return this.test;
        }

        set
        { this.test = value;
        }
    }

    // Invalid indexer #6
    public bool this[uint index]
    {
        get
        { return this.test; }

        set
        { this.test = value; }
    }

    // Invalid indexer #7
    public bool this[long index] {
        get { return this.test; }
    }

    // Invalid indexer #8
    public bool this[ulong index] {
        get { return this.test; } }

    // Invalid indexer #9
    public bool this[bool index] { get { return this.test; }
    }

    // Invalid indexer #10
    public bool this[char index]
    {
        get { return this.test; } }

    // Invalid indexer #11
    public bool this[string index]
    { get { return this.test; }
    }
}";

            var fixedTestCode = @"public class Foo
{
    private bool test;

    // Invalid indexer #1
    public bool this[byte index]
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

    // Invalid indexer #2
    public bool this[sbyte index]
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

    // Invalid indexer #3
    public bool this[short index]
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

    // Invalid indexer #4
    public bool this[ushort index]
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

    // Invalid indexer #5
    public bool this[int index]
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

    // Invalid indexer #6
    public bool this[uint index]
    {
        get { return this.test; }

        set { this.test = value; }
    }

    // Invalid indexer #7
    public bool this[long index]
    {
        get { return this.test; }
    }

    // Invalid indexer #8
    public bool this[ulong index]
    {
        get { return this.test; }
    }

    // Invalid indexer #9
    public bool this[bool index]
    {
        get { return this.test; }
    }

    // Invalid indexer #10
    public bool this[char index]
    {
        get { return this.test; }
    }

    // Invalid indexer #11
    public bool this[string index]
    {
        get { return this.test; }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid indexer #1
                this.CSharpDiagnostic().WithLocation(8, 13),
                this.CSharpDiagnostic().WithLocation(12, 13),

                // Invalid indexer #2
                this.CSharpDiagnostic().WithLocation(20, 13),
                this.CSharpDiagnostic().WithLocation(21, 31),
                this.CSharpDiagnostic().WithLocation(23, 13),
                this.CSharpDiagnostic().WithLocation(24, 32),

                // Invalid indexer #3
                this.CSharpDiagnostic().WithLocation(30, 13),
                this.CSharpDiagnostic().WithLocation(33, 13),

                // Invalid indexer #4
                this.CSharpDiagnostic().WithLocation(42, 31),
                this.CSharpDiagnostic().WithLocation(46, 32),

                // Invalid indexer #5
                this.CSharpDiagnostic().WithLocation(53, 9),
                this.CSharpDiagnostic().WithLocation(57, 9),

                // Invalid indexer #6 (Only report once for accessor statements on a single line)
                this.CSharpDiagnostic().WithLocation(65, 9),
                this.CSharpDiagnostic().WithLocation(68, 9),

                // Invalid indexer #7
                this.CSharpDiagnostic().WithLocation(72, 34),

                // Invalid indexer #8
                this.CSharpDiagnostic().WithLocation(77, 35),
                this.CSharpDiagnostic().WithLocation(78, 35),

                // Invalid indexer #9
                this.CSharpDiagnostic().WithLocation(81, 34),

                // Invalid indexer #10
                this.CSharpDiagnostic().WithLocation(87, 35),

                // Invalid indexer #11
                this.CSharpDiagnostic().WithLocation(91, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
