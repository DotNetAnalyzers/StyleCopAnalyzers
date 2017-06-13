// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;

    public class SA1502CSharp7UnitTests : SA1502UnitTests
    {
        /// <summary>
        /// Verifies that a valid local function will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEmptyLocalFunctionAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        void Bar()
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty local function with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLocalFunctionOnSingleLineAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        void Bar() { }
    }
}";

            var expected = this.CSharpDiagnostic().WithLocation(5, 20);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a local function with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionOnSingleLineAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() { return 0; }
    }
}";

            var expected = this.CSharpDiagnostic().WithLocation(5, 19);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a local function with its block on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionWithBlockOnSingleLineAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() 
        { return 0; }
    }
}";

            var expected = this.CSharpDiagnostic().WithLocation(6, 9);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a local function with its block on multiple lines will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionWithBlockStartOnSameLineAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() {
            return 0; }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a local function with an expression body will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionWithExpressionBodyAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar(int x, int y) => x + y;
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an empty local function with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyLocalFunctionOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        void Bar() { }
    }
}";
            var fixedTestCode = @"public class TypeName
{
    public void Method()
    {
        void Bar()
        {
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a local function with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() { return 0; }
    }
}";
            var fixedTestCode = @"public class TypeName
{
    public void Method()
    {
        int Bar()
        {
            return 0;
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a local function with its block on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() 
        { return 0; }
    }
}";
            var fixedTestCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() 
        {
            return 0;
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a local function with lots of trivia is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLocalFunctionWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() /* TR1 */ { /* TR2 */ return 0; /* TR3 */ } /* TR4 */
    }
}";
            var fixedTestCode = @"public class TypeName
{
    public void Method()
    {
        int Bar() /* TR1 */
        { /* TR2 */
            return 0; /* TR3 */
        } /* TR4 */
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
