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
        /// Verifies that no diagnostics are reported for the valid indexers defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx)
        /// series.</para>
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 13),
                Diagnostic().WithLocation(12, 13),

                // Invalid indexer #2
                Diagnostic().WithLocation(20, 13),
                Diagnostic().WithLocation(21, 31),
                Diagnostic().WithLocation(23, 13),
                Diagnostic().WithLocation(24, 32),

                // Invalid indexer #3
                Diagnostic().WithLocation(30, 13),
                Diagnostic().WithLocation(33, 13),

                // Invalid indexer #4
                Diagnostic().WithLocation(42, 31),
                Diagnostic().WithLocation(46, 32),

                // Invalid indexer #5
                Diagnostic().WithLocation(53, 9),
                Diagnostic().WithLocation(57, 9),

                // Invalid indexer #6 (Only report once for accessor statements on a single line)
                Diagnostic().WithLocation(65, 9),
                Diagnostic().WithLocation(68, 9),

                // Invalid indexer #7
                Diagnostic().WithLocation(72, 34),

                // Invalid indexer #8
                Diagnostic().WithLocation(77, 35),
                Diagnostic().WithLocation(78, 35),

                // Invalid indexer #9
                Diagnostic().WithLocation(81, 34),

                // Invalid indexer #10
                Diagnostic().WithLocation(87, 35),

                // Invalid indexer #11
                Diagnostic().WithLocation(91, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
