namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the constructors part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a valid constructor will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestValidEmptyConstructor(string elementType)
        {
            var testCodeFormat = @"public {0} Foo
{
    public Foo()
    {
    }
}";
            var testCode = string.Format(CultureInfo.InvariantCulture, testCodeFormat, elementType);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that an empty constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestEmptyConstructorOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public {0} Foo
{
    public Foo() { }
}";
            var testCode = string.Format(CultureInfo.InvariantCulture, testCodeFormat, elementType);

            var expected = this.CSharpDiagnostic().WithLocation(3, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public {0} Foo
{
    public Foo() { int bar; }
}";
            var testCode = string.Format(CultureInfo.InvariantCulture, testCodeFormat, elementType);

            var expected = this.CSharpDiagnostic().WithLocation(3, 18);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a constructor with its block on a single line will trigger a diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithBlockOnSingleLine(string elementType)
        {
            var testCodeFormat = @"public {0} Foo
{
    public Foo() 
    { int bar; }
}";
            var testCode = string.Format(CultureInfo.InvariantCulture, testCodeFormat, elementType);

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a constructor with its block on multiple lines will pass without diagnostic.
        /// </summary>
        [Theory, InlineData("class"), InlineData("struct")]
        public async Task TestConstructorWithBlockStartOnSameLine(string elementType)
        {
            var testCodeFormat = @"public {0} Foo
{
    public Foo() { 
        int bar; }
}";
            var testCode = string.Format(CultureInfo.InvariantCulture, testCodeFormat, elementType);

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }
    }
}
