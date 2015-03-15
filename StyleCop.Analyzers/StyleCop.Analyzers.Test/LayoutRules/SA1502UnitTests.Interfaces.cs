namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the interfaces part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty interface will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyInterface()
        {
            var testCode = @"public interface IFoo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty interface defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyInterfaceOnSingleLine()
        {
            var testCode = "public interface IFoo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an interface definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestInterfaceOnSingleLine()
        {
            var testCode = "public interface IFoo { void Bar(); }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an interface with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestInterfaceWithBlockOnSingleLine()
        {
            var testCode = @"public interface IFoo
{ void Bar(); }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an interface definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestInterfaceWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo { 
void Bar(); }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
