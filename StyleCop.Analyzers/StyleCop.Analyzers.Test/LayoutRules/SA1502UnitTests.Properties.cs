namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the properties part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that valid properties will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidProperties()
        {
            var testCode = @"public class Foo
{
    private bool b;

    public bool Bar
    {
        get { return true; }
        set { this.b = value; }
    }

    public bool Baz
    {
        get 
        { 
            return true; 
        }

        set 
        { 
            this.b = value; 
        }
    }

    public int AutoBar { get; set; } = 7;

    public int AutoBaz { get; } = 9;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a property with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestPropertyOnSingleLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar { get { return true; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 21);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a property with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestPropertyWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar
    { get { return true; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a property with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestPropertyWithBlockStartOnSameLine()
        {
            var testCode = @"public class Foo
{
    public bool Bar {
        get { return true; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a property with an expression body will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestPropertyWithExpressionBody()
        {
            var testCode = @"public class Foo
{
    private bool x;
    private bool y;

    public bool Bar => this.x ^ this.y;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix for a property with its block on the same line will work properly.
        /// </summary>
        [Fact]
        public async Task TestPropertyOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool Bar { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool Bar
    {
        get { return true; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a property with its block on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestPropertyWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool Bar
    { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool Bar
    {
        get { return true; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix for a property with lots of trivia is working properly.
        /// </summary>
        [Fact]
        public async Task TestPropertyWithLotsOfTriviaCodeFix()
        {
            var testCode = @"public class Foo
{
    public bool Bar /* TR1 */ { /* TR2 */ get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */ } /* TR11 */
}";
            var fixedTestCode = @"public class Foo
{
    public bool Bar /* TR1 */
    { /* TR2 */
        get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */
    } /* TR11 */
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }
    }
}
