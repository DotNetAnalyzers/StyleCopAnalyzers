namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the events part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that correct events will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestValidEvent()
        {
            var testCode = @"public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar
    {
        add { x += value; }
        remove { x -= value; }
    }

    public event EventHandler Baz
    {
        add 
        { 
            x += value; 
        }

        remove 
        { 
            x -= value; 
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEventOnSingleLine()
        {
            var testCode = @"public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar { add { x += value; } remove { x -= value; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(5, 35);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEventWithBlockOnSingleLine()
        {
            var testCode = @"public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar
    { add { x += value; } remove { x -= value; } }
}";

            var expected = this.CSharpDiagnostic().WithLocation(6, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event with its block defined on a mutiple lines will pass without diagnostic.
        /// </summary>
        [Fact]
        public async Task TestEventWithBlockStartkOnSameLine()
        {
            var testCode = @"public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar {
        add { x += value; } 
        remove { x -= value; } }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event block on the same line will work properly.
        /// </summary>
        [Fact]
        public async Task TestEventOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar { add { x += value; } remove { x -= value; } }
}";
            var fixedTestCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar
    {
        add { x += value; } remove { x -= value; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event with its block on a single line will work properly.
        /// </summary>
        [Fact]
        public async Task TestEventWithBlockOnSingleLineCodeFix()
        {
            var testCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar 
    { add { x += value; } remove { x -= value; } }
}";
            var fixedTestCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar
    {
        add { x += value; } remove { x -= value; }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event with lots of trivia is working properly.
        /// </summary>
        [Fact]
        public async Task TestEventWithLotsOfTriviaCodeFix()
        {
            var testCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar /* TR1 */ { /* TR2 */ add /* TR3 */ { /* TR4 */ x += value; /* TR5 */ } /* TR6 */ remove /* TR7 */ { /* TR8 */ x -= value; /* TR9 */ } /* TR10 */ } /* TR11 */
}";
            var fixedTestCode = @"public class Foo
{
    private EventHandler x;

    public event EventHandler Bar /* TR1 */
    { /* TR2 */
        add /* TR3 */ { /* TR4 */ x += value; /* TR5 */ } /* TR6 */ remove /* TR7 */ { /* TR8 */ x -= value; /* TR9 */ } /* TR10 */
    } /* TR11 */
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
