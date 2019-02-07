// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1024ColonsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1024UnitTests
    {
        private const string ExpectedCode = @"using System;

public class Foo<T> : object where T : IFormattable
{
    public Foo()/* test */ : base()
    {
    }
    public Foo(int x) : this()
    {
        Bar(value: x > 2 ? 2 : 3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

        /// <summary>
        /// Verifies that the analyzer will not produce the proper diagnostics when the colons spaced correctly on same line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacedColonsOnSameLineAsync()
        {
            const string testCode = @"using System;

public class Foo<T> : object where T/* test */ : IFormattable
{
    public Foo() : base()
    {
    }
    public Foo(int x) : this()
    {
        Bar(value: x > 2 ? 2 : 3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce the proper diagnostics when the colons is the first or last character on the line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacedColonsOnMultipleLinesAsync()
        {
            const string testCode = @"using System;

public class Foo<T> :
object where T
: IFormattable
{
    public Foo() :
base()
    {
    }
    public Foo(int x)
: this()
    {
        Bar(value: x > 2 ? 2
                         : 3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the colons not followed by space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacedColonsMustBeFollowedAsync()
        {
            const string testCode = @"using System;

public class Foo<T> :object where T :IFormattable
{
    public Foo()/* test */ :base()
    {
    }
    public Foo(int x) :this()
    {
        Bar(value: x > 2 ? 2 :3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 21).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(3, 37).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(5, 28).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(8, 23).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(10, 30).WithArguments(string.Empty, "followed", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the colons not preceded by space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacedColonsMustBePrecededAsync()
        {
            const string testCode = @"using System;

public class Foo<T>: object where T: IFormattable
{
    public Foo()/* test */: base()
    {
    }
    public Foo(int x): this()
    {
        Bar(value: x > 2 ? 2: 3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 20).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(3, 36).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(5, 27).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(8, 22).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(10, 29).WithArguments(string.Empty, "preceded", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the colons should not preceded by space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacedColonsMustNotBePrecededAsync()
        {
            const string testCode = @"using System;

public class Foo<T> : object where T : IFormattable
{
    public Foo()/* test */ : base()
    {
    }
    public Foo(int x) : this()
    {
        Bar(value : x > 2 ? 2 : 3);
    }

    private int Bar(int value)
    {
    _label :
        switch (value)
        {
            case 2:
            case 3 :
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3 :N}"");
                return (int)Convert.ToDouble($""{3 : N}"");
            default :
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 19).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(15, 12).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(19, 20).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(22, 51).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(23, 51).WithArguments(" not", "preceded", string.Empty),
                Diagnostic().WithLocation(24, 21).WithArguments(" not", "preceded", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the colons not preceded and not followed by space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacedColonsMustBePrecededAndFollowedAsync()
        {
            const string testCode = @"using System;

public class Foo<T>:object where T:IFormattable
{
    public Foo()/* test */:base()
    {
    }
    public Foo(int x):this()
    {
        Bar(value: x > 2 ? 2:3);
    }

    private int Bar(int value)
    {
    _label:
        switch (value)
        {
            case 2:
            case 3:
                return value;
            case 4:
                return (int)Convert.ToDouble($""{3:N}"");
                return (int)Convert.ToDouble($""{3: N}"");
            default:
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 20).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(3, 20).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(3, 35).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(3, 35).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(5, 27).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(5, 27).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(8, 22).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(8, 22).WithArguments(string.Empty, "followed", string.Empty),
                Diagnostic().WithLocation(10, 29).WithArguments(string.Empty, "preceded", string.Empty),
                Diagnostic().WithLocation(10, 29).WithArguments(string.Empty, "followed", string.Empty),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode).ConfigureAwait(false);
        }
    }
}
