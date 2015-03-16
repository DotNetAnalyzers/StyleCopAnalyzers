namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the destructors part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a valid destructor will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidDestructor()
        {
            var testCode = @"public class Foo
{
    ~Foo() 
    { 
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty destructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyDestructorOnSingleLine()
        {
            var testCode = @"public class Foo
{
    ~Foo() { }
}";
            var expected = this.CSharpDiagnostic().WithLocation(3, 12);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a destructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestDestructorOnSingleLine()
        {
            var testCode = @"public class Foo
{
    ~Foo() { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 12);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a destructor with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestDestructorWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo
{
    ~Foo() 
    { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a destructor with its block on a multiple lines will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestDestructorWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo
{
    ~Foo() { 
        int bar; }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
