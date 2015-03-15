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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
