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
    /// Unit tests for the namespaces part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty namespace will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEmptyNamespaceAsync()
        {
            var testCode = @"namespace Foo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty namespace defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyNamespaceOnSingleLineAsync()
        {
            var testCode = @"namespace Foo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 15);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a namespace defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceOnSingleLineAsync()
        {
            var testCode = @"namespace Foo { using System; }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 15);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a namespace with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceWithBlockOnSingleLineAsync()
        {
            var testCode = @"namespace Foo 
{ using System; }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a namespace with its block defined on a multiple lines will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceWithBlockStartkOnSameLineAsync()
        {
            var testCode = @"namespace Foo {
    using System;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty namespace defined on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyNamespaceOnSingleLineCodeFixAsync()
        {
            var testCode = @"namespace Foo { }";
            var fixedTestCode = @"namespace Foo
{
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a namespace defined on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceOnSingleLineCodeFixAsync()
        {
            var testCode = @"namespace Foo { using System; }";
            var fixedTestCode = @"namespace Foo
{
    using System;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a namespace with its block defined on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"namespace Foo
{ using System; }";
            var fixedTestCode = @"namespace Foo
{
    using System;
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for a namespace defined with lots of trivia will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"namespace Foo /* TR1 */ { /* TR2 */ using System; /* TR3 */ } /* TR4 */";
            var fixedTestCode = @"namespace Foo /* TR1 */
{ /* TR2 */
    using System; /* TR3 */
} /* TR4 */
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
