namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the indexers part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that valid indexers will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidIndexers()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index]
    {
        get { return true; }
    }

    public bool this[string index]
    {
        get 
        { 
            return true; 
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an indexer defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestIndexerOnSingleLine()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] { get { return true; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 33);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an indexer with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestIndexerWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] 
    { get { return true; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an indexer with its block defined on a mutiple lines will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestIndexerWithBlockStartkOnSameLine()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] { 
    get { return true; } }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an indexer with an expression body will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestIndexerWithExpressionBody()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] => index > 0;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
