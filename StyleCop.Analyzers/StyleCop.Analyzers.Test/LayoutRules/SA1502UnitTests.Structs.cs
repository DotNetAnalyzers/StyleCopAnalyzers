namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the structs part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty class will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEmptyStruct()
        {
            var testCode = @"public struct Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty struct defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEmptyStructOnSingleLine()
        {
            var testCode = "public struct Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 19);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct definition on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructOnSingleLine()
        {
            var testCode = "public struct Foo { public int Bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 19);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructWithBlockOnSingleLine()
        {
            var testCode = @"public struct Foo
{ public int Bar; }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a struct definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestStructWithBlockStartOnSameLine()
        {
            var testCode = @"public struct Foo { 
    public int Bar; 
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
