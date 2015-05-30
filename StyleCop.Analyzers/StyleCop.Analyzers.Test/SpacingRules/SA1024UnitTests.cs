namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1024ColonsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1024UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

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
            default:
                goto _label;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            default:
                goto _label;
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    public Foo() :base()
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
            default:
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 21).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(3, 37).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(5, 18).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(8, 23).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(10, 30).WithArguments(string.Empty, "followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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
            default:
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 20).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(3, 36).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(5, 27).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(8, 22).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(10, 29).WithArguments(string.Empty, "preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the colons must not preceded by space.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacedColonsMustNotBePrecededAsync()
        {
            const string testCode = @"using System;

public class Foo<T> : object where T : IFormattable
{
    public Foo() : base()
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
            default :
                goto _label;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 19).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(15, 12).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(19, 20).WithArguments(" not", "preceded"),
                this.CSharpDiagnostic().WithLocation(21, 21).WithArguments(" not", "preceded"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1024ColonsMustBeSpacedCorrectly();
        }
    }
}
