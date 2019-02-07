﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
    /// Unit tests for the indexers part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that valid indexers will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidIndexersAsync()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index]
    {
        get { return true; }
    }

    public bool this[string index]
    {
        get 
        { 
            return true; 
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an indexer defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerOnSingleLineAsync()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] { get { return true; } }
}";

            var expected = Diagnostic().WithLocation(3, 33);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an indexer with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerWithBlockOnSingleLineAsync()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] 
    { get { return true; } }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an indexer with its block defined on a multiple lines will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerWithBlockStartkOnSameLineAsync()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] { 
    get { return true; } }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an indexer with an expression body will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerWithExpressionBodyAsync()
        {
            var testCode = @"public class Foo 
{
    public bool this[int index] => index > 0;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an indexer with its block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public bool this[int index] { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index]
    {
        get { return true; }
    }
}";

            var expected = Diagnostic().WithLocation(3, 33);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an indexer with its block on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public bool this[int index]
    { get { return true; } }
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index]
    {
        get { return true; }
    }
}";

            var expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an indexer with lots of trivia is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIndexerWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"public class Foo
{
    public bool this[int index] /* TR1 */ { /* TR2 */ get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */ } /* TR11 */
}";
            var fixedTestCode = @"public class Foo
{
    public bool this[int index] /* TR1 */
    { /* TR2 */
        get /* TR3 */ { /* TR4 */ return true; /* TR5 */ } /* TR6 */ set /* TR7 */ { /* TR8 */ throw new System.InvalidOperationException(); /* TR9 */ } /* TR10 */
    } /* TR11 */
}";

            var expected = Diagnostic().WithLocation(3, 43);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }
    }
}
