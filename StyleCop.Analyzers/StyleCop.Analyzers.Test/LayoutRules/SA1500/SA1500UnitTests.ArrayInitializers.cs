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
        /// Verifies that no diagnostics are reported for the valid array initializers defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestArrayInitializersValidAsync()
        {
            var testCode = @"
using System;

public class TestClass
{
    private int[] array1 = { };

    private int[] array2 = { 0, 1 };

    private int[] array3 = new[] { 0, 1 };

    private int[] array3b = new int[] { 0, 1 };

    private int[] array4 =
    {
    };

    private int[] array5 =
    {
        0,
        1,
    };

    private int[] array6 = new[]
    {
        0,
    };

    public void TestMethod()
    {
        int[] array7 = { };

        int[] array8 = { 0, 1 };

        var array9 = new[] { 0, 1 };

        int[] array10 =
        {
        };

        int[] array11 =
        {
            0,
            1,
        };

        var array12 = new[]
        {
            0,
        };

        Console.WriteLine(new[] { 0, 1 });
        Console.WriteLine(new int[] { 0, 1 });
        Console.WriteLine(new int[] { });
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid implicit array initializer definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestImplicitArrayInitializersInvalidAsync()
        {
            var testCode = @"
public class TestClass
{
    private int[] invalidArray1 =
        { };

    private int[] invalidArray2 =
        { 0, 1 };

    private int[] invalidArray3 =
        { 0, 1
        };

    private int[] invalidArray4 =
        {
            0, 1 };

    private int[] invalidArray5 = new[]
        { 0, 1 };

    private int[] invalidArray6 = new[]
        { 0, 1
        };

    private int[] invalidArray7 = new[]
        {
            0, 1 };

    public void TestMethod()
    {
        int[] invalidArray8 =
            { };

        int[] invalidArray9 =
            { 0, 1 };

        int[] invalidArray10 =
            { 0, 1
            };

        int[] invalidArray11 =
            {
                0, 1 };

        var invalidArray12 = new[]
            { 0, 1 };

        var invalidArray13 = new[]
            { 0, 1
            };

        var invalidArray14 = new[]
            {
                0, 1 };
    }
}";

            var fixedTestCode = @"
public class TestClass
{
    private int[] invalidArray1 =
        {
        };

    private int[] invalidArray2 =
        {
            0, 1
        };

    private int[] invalidArray3 =
        {
            0, 1
        };

    private int[] invalidArray4 =
        {
            0, 1
        };

    private int[] invalidArray5 = new[]
        {
            0, 1
        };

    private int[] invalidArray6 = new[]
        {
            0, 1
        };

    private int[] invalidArray7 = new[]
        {
            0, 1
        };

    public void TestMethod()
    {
        int[] invalidArray8 =
            {
            };

        int[] invalidArray9 =
            {
                0, 1
            };

        int[] invalidArray10 =
            {
                0, 1
            };

        int[] invalidArray11 =
            {
                0, 1
            };

        var invalidArray12 = new[]
            {
                0, 1
            };

        var invalidArray13 = new[]
            {
                0, 1
            };

        var invalidArray14 = new[]
            {
                0, 1
            };
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(5, 11),
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(8, 16),
                this.CSharpDiagnostic().WithLocation(11, 9),
                this.CSharpDiagnostic().WithLocation(16, 18),
                this.CSharpDiagnostic().WithLocation(19, 9),
                this.CSharpDiagnostic().WithLocation(19, 16),
                this.CSharpDiagnostic().WithLocation(22, 9),
                this.CSharpDiagnostic().WithLocation(27, 18),
                this.CSharpDiagnostic().WithLocation(32, 13),
                this.CSharpDiagnostic().WithLocation(32, 15),
                this.CSharpDiagnostic().WithLocation(35, 13),
                this.CSharpDiagnostic().WithLocation(35, 20),
                this.CSharpDiagnostic().WithLocation(38, 13),
                this.CSharpDiagnostic().WithLocation(43, 22),
                this.CSharpDiagnostic().WithLocation(46, 13),
                this.CSharpDiagnostic().WithLocation(46, 20),
                this.CSharpDiagnostic().WithLocation(49, 13),
                this.CSharpDiagnostic().WithLocation(54, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid array initializer definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2607, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2607")]
        public async Task TestArrayInitializersInvalidAsync()
        {
            var testCode = @"
public class TestClass
{
    private int[] invalidArray5 = new int[]
        { 0, 1 };

    private int[] invalidArray6 = new int[]
        { 0, 1
        };

    private int[] invalidArray7 = new int[]
        {
            0, 1 };

    public void TestMethod()
    {
        var invalidArray12 = new int[]
            { 0, 1 };

        var invalidArray13 = new int[]
            { 0, 1
            };

        var invalidArray14 = new int[]
            {
                0, 1 };
    }
}";

            var fixedTestCode = @"
public class TestClass
{
    private int[] invalidArray5 = new int[]
        {
            0, 1
        };

    private int[] invalidArray6 = new int[]
        {
            0, 1
        };

    private int[] invalidArray7 = new int[]
        {
            0, 1
        };

    public void TestMethod()
    {
        var invalidArray12 = new int[]
            {
                0, 1
            };

        var invalidArray13 = new int[]
            {
                0, 1
            };

        var invalidArray14 = new int[]
            {
                0, 1
            };
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(5, 9),
                this.CSharpDiagnostic().WithLocation(5, 16),
                this.CSharpDiagnostic().WithLocation(8, 9),
                this.CSharpDiagnostic().WithLocation(13, 18),
                this.CSharpDiagnostic().WithLocation(18, 13),
                this.CSharpDiagnostic().WithLocation(18, 20),
                this.CSharpDiagnostic().WithLocation(21, 13),
                this.CSharpDiagnostic().WithLocation(26, 22),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
