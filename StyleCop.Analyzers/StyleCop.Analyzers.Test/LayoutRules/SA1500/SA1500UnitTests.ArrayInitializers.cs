// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid array initializers defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics.</para>
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(5, 9),
                Diagnostic().WithLocation(5, 11),
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(8, 16),
                Diagnostic().WithLocation(11, 9),
                Diagnostic().WithLocation(16, 18),
                Diagnostic().WithLocation(19, 9),
                Diagnostic().WithLocation(19, 16),
                Diagnostic().WithLocation(22, 9),
                Diagnostic().WithLocation(27, 18),
                Diagnostic().WithLocation(32, 13),
                Diagnostic().WithLocation(32, 15),
                Diagnostic().WithLocation(35, 13),
                Diagnostic().WithLocation(35, 20),
                Diagnostic().WithLocation(38, 13),
                Diagnostic().WithLocation(43, 22),
                Diagnostic().WithLocation(46, 13),
                Diagnostic().WithLocation(46, 20),
                Diagnostic().WithLocation(49, 13),
                Diagnostic().WithLocation(54, 22),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(5, 9),
                Diagnostic().WithLocation(5, 16),
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(13, 18),
                Diagnostic().WithLocation(18, 13),
                Diagnostic().WithLocation(18, 20),
                Diagnostic().WithLocation(21, 13),
                Diagnostic().WithLocation(26, 22),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi-dimensional array initialization produces the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2632, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2632")]
        public async Task VerifyMultidimensionalArrayInitializationAsync()
        {
            var testCode = @"
public class TestClass
{
    private static readonly float[,] TestMatrix1 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };

    private static readonly float[,] TestMatrix2 =
        new float[,]
        {   { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };

    private static readonly float[,] TestMatrix3 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 } };

    private static readonly float[,] TestMatrix4 =
        new float[,]
        {
            { 0, 0, 1, 1 }, { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };
}
";

            var fixedTestCode = @"
public class TestClass
{
    private static readonly float[,] TestMatrix1 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };

    private static readonly float[,] TestMatrix2 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };

    private static readonly float[,] TestMatrix3 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };

    private static readonly float[,] TestMatrix4 =
        new float[,]
        {
            { 0, 0, 1, 1 },
            { 1, 1, 1, 0 },
            { 0, 1, 0, 0 }
        };
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(14, 9),
                Diagnostic().WithLocation(24, 28),
                Diagnostic().WithLocation(29, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a jagged array initialization produces the expected diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2632, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2632")]
        public async Task VerifyJaggedArrayInitializationAsync()
        {
            var testCode = @"
public class TestClass
{
    private static readonly int[][] TestMatrix1 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };

    private static readonly int[][] TestMatrix2 =
        new int[][]
        {   new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };

    private static readonly int[][] TestMatrix3 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 } };

    private static readonly int[][] TestMatrix4 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 }, new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };
}
";

            var fixedTestCode = @"
public class TestClass
{
    private static readonly int[][] TestMatrix1 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };

    private static readonly int[][] TestMatrix2 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };

    private static readonly int[][] TestMatrix3 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 },
            new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };

    private static readonly int[][] TestMatrix4 =
        new int[][]
        {
            new[] { 0, 0, 1, 1 }, new[] { 1, 1, 1, 0 },
            new[] { 0, 1, 0, 0 }
        };
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(14, 9),
                Diagnostic().WithLocation(24, 34),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
