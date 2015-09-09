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
    /// Unit tests for the interfaces part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that a correct empty interface will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEmptyInterfaceAsync()
        {
            var testCode = @"public interface IFoo 
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty interface defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyInterfaceOnSingleLineAsync()
        {
            var testCode = "public interface IFoo { }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an interface definition on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceOnSingleLineAsync()
        {
            var testCode = "public interface IFoo { void Bar(); }";

            var expected = this.CSharpDiagnostic().WithLocation(1, 23);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an interface with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceWithBlockOnSingleLineAsync()
        {
            var testCode = @"public interface IFoo
{ void Bar(); }";

            var expected = this.CSharpDiagnostic().WithLocation(2, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an interface definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceWithBlockStartOnSameLineAsync()
        {
            var testCode = @"public interface Foo { 
void Bar(); }";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an empty interface element is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyInterfaceOnSingleLineCodeFixAsync()
        {
            var testCode = "public interface Foo { }";
            var fixedTestCode = @"public interface Foo
{
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an interface with a statement is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceOnSingleLineCodeFixAsync()
        {
            var testCode = "public interface Foo { void Bar(); }";
            var fixedTestCode = @"public interface Foo
{
    void Bar();
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an interface with multiple statements is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceOnSingleLineWithMultipleStatementsCodeFixAsync()
        {
            var testCode = "public interface Foo { void Bar(); void Baz(); }";
            var fixedTestCode = @"public interface Foo
{
    void Bar(); void Baz();
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an interface with its block defined on a single line is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public interface Foo
{ void Bar(); }";
            var fixedTestCode = @"public interface Foo
{
    void Bar();
}
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an interface with lots of trivia is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"public interface Foo /* TR1 */ { /* TR2 */ void Bar(); /* TR3 */ void Baz(); /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public interface Foo /* TR1 */
{ /* TR2 */
    void Bar(); /* TR3 */ void Baz(); /* TR4 */
} /* TR5 */
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
