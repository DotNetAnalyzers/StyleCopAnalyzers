// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    /// Unit tests for the destructors part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that a valid destructor will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidDestructorAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() 
    { 
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty destructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyDestructorOnSingleLineAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() { }
}";
            var expected = Diagnostic().WithLocation(3, 12);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with its block on the same line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorOnSingleLineAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() { int bar; }
}";

            var expected = Diagnostic().WithLocation(3, 12);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with its block on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithBlockOnSingleLineAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() 
    { int bar; }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a destructor with its block on a multiple lines will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithBlockStartOnSameLineAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() { 
        int bar; }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty destructor with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyDestructorOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() { }
}";
            var fixedTestCode = @"public class Foo
{
    ~Foo()
    {
    }
}";

            var expected = Diagnostic().WithLocation(3, 12);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix a destructor with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() { int bar; }
}";
            var fixedTestCode = @"public class Foo
{
    ~Foo()
    {
        int bar;
    }
}";

            var expected = Diagnostic().WithLocation(3, 12);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix a destructor with its block on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() 
    { int bar; }
}";
            var fixedTestCode = @"public class Foo
{
    ~Foo() 
    {
        int bar;
    }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix a destructor with lots of trivia will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDestructorWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    ~Foo() /* TR1 */ { /* TR2 */ int bar; /* TR3 */ } /* TR4 */
}";
            var fixedTestCode = @"public class Foo
{
    ~Foo() /* TR1 */
    { /* TR2 */
        int bar; /* TR3 */
    } /* TR4 */
}";

            var expected = Diagnostic().WithLocation(3, 22);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
