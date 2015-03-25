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
        /// Verifies that no diagnostics are reported for the valid destructors defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        [Fact]
        public async Task TestDestructorValid()
        {
            var testCode = @"public class Foo
{
    // Valid destructor #1
    public class TestClass1
    {
        ~TestClass1()
        {
        }
    }

    // Valid destructor #2
    public class TestClass2
    {
        ~TestClass2()
        {
            int x;
        }
    }

    // Valid destructor #3 (Valid only for SA1500)
    public class TestClass3
    {
        ~TestClass3() { }
    }

    // Valid destructor #4 (Valid only for SA1500)
    public class TestClass4
    {
        ~TestClass4() { int x; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid destructor definitions.
        /// </summary>
        [Fact]
        public async Task TestDestructorInvalid()
        {
            var testCode = @"public class Foo
{
    // Invalid destructor #1
    public class TestClass1
    {
        ~TestClass1() {
        }
    }

    // Invalid destructor #2
    public class TestClass2
    {
        ~TestClass2() {
            int x;
        }
    }

    // Invalid destructor #3
    public class TestClass3
    {
        ~TestClass3() {
            int x; }
    }

    // Invalid destructor #4
    public class TestClass4
    {
        ~TestClass4() { int x;
        }
    }

    // Invalid destructor #5
    public class TestClass5
    {
        ~TestClass5()
        {
            int x; }
    }

    // Invalid destructor #6
    public class TestClass6
    {
        ~TestClass6()
        { int x;
        }
    }

    // Invalid destructor #7
    public class TestClass7
    {
        ~TestClass7()
        { int x; }
    }
}";

            var expectedDiagnostics = new[]
            {
                // Invalid destructor #1
                this.CSharpDiagnostic().WithLocation(6, 23),
                // Invalid destructor #2
                this.CSharpDiagnostic().WithLocation(13, 23),
                // Invalid destructor #3
                this.CSharpDiagnostic().WithLocation(21, 23),
                this.CSharpDiagnostic().WithLocation(22, 20),
                // Invalid destructor #4
                this.CSharpDiagnostic().WithLocation(28, 23),
                // Invalid destructor #5
                this.CSharpDiagnostic().WithLocation(37, 20),
                // Invalid destructor #6
                this.CSharpDiagnostic().WithLocation(44, 9),
                // Invalid destructor #7
                this.CSharpDiagnostic().WithLocation(52, 9),
                this.CSharpDiagnostic().WithLocation(52, 18)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
