// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

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
    /// Unit tests for the interfaces part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty interface defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyInterfaceOnSingleLineAsync()
        {
            var testCode = "public interface IFoo { }";

            var expected = Diagnostic().WithLocation(1, 23);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an interface definition on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterfaceOnSingleLineAsync()
        {
            var testCode = "public interface IFoo { void Bar(); }";

            var expected = Diagnostic().WithLocation(1, 23);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(1, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(1, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(1, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(1, 32);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
