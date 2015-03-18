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

        /// <summary>
        /// Verifies that the code fix for an indexer with its block on the same line will work properly.
        /// </summary>
        [Fact]
        public async Task TestIndexerOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool this[int index] { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index]
    {
        get { return true; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for an indexer with its block on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestIndexerWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool this[int index]
    { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index]
    {
        get { return true; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for an indexer with lots of trivia is working properly.
        /// </summary>
        [Fact]
        public async Task TestIndexerWithLotsOfTriviaCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool this[int index] /* TR1 */ { /* TR2 */ get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */ } /* TR11 */
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index] /* TR1 */
    { /* TR2 */
        get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */
    } /* TR11 */
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }
    }
}
