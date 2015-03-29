﻿namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid structs defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
        /// </remarks>
        [Fact]
        public async Task TestStructValid()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid struct definitions.
        /// </summary>
        /// <remarks>
        /// These will normally also report SA1401, but not in the unit test.
        /// </remarks>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestStructInvalid()
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

            var expectedDiagnostics = new[]
            {
                // InvalidStruct1
                this.CSharpDiagnostic().WithLocation(3, 34),
                // InvalidStruct2
                this.CSharpDiagnostic().WithLocation(6, 34),
                // InvalidStruct3
                this.CSharpDiagnostic().WithLocation(10, 34),
                this.CSharpDiagnostic().WithLocation(11, 27),
                // InvalidStruct4
                this.CSharpDiagnostic().WithLocation(13, 34),
                // InvalidStruct5
                this.CSharpDiagnostic().WithLocation(18, 27),
                // InvalidStruct6
                this.CSharpDiagnostic().WithLocation(21, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}