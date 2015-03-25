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
    public partial class SA1500UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid delegates defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestDelegateValid()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    private delegate void MyDelegate();

    private void TestMethod(MyDelegate d)
    {
    }

    private void Bar()
    {
        // Valid delegate #1
        MyDelegate item1 = delegate { };
        
        // Valid delegate #2
        MyDelegate item2 = delegate { Debug.Assert(true); };

        // Valid delegate #3
        MyDelegate item3 = delegate
        {
        };

        // Valid delegate #4
        MyDelegate item4 = delegate
        {
            Debug.Assert(true);
        };

        // Valid delegate #5
        this.TestMethod(delegate { });

        // Valid delegate #6
        this.TestMethod(delegate 
        { 
        });

        // Valid delegate #7
        this.TestMethod(delegate { Debug.Assert(true); });

        // Valid delegate #8
        this.TestMethod(delegate 
        { 
            Debug.Assert(true); 
        });
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid delegate definitions.
        /// </summary>
        [Fact(Skip = "Disabled until the SA1500 implementation is available")]
        public async Task TestDelegateInvalid()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    private delegate void MyDelegate();

    private void TestMethod(MyDelegate d)
    {
    }

    private void Bar()
    {
        // Invalid delegate #1
        MyDelegate item1 = delegate {
        };
        
        // Invalid delegate #2
        MyDelegate item2 = delegate {
            Debug.Assert(true); 
        };

        // Invalid delegate #3
        MyDelegate item3 = delegate {
            Debug.Assert(true); };

        // Invalid delegate #4
        MyDelegate item4 = delegate { Debug.Assert(true); 
        };

        // Invalid delegate #5
        MyDelegate item5 = delegate 
        {
            Debug.Assert(true); };

        // Invalid delegate #6
        MyDelegate item6 = delegate 
        { Debug.Assert(true); 
        };

        // Invalid delegate #7
        MyDelegate item7 = delegate 
        { Debug.Assert(true); };

        // Invalid delegate #8
        this.TestMethod(delegate {
        });

        // Invalid delegate #9
        this.TestMethod(delegate {
            Debug.Assert(true);
        });

        // Invalid delegate #10
        this.TestMethod(delegate {
            Debug.Assert(true); });

        // Invalid delegate #11
        this.TestMethod(delegate { Debug.Assert(true);
        });

        // Invalid delegate #12
        this.TestMethod(delegate 
        { 
            Debug.Assert(true); });

        // Invalid delegate #13
        this.TestMethod(delegate 
        { Debug.Assert(true); 
        });

        // Invalid delegate #14
        this.TestMethod(delegate 
        { Debug.Assert(true); });
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid delegate #1
                this.CSharpDiagnostic().WithLocation(14, 37),
                // Invalid delegate #2
                this.CSharpDiagnostic().WithLocation(18, 37),
                // Invalid delegate #3
                this.CSharpDiagnostic().WithLocation(23, 37),
                this.CSharpDiagnostic().WithLocation(24, 33),
                // Invalid delegate #4
                this.CSharpDiagnostic().WithLocation(27, 37),
                // Invalid delegate #5
                this.CSharpDiagnostic().WithLocation(33, 33),
                // Invalid delegate #6
                this.CSharpDiagnostic().WithLocation(37, 9),
                // Invalid delegate #7
                this.CSharpDiagnostic().WithLocation(42, 9),
                this.CSharpDiagnostic().WithLocation(42, 31),
                // Invalid delegate #8
                this.CSharpDiagnostic().WithLocation(45, 34),
                // Invalid delegate #9
                this.CSharpDiagnostic().WithLocation(49, 34),
                // Invalid delegate #10
                this.CSharpDiagnostic().WithLocation(54, 34),
                this.CSharpDiagnostic().WithLocation(55, 33),
                // Invalid delegate #11
                this.CSharpDiagnostic().WithLocation(58, 34),
                // Invalid delegate #12
                this.CSharpDiagnostic().WithLocation(64, 33),
                // Invalid delegate #13
                this.CSharpDiagnostic().WithLocation(68, 9),
                // Invalid delegate #14
                this.CSharpDiagnostic().WithLocation(73, 9),
                this.CSharpDiagnostic().WithLocation(73, 31)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}