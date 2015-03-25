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
            var testCode = @"public class Foo
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
        MyDelegate item2 = delegate { int x; };

        // Valid delegate #3
        MyDelegate item3 = delegate
        {
        };

        // Valid delegate #4
        MyDelegate item4 = delegate
        {
            int x;
        };

        // Valid delegate #5
        this.TestMethod(delegate { });

        // Valid delegate #6
        this.TestMethod(delegate 
        { 
        });

        // Valid delegate #7
        this.TestMethod(delegate { int x; });

        // Valid delegate #8
        this.TestMethod(delegate 
        { 
            int x; 
        });
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid delegate definitions.
        /// </summary>
        [Fact]
        public async Task TestDelegateInvalid()
        {
            var testCode = @"public class Foo
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
            int x; 
        };

        // Invalid delegate #3
        MyDelegate item3 = delegate {
            int x; };

        // Invalid delegate #4
        MyDelegate item4 = delegate { int x; 
        };

        // Invalid delegate #5
        MyDelegate item5 = delegate 
        {
            int x; };

        // Invalid delegate #6
        MyDelegate item6 = delegate 
        { int x; 
        };

        // Invalid delegate #7
        MyDelegate item7 = delegate 
        { int x; };

        // Invalid delegate #8
        this.TestMethod(delegate {
        });

        // Invalid delegate #9
        this.TestMethod(delegate {
            int x;
        });

        // Invalid delegate #10
        this.TestMethod(delegate {
            int x; });

        // Invalid delegate #11
        this.TestMethod(delegate { int x;
        });

        // Invalid delegate #12
        this.TestMethod(delegate 
        { 
            int x; });

        // Invalid delegate #13
        this.TestMethod(delegate 
        { int x; 
        });

        // Invalid delegate #14
        this.TestMethod(delegate 
        { int x; });
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid delegate #1
                this.CSharpDiagnostic().WithLocation(12, 37),
                // Invalid delegate #2
                this.CSharpDiagnostic().WithLocation(16, 37),
                // Invalid delegate #3
                this.CSharpDiagnostic().WithLocation(21, 37),
                this.CSharpDiagnostic().WithLocation(22, 20),
                // Invalid delegate #4
                this.CSharpDiagnostic().WithLocation(25, 37),
                // Invalid delegate #5
                this.CSharpDiagnostic().WithLocation(31, 20),
                // Invalid delegate #6
                this.CSharpDiagnostic().WithLocation(35, 9),
                // Invalid delegate #7
                this.CSharpDiagnostic().WithLocation(40, 9),
                this.CSharpDiagnostic().WithLocation(40, 18),
                // Invalid delegate #8
                this.CSharpDiagnostic().WithLocation(43, 34),
                // Invalid delegate #9
                this.CSharpDiagnostic().WithLocation(47, 34),
                // Invalid delegate #10
                this.CSharpDiagnostic().WithLocation(52, 34),
                this.CSharpDiagnostic().WithLocation(53, 20),
                // Invalid delegate #11
                this.CSharpDiagnostic().WithLocation(56, 34),
                // Invalid delegate #12
                this.CSharpDiagnostic().WithLocation(62, 20),
                // Invalid delegate #13
                this.CSharpDiagnostic().WithLocation(66, 9),
                // Invalid delegate #14
                this.CSharpDiagnostic().WithLocation(71, 9),
                this.CSharpDiagnostic().WithLocation(71, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}