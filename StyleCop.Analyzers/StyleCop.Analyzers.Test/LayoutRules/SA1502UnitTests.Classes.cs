namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the classes part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty class will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyClass()
        {
            var testCode = @"public class Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty class defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyClassOnSingleLine()
        {
            var testCode = "public class Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a class definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassOnSingleLine()
        {
            var testCode = "public class Foo { private int bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a class with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo
{ private int bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a class definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestClassWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo { 
    private int bar; 
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
