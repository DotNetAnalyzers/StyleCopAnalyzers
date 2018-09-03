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
        /// Verifies that no diagnostics are reported for the valid structs defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStructValidAsync()
        {
            var testCode = @"public class Foo
{
    public struct ValidStruct1
    {
    }

    public struct ValidStruct2
    {
        public int Field;
    }

    public struct ValidStruct3 { } /* Valid only for SA1500 */

    public struct ValidStruct4 { public int Field; }  /* Valid only for SA1500 */

    public struct ValidStruct5 /* Valid only for SA1500 */
    { public int Field; }  
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid struct definitions.
        /// </summary>
        /// <remarks>
        /// These will normally also report SA1401, but not in the unit test.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStructInvalidAsync()
        {
            var testCode = @"public class Foo
{
    public struct InvalidStruct1 {
    }

    public struct InvalidStruct2 {
        public int Field;
    }

    public struct InvalidStruct3 {
        public int Field; }

    public struct InvalidStruct4 { public int Field;
    }

    public struct InvalidStruct5
    {
        public int Field; }

    public struct InvalidStruct6
    { public int Field;
    }
}";

            var fixedTestCode = @"public class Foo
{
    public struct InvalidStruct1
    {
    }

    public struct InvalidStruct2
    {
        public int Field;
    }

    public struct InvalidStruct3
    {
        public int Field;
    }

    public struct InvalidStruct4
    {
        public int Field;
    }

    public struct InvalidStruct5
    {
        public int Field;
    }

    public struct InvalidStruct6
    {
        public int Field;
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidStruct1
                Diagnostic().WithLocation(3, 34),

                // InvalidStruct2
                Diagnostic().WithLocation(6, 34),

                // InvalidStruct3
                Diagnostic().WithLocation(10, 34),
                Diagnostic().WithLocation(11, 27),

                // InvalidStruct4
                Diagnostic().WithLocation(13, 34),

                // InvalidStruct5
                Diagnostic().WithLocation(18, 27),

                // InvalidStruct6
                Diagnostic().WithLocation(21, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
