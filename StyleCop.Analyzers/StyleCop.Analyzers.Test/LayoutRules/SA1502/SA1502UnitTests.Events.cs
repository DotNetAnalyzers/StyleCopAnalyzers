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
    /// Unit tests for the events part of <see cref="SA1502ElementMustNotBeOnASingleLine"/>.
    /// </summary>
    public partial class SA1502UnitTests
    {
        /// <summary>
        /// Verifies that correct events will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidEventAsync()
        {
            var testCode = @"using System;

public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar
    {
        add { x += value; }
        remove { x -= value; }
    }

    public event EventHandler Baz
    {
        add 
        { 
            x += value; 
        }

        remove 
        { 
            x -= value; 
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventOnSingleLineAsync()
        {
            var testCode = @"using System;

public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar { add { x += value; } remove { x -= value; } }
}";

            var expected = Diagnostic().WithLocation(7, 35);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event with its block defined on a single line will trigger a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventWithBlockOnSingleLineAsync()
        {
            var testCode = @"using System;

public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar
    { add { x += value; } remove { x -= value; } }
}";

            var expected = Diagnostic().WithLocation(8, 5);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an event with its block defined on a multiple lines will pass without diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventWithBlockStartkOnSameLineAsync()
        {
            var testCode = @"using System;

public class Foo 
{
    private EventHandler x;

    public event EventHandler Bar {
        add { x += value; } 
        remove { x -= value; } }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event block on the same line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventOnSingleLineCodeFixAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar { add { x += value; } remove { x -= value; } }
}";
            var fixedTestCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar
    {
        add { x += value; } remove { x -= value; }
    }
}";

            var expected = Diagnostic().WithLocation(7, 35);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event with its block on a single line will work properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventWithBlockOnSingleLineCodeFixAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar 
    { add { x += value; } remove { x -= value; } }
}";
            var fixedTestCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar 
    {
        add { x += value; } remove { x -= value; }
    }
}";

            var expected = Diagnostic().WithLocation(8, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix for an event with lots of trivia is working properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEventWithLotsOfTriviaCodeFixAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar /* TR1 */ { /* TR2 */ add /* TR3 */ { /* TR4 */ x += value; /* TR5 */ } /* TR6 */ remove /* TR7 */ { /* TR8 */ x -= value; /* TR9 */ } /* TR10 */ } /* TR11 */
}";
            var fixedTestCode = @"using System;

public class Foo
{
    private EventHandler x;

    public event EventHandler Bar /* TR1 */
    { /* TR2 */
        add /* TR3 */ { /* TR4 */ x += value; /* TR5 */ } /* TR6 */ remove /* TR7 */ { /* TR8 */ x -= value; /* TR9 */ } /* TR10 */
    } /* TR11 */
}";

            var expected = Diagnostic().WithLocation(7, 45);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }
    }
}
