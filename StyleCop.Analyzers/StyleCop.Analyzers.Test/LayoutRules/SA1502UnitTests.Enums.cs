namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the enums part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty enum will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyEnum()
        {
            var testCode = @"public enum Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty enum defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyEnumOnSingleLine()
        {
            var testCode = "public enum Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an enum definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumOnSingleLine()
        {
            var testCode = "public enum Foo { Value1 }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 17);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an enum with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumWithBlockOnSingleLine()
        {
            var testCode = @"public enum Foo
{ Value1 }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an enum definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEnumWithBlockStartOnSameLine()
        {
            var testCode = @"public enum Foo { 
Value1 }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
