// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1502ElementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1502CodeFixProvider>;

    /// <summary>
    /// Unit tests for the methods part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that a valid method will pass without diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestValidEmptyMethodAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty method with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyMethodOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar() { }
}";

            var expected = Diagnostic().WithLocation(3, 23);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() { return false; }
}";

            var expected = Diagnostic().WithLocation(3, 23);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on a single line will trigger a diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodWithBlockOnSingleLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() 
    { return false; }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with its block on multiple lines will pass without diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodWithBlockStartOnSameLineAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() {
        return false; }
}";

            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a method with an expression body will pass without diagnostic.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodWithExpressionBodyAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar(int x, int y) => x > y;
}";

            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, elementType), DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty method with its block on the same line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestEmptyMethodOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public void Bar() { }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public void Bar()
    {
    }
}";

            var expected = Diagnostic().WithLocation(3, 23);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), expected, FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a method with its block on the same line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() { return false; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar()
    {
        return false;
    }
}";

            var expected = Diagnostic().WithLocation(3, 23);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), expected, FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a method with its block on a single line will work properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodWithBlockOnSingleLineCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() 
    { return false; }
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar() 
    {
        return false;
    }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), expected, FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a property with lots of trivia is working properly.
        /// </summary>
        /// <param name="elementType">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("class")]
        [InlineData("struct")]
        public async Task TestMethodWithLotsOfTriviaCodeFixAsync(string elementType)
        {
            var testCode = @"public ##PH## Foo
{
    public bool Bar() /* TR1 */ { /* TR2 */ return false; /* TR3 */ } /* TR4 */
}";
            var fixedTestCode = @"public ##PH## Foo
{
    public bool Bar() /* TR1 */
    { /* TR2 */
        return false; /* TR3 */
    } /* TR4 */
}";

            var expected = Diagnostic().WithLocation(3, 33);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, elementType), expected, FormatTestCode(fixedTestCode, elementType)).ConfigureAwait(false);
        }
    }
}
