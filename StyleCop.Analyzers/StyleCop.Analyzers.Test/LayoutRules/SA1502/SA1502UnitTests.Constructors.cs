// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
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
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestValidEmptyConstructorAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter)
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyConstructorOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) { }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 31);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(3, 31);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on a single line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorWithBlockOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) 
    { int bar; }
}";

            var expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a constructor with its block on multiple lines will pass without diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorWithBlockStartOnSameLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) { 
        int bar; }
}";

            await this.VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty constructor with its block on the same line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyConstructorOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) { }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo(int parameter)
    {
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with its block on the same line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) { int bar; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo(int parameter)
    {
        int bar;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with its block on a single line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorWithBlockOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) 
    { int bar; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo(int parameter) 
    {
        int bar;
    }
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a constructor with lots of trivia will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestConstructorWithLotsOfTriviaCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public Foo(int parameter) /* TR1 */ { /* TR2 */ int bar; /* TR3 */ } /* TR4 */
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public Foo(int parameter) /* TR1 */
    { /* TR2 */
        int bar; /* TR3 */
    } /* TR4 */
}";

            await this.VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }
    }
}
