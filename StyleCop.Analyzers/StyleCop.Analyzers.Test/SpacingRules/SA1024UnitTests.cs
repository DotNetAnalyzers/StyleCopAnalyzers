// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorFollowed).WithLocation(3, 21),
                Diagnostic(DescriptorFollowed).WithLocation(3, 37),
                Diagnostic(DescriptorFollowed).WithLocation(5, 28),
                Diagnostic(DescriptorFollowed).WithLocation(8, 23),
                Diagnostic(DescriptorFollowed).WithLocation(10, 30),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPreceded).WithLocation(3, 20),
                Diagnostic(DescriptorPreceded).WithLocation(3, 36),
                Diagnostic(DescriptorPreceded).WithLocation(5, 27),
                Diagnostic(DescriptorPreceded).WithLocation(8, 22),
                Diagnostic(DescriptorPreceded).WithLocation(10, 29),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorNotPreceded).WithLocation(10, 19),
                Diagnostic(DescriptorNotPreceded).WithLocation(15, 12),
                Diagnostic(DescriptorNotPreceded).WithLocation(19, 20),
                Diagnostic(DescriptorNotPreceded).WithLocation(22, 51),
                Diagnostic(DescriptorNotPreceded).WithLocation(23, 51),
                Diagnostic(DescriptorNotPreceded).WithLocation(24, 21),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPreceded).WithLocation(3, 20),
                Diagnostic(DescriptorFollowed).WithLocation(3, 20),
                Diagnostic(DescriptorPreceded).WithLocation(3, 35),
                Diagnostic(DescriptorFollowed).WithLocation(3, 35),
                Diagnostic(DescriptorPreceded).WithLocation(5, 27),
                Diagnostic(DescriptorFollowed).WithLocation(5, 27),
                Diagnostic(DescriptorPreceded).WithLocation(8, 22),
                Diagnostic(DescriptorFollowed).WithLocation(8, 22),
                Diagnostic(DescriptorPreceded).WithLocation(10, 29),
                Diagnostic(DescriptorFollowed).WithLocation(10, 29),
            };

            await VerifyCSharpFixAsync(testCode, expected, ExpectedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
