// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1502ElementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1502CodeFixProvider>;

    /// <summary>
    /// Unit tests for the type declaration part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        public static IEnumerable<object[]> TokensToTest
        {
            get
            {
                yield return new[] { "class" };
                yield return new[] { "struct" };
            }
        }

        /// <summary>
        /// Verifies that a correct empty type will pass without diagnostic.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestValidEmptyTypeAsync(string token)
        {
            var testCode = @"public ##PH## Foo 
{
}";

            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, token), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an empty type defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestEmptyTypeOnSingleLineAsync(string token)
        {
            var testCode = "public ##PH## Foo { }";

            var expected = Diagnostic().WithLocation(1, 13 + token.Length);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, token), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type definition on a single line will trigger a diagnostic.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeOnSingleLineAsync(string token)
        {
            var testCode = "public ##PH## Foo { private int bar; }";

            var expected = Diagnostic().WithLocation(1, 13 + token.Length);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, token), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeWithBlockOnSingleLineAsync(string token)
        {
            var testCode = @"public ##PH## Foo
{ private int bar; }";

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, token), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type definition with only the block start on the same line will pass without diagnostic.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeWithBlockStartOnSameLineAsync(string token)
        {
            var testCode = @"public ##PH## Foo { 
    private int bar; 
}";

            await VerifyCSharpDiagnosticAsync(FormatTestCode(testCode, token), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an empty type element is working properly.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestEmptyTypeOnSingleLineCodeFixAsync(string token)
        {
            var testCode = "public ##PH## Foo { }";
            var fixedTestCode = @"public ##PH## Foo
{
}
";

            var expected = Diagnostic().WithLocation(1, 13 + token.Length);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, token), expected, FormatTestCode(fixedTestCode, token), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a type with a statement is working properly.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeOnSingleLineCodeFixAsync(string token)
        {
            var testCode = "public ##PH## Foo { private int bar; }";
            var fixedTestCode = @"public ##PH## Foo
{
    private int bar;
}
";

            var expected = Diagnostic().WithLocation(1, 13 + token.Length);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, token), expected, FormatTestCode(fixedTestCode, token), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a type with multiple statements is working properly.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeOnSingleLineWithMultipleStatementsCodeFixAsync(string token)
        {
            var testCode = "public ##PH## Foo { private int bar; private bool baz; }";
            var fixedTestCode = @"public ##PH## Foo
{
    private int bar; private bool baz;
}
";

            var expected = Diagnostic().WithLocation(1, 13 + token.Length);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, token), expected, FormatTestCode(fixedTestCode, token), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a type with its block defined on a single line is working properly.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeWithBlockOnSingleLineCodeFixAsync(string token)
        {
            var testCode = @"public ##PH## Foo
{ private int bar; }";
            var fixedTestCode = @"public ##PH## Foo
{
    private int bar;
}
";

            var expected = Diagnostic().WithLocation(2, 1);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, token), expected, FormatTestCode(fixedTestCode, token), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for a type with lots of trivia is working properly.
        /// </summary>
        /// <param name="token">The type of element to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TokensToTest))]
        public async Task TestTypeWithLotsOfTriviaCodeFixAsync(string token)
        {
            var testCode = @"public ##PH## Foo /* TR1 */ { /* TR2 */ private int bar; /* TR3 */ private int baz; /* TR4 */ } /* TR5 */";
            var fixedTestCode = @"public ##PH## Foo /* TR1 */
{ /* TR2 */
    private int bar; /* TR3 */ private int baz; /* TR4 */
} /* TR5 */
";

            var expected = Diagnostic().WithLocation(1, 23 + token.Length);
            await VerifyCSharpFixAsync(FormatTestCode(testCode, token), expected, FormatTestCode(fixedTestCode, token), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
