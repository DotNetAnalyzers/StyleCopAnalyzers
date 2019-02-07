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
    /// Unit tests for the enums part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that a correct empty enum will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEmptyEnumAsync()
        {
            var testCode = @"public enum Foo 
{
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty enum defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyEnumOnSingleLineAsync()
        {
            var testCode = "public enum Foo { }";

            var expected = Diagnostic().WithLocation(1, 17);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum definition on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumOnSingleLineAsync()
        {
            var testCode = "public enum Foo { Value1 }";

            var expected = Diagnostic().WithLocation(1, 17);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumWithBlockOnSingleLineAsync()
        {
            var testCode = @"public enum Foo
{ Value1 }";

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an enum definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumWithBlockStartOnSameLineAsync()
        {
            var testCode = @"public enum Foo { 
Value1 }";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an empty enum defined on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptyEnumOnSingleLineCodeFixAsync()
        {
            var testCode = "public enum Foo { }";
            var fixedTestCode = @"public enum Foo
{
}
";

            var expected = Diagnostic().WithLocation(1, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumOnSingleLineCodeFixAsync()
        {
            var testCode = "public enum Foo { Value1 }";
            var fixedTestCode = @"public enum Foo
{
    Value1
}
";

            var expected = Diagnostic().WithLocation(1, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition with multiple values on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultiValueEnumOnSingleLineCodeFixAsync()
        {
            var testCode = "public enum Foo { Value1, Value2 }";
            var fixedTestCode = @"public enum Foo
{
    Value1, Value2
}
";

            var expected = Diagnostic().WithLocation(1, 17);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum with its block defined on a single line will properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public enum Foo
{ Value1 }";
            var fixedTestCode = @"public enum Foo
{
    Value1
}
";

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix for an enum definition with lots of trivia will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEnumWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = "public enum Foo /* TR1 */ { /* TR2 */ Value1, /* TR3 */ Value2 /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public enum Foo /* TR1 */
{ /* TR2 */
    Value1, /* TR3 */ Value2 /* TR4 */
} /* TR5 */
";

            var expected = Diagnostic().WithLocation(1, 27);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }
    }
}
