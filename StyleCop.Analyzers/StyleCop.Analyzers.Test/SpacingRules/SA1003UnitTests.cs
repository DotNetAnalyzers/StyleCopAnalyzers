
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
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that valid unary expressions do not produce diagnostics.
        /// </summary>
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
        [Fact]
        public async Task TestValidBinaryExpressions()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        var v1 = 1 + 2;
        var v2 = v1 > 0 ? 1 : 2;
        var v3 = (2 * 3);
        var v4 = new[] { 1, 2, 3 };
        var v5 = 1;
        var v6 = v4[v5 + 1];
        Action<int> v7 = t => { v5 += v6 + t; };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that invalid binary expressions produce the correct diagnostics and code fixes.
        /// </summary>
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

        // invalid conditionals
        var v7 = v6? 1 : 0;
        var v8 = v6 ?1 : 0;
        var v9 = v6?1 : 0;

        var v10 = v6 ? 1: 0;
        var v11 = v6 ? 1 :0;
        var v12 = v6 ? 1:0;

        var v13 = v6? 1: 0;
        var v14 = v6 ?1 :0;
        var v15 = v6?1:0;

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

        // invalid conditionals
        var v7 = v6 ? 1 : 0;
        var v8 = v6 ? 1 : 0;
        var v9 = v6 ? 1 : 0;

        var v10 = v6 ? 1 : 0;
        var v11 = v6 ? 1 : 0;
        var v12 = v6 ? 1 : 0;

        var v13 = v6 ? 1 : 0;
        var v14 = v6 ? 1 : 0;
        var v15 = v6 ? 1 : 0;

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
                // invalid conditionals
                this.CSharpDiagnostic().WithLocation(18, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(19, 21).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(20, 20).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(20, 20).WithArguments("?", BeFollowedBySpace),

                this.CSharpDiagnostic().WithLocation(22, 25).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(23, 26).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(24, 25).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(24, 25).WithArguments(":", BeFollowedBySpace),

                this.CSharpDiagnostic().WithLocation(26, 21).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(26, 24).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(27, 22).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(27, 25).WithArguments(":", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(28, 21).WithArguments("?", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(28, 21).WithArguments("?", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(28, 23).WithArguments(":", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(28, 23).WithArguments(":", BeFollowedBySpace),
                // invalid lambda expressions
                this.CSharpDiagnostic().WithLocation(31, 23).WithArguments("=>", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(32, 24).WithArguments("=>", BeFollowedBySpace),
                this.CSharpDiagnostic().WithLocation(33, 23).WithArguments("=>", BePrecededBySpace),
                this.CSharpDiagnostic().WithLocation(33, 23).WithArguments("=>", BeFollowedBySpace),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that valid type constraints do not produce diagnostics.
        /// </summary>
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
