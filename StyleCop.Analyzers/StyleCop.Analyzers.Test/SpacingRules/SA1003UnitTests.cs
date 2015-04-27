namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1003UnitTests : CodeFixVerifier
    {
        private const string BePrecededBySpace = "be preceded by a space";
        private const string BeFollowedBySpace = "be followed by a space";
        private const string NotFollowedBySpace = "not be followed by a space";
        private const string NotFollowedByComment = "not be followed by a comment";
        private const string NotAppearAtLineEnd = "not appear at the end of a line";
        private const string NotBePrecededBySpace = "not be preceded by a space";

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that valid unary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUnaryExpressions()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var v1 = +1;
        var v2 = (byte)2;
        var v3 = (--v1 == 0);
        var v4 = new[] { 1, 2, 3 };
        var v5 = 1;
        var v6 = v4[++v5];
        var v7 =
++v5;
        var v8 = (short)++v2;
        var v9 = v4[ /* increment */ ++v1];
        
        // valid for SA1003 only
        var v10 = - 1; /* SA1021 */
        var v11 = + 1; /* SA1022 */
        var v12 = ++ v1; /* SA1020 */
        var v13 = -- v1; /* SA1020 */
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that Invalid unary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUnaryExpressions()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        for (var i = 0; i < 2;++i)
        {
        }

        var v1 = 1;
        var v2 = ~ v1;
        var v3 = ~
v1;
        var v4 = ~//comment
v1;
        var v5 = ~/*comment*/v1;
        var v6 = new[] { 1, 2, 3 };
        var v7 = ( (byte)v1 == 0);
        var v8 = v6[ ~v1];
    }
}
";

            // The comments cannot be fixed, as they should not be removed automatically and moving them will cause issues in more complex statements
            var fixedTestCode = @"public class Foo
{
    public void Bar()
    {
        for (var i = 0; i < 2; ++i)
        {
        }

        var v1 = 1;
        var v2 = ~v1;
        var v3 = ~v1;
        var v4 = ~//comment
v1;
        var v5 = ~/*comment*/v1;
        var v6 = new[] { 1, 2, 3 };
        var v7 = ((byte)v1 == 0);
        var v8 = v6[~v1];
    }
}
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 31).WithArguments("++", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(10, 18).WithArguments("~", NotFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(11, 18).WithArguments("~", NotAppearAtLineEnd),
                this.CSharpDiagnostic().WithLocation(13, 18).WithArguments("~", NotFollowedByComment),
                this.CSharpDiagnostic().WithLocation(15, 18).WithArguments("~", NotFollowedByComment),
                this.CSharpDiagnostic().WithLocation(17, 20).WithArguments("(byte)", NotBePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(18, 22).WithArguments("~", NotBePrecededBySpace)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid binary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBinaryExpressions()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        var v1 = 1 + 2;
        var v2 = (2 * 3);
        var v3 = new[] { 1, 2, 3 };
        var v4 = 1;
        var v5 = v3[v4 + 1];
        Action<int> v6 = t => { v4 += v5 + t; };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid binary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBinaryExpressions()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        // invalid assignment
        var v1= 1;
        var v2 =2;
        var v3=3;

        // invalid operators
        var v4 = v1+ v2;
        var v5 = v1 |v2;
        var v6 = v1>v2;

        // invalid lambda expressions
        Action a1 = ()=> { };
        Action a2 = () =>{ };
        Action a3 = ()=>{ };
    }
}
";

            var fixedTestCode = @"using System;

public class Foo
{
    public void Bar()
    {
        // invalid assignment
        var v1 = 1;
        var v2 = 2;
        var v3 = 3;

        // invalid operators
        var v4 = v1 + v2;
        var v5 = v1 | v2;
        var v6 = v1 > v2;

        // invalid lambda expressions
        Action a1 = () => { };
        Action a2 = () => { };
        Action a3 = () => { };
    }
}
";
            DiagnosticResult[] expected =
            {
                // invalid assignment
                this.CSharpDiagnostic().WithLocation(8, 15).WithArguments("=", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(9, 16).WithArguments("=", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(10, 15).WithArguments("=", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(10, 15).WithArguments("=", BeFollowedBySpace),
                // invalid operators
                this.CSharpDiagnostic().WithLocation(13, 20).WithArguments("+", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(14, 21).WithArguments("|", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(15, 20).WithArguments(">", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(15, 20).WithArguments(">", BeFollowedBySpace),
                // invalid lambda expressions
                this.CSharpDiagnostic().WithLocation(18, 23).WithArguments("=>", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(19, 24).WithArguments("=>", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(20, 23).WithArguments("=>", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(20, 23).WithArguments("=>", BeFollowedBySpace),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid ternary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidTernaryExpressions()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        var v1 = 1 + 2;
        var v2 = v1 > 0 ? 1 : 2;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid ternary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidTernaryExpressions()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar(bool p1)
    {
        // invalid conditionals
        var v1 = p1? 1 : 0;
        var v2 = p1 ?1 : 0;
        var v3 = p1?1 : 0;

        var v4 = p1 ? 1: 0;
        var v5 = p1 ? 1 :0;
        var v6 = p1 ? 1:0;

        var v7 = p1? 1: 0;
        var v8 = p1 ?1 :0;
        var v9 = p1?1:0;

        var v10 = p1 ? (1): 0;
    }
}
";

            var fixedTestCode = @"using System;

public class Foo
{
    public void Bar(bool p1)
    {
        // invalid conditionals
        var v1 = p1 ? 1 : 0;
        var v2 = p1 ? 1 : 0;
        var v3 = p1 ? 1 : 0;

        var v4 = p1 ? 1 : 0;
        var v5 = p1 ? 1 : 0;
        var v6 = p1 ? 1 : 0;

        var v7 = p1 ? 1 : 0;
        var v8 = p1 ? 1 : 0;
        var v9 = p1 ? 1 : 0;

        var v10 = p1 ? (1) : 0;
    }
}
";
            DiagnosticResult[] expected =
            {
                // invalid conditionals
                this.CSharpDiagnostic().WithLocation(8, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(10, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(10, 20).WithArguments("?", BeFollowedBySpace),

                this.CSharpDiagnostic().WithLocation(12, 24).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(13, 25).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(14, 24).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(14, 24).WithArguments(":", BeFollowedBySpace),

                this.CSharpDiagnostic().WithLocation(16, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(16, 23).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(17, 21).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(17, 24).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(18, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(18, 20).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(18, 22).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(18, 22).WithArguments(":", BeFollowedBySpace),

                this.CSharpDiagnostic().WithLocation(20, 27).WithArguments(":", BePrecededBySpace)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid type constraints do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidTypeConstraints()
        {
            var testCode = @"public class Foo
{
    public T Bar<T>() where T : class
    {
        return default(T);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid type constraints produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidTypeConstraints()
        {
            var testCode = @"public class Foo
{
    public T Bar1<T>() where T: class
    {
        return default(T);
    }

    public T Bar2<T>() where T :class
    {
        return default(T);
    }

    public T Bar3<T>() where T:class
    {
        return default(T);
    }
}
";

            var fixedTestCode = @"public class Foo
{
    public T Bar1<T>() where T : class
    {
        return default(T);
    }

    public T Bar2<T>() where T : class
    {
        return default(T);
    }

    public T Bar3<T>() where T : class
    {
        return default(T);
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 31).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(8, 32).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(13, 31).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(13, 31).WithArguments(":", BeFollowedBySpace)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid base class constructor calls do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBaseCall()
        {
            var testCode = @"using System;

public class Foo : Exception
{
    public Foo() : base()
    {
    }

    public Foo(int value)
        : base(value.ToString())
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid base class constructor calls produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBaseCall()
        {
            var testCode = @"using System;

public class Foo : Exception
{
    public Foo(byte value): base(value.ToString())
    {
    }

    public Foo(sbyte value) :base(value.ToString())
    {
    }

    public Foo(short value):base(value.ToString())
    {
    }

    public Foo(ushort value)
        :base(value.ToString())
    {
    }

    public Foo(int value) :
        base(value.ToString())
    {
    }
}
";

            var fixedTestCode = @"using System;

public class Foo : Exception
{
    public Foo(byte value) : base(value.ToString())
    {
    }

    public Foo(sbyte value) : base(value.ToString())
    {
    }

    public Foo(short value) : base(value.ToString())
    {
    }

    public Foo(ushort value)
        : base(value.ToString())
    {
    }

    public Foo(int value) : base(value.ToString())
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(5, 27).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(9, 29).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(13, 28).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(13, 28).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(18, 9).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(22, 27).WithArguments(":", NotAppearAtLineEnd)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid auto property initializers do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidAutoPropertyInitializers()
        {
            var testCode = @"public class Foo
{
    public int Bar { get; } = 1;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid auto property initializers produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidAutoPropertyInitializers()
        {
            var testCode = @"public class Foo
{
    public int Bar { get; }= 1;

    public int Baz { get; } =2;

    public int Qux { get; }=3;
}
";

            var fixedTestCode = @"public class Foo
{
    public int Bar { get; } = 1;

    public int Baz { get; } = 2;

    public int Qux { get; } = 3;
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 28).WithArguments("=", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(5, 29).WithArguments("=", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(7, 28).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(7, 28).WithArguments(":", BeFollowedBySpace)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid switch statements do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSwitchStatement()
        {
            var testCode = @"public class Foo
{
    public int Bar(int p1)
    {
        switch (p1)
        {
            case 1:
                return 0;
            case (2):
                return 1;
            default:
                return p1 + 1;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1003SymbolsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1003CodeFixProvider();
        }
    }
}
