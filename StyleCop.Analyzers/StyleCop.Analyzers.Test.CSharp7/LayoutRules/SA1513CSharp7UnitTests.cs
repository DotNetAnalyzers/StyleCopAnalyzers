// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1513ClosingBraceMustBeFollowedByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1513CodeFixProvider>;

    public class SA1513CSharp7UnitTests : SA1513UnitTests
    {
        /// <summary>
        /// Verifies that all valid usages of a closing brace in new C# 7 syntax without a following blank line will
        /// report no diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidCSharp7Async()
        {
            var testCode = @"using System;

public class Foo
{
    public void Baz()
    {
        // Valid #1
        int LocalFunction(int value)
        {
            return value * 2;
        }
    }

    public Func<int, int> Quux()
    {
        // Valid #2
#if SOMETHING
        return null;
#else
        return LocalFunction;

        int LocalFunction(int value)
        {
            return value * 2;
        }
#endif
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that all invalid usages of a closing brace in new C# 7 syntax without a following blank line will
        /// report a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidCSharp7Async()
        {
            var testCode = @"
public class Foo
{
    private int x;

    public void Baz()
    {
        // Invalid #1
        switch (new object())
        {
            case int y:
            {
                this.x = 1;
                break;
            }
            case short z:
                this.x = 2;
                break;
        }

        // Invalid #2, #3
        void LocalFunction1()
        {
        }
        void LocalFunction2()
        {
        }
        return;
    }
}
";
            var fixedCode = @"
public class Foo
{
    private int x;

    public void Baz()
    {
        // Invalid #1
        switch (new object())
        {
            case int y:
            {
                this.x = 1;
                break;
            }

            case short z:
                this.x = 2;
                break;
        }

        // Invalid #2, #3
        void LocalFunction1()
        {
        }

        void LocalFunction2()
        {
        }

        return;
    }
}
";

            var expected = new[]
            {
                // Invalid #1
                Diagnostic().WithLocation(15, 14),

                // Invalid #2, #3
                Diagnostic().WithLocation(24, 10),
                Diagnostic().WithLocation(27, 10),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
