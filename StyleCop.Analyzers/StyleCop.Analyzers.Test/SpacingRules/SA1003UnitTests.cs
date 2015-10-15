// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;

    /// <summary>
    /// Unit tests for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1003UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that valid unary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUnaryExpressionsAsync()
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

        // additional valid cases to improve code coverage
        var v9_1 = new { Property = v1++ };
        var v9_2 = v4[v1++];
        var v9_3 = $""{v1++}"";
        var v9_4 = v1++;
        var v9_5 = new
        {
            Property = v1++
        };
        var v9_6 = new
        {
            Property = v1--
        };
        var v9_7 = new int[]
        {
            v1++,
            v1++
        };
        var v9_8 = new int[]
        {
            v1--,
            v1--
        };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that Invalid unary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUnaryExpressionsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        for (var i = 0; i < 2; ++i)
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

        var v9_1 = new
        {
            Property = v1++
,
        };
        var v9_2 = new
        {
            Property = v1--
,
        };
        var v9_3 = new int[]
        {
            v1++,
            v1++
,
        };
        var v9_4 = new int[]
        {
            v1--,
            v1--
,
        };
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

        var v9_1 = new
        {
            Property = v1++,
        };
        var v9_2 = new
        {
            Property = v1--,
        };
        var v9_3 = new int[]
        {
            v1++,
            v1++,
        };
        var v9_4 = new int[]
        {
            v1--,
            v1--,
        };
    }
}
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(DescriptorNotFollowedByWhitespace).WithLocation(10, 18).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(11, 18).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotFollowedByComment).WithLocation(13, 18).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotFollowedByComment).WithLocation(15, 18).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotPrecededByWhitespace).WithLocation(17, 20).WithArguments("(byte)"),
                this.CSharpDiagnostic(DescriptorNotPrecededByWhitespace).WithLocation(18, 22).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(22, 26).WithArguments("++"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(27, 26).WithArguments("--"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(33, 15).WithArguments("++"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(39, 15).WithArguments("--")
            };

            DiagnosticResult[] fixedExpected =
            {
                this.CSharpDiagnostic(DescriptorNotFollowedByComment).WithLocation(12, 18).WithArguments("~"),
                this.CSharpDiagnostic(DescriptorNotFollowedByComment).WithLocation(14, 18).WithArguments("~")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, fixedExpected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid binary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBinaryExpressionsAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid binary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBinaryExpressionsAsync()
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
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(8, 15).WithArguments("="),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 16).WithArguments("="),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(10, 15).WithArguments("="),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(10, 15).WithArguments("="),

                // invalid operators
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 20).WithArguments("+"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(14, 21).WithArguments("|"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(15, 20).WithArguments(">"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(15, 20).WithArguments(">"),

                // invalid lambda expressions
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 23).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(19, 24).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(20, 23).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(20, 23).WithArguments("=>"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid ternary expressions do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidTernaryExpressionsAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid ternary expressions produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidTernaryExpressionsAsync()
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
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(8, 20).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 21).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(10, 20).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(10, 20).WithArguments("?"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(12, 24).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 25).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(14, 24).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(14, 24).WithArguments(":"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(16, 20).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(16, 23).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(17, 21).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(17, 24).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 20).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 20).WithArguments("?"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 22).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 22).WithArguments(":"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(20, 27).WithArguments(":")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid type constraints do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidTypeConstraintsAsync()
        {
            var testCode = @"public class Foo
{
    public T Bar<T>() where T : class
    {
        return default(T);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid type constraints produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidTypeConstraintsAsync()
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
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(3, 31).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(8, 32).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 31).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 31).WithArguments(":")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid base class constructor calls do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBaseCallAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid base class constructor calls produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidBaseCallAsync()
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

    public Foo(float value) :
        // A code fix should not be offered
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

    public Foo(float value) :
        // A code fix should not be offered
        base(value.ToString())
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(5, 27).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 29).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 28).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 28).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 9).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(22, 27).WithArguments(":"),
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(27, 29).WithArguments(":")
            };

            DiagnosticResult[] fixedExpected =
            {
                this.CSharpDiagnostic(DescriptorNotAtEndOfLine).WithLocation(26, 29).WithArguments(":")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, fixedExpected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid auto property initializers do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidAutoPropertyInitializersAsync()
        {
            var testCode = @"public class Foo
{
    public int Bar { get; } = 1;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that invalid auto property initializers produce the correct diagnostics and code fixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidAutoPropertyInitializersAsync()
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
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(3, 28).WithArguments("="),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(5, 29).WithArguments("="),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(7, 28).WithArguments("="),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(7, 28).WithArguments("=")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that valid switch statements do not produce diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSwitchStatementAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a simple for statement will not trigger any diagnostics.
        /// </summary>
        /// <remarks>This is a regression for issue #955.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestForStatementAsync()
        {
            var testCode = @"public class TestClass
{
    // This empty constructor improves test coverage.
    public TestClass()
    {
    }

    public void TestMethod()
    {
        var j = 100;
        var k = 0;
        for (var i = 0; i < 99; k++, i++)
        {
            j--;
        }
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a type cast inside an interpolation statement will not trigger any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInterpolationTypeCastAsync()
        {
            var testCode = @"public class TestClass
{
    public string TestMethod(int i)
    {
        return $""Test value 1: {(byte)i:X2}\r\n""
            + $""Test value 2: {++i}\r\n""
            + $""Test value 3: {i++}\r\n"";
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that prefix unary expression that are the first token on a source line will not trigger any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFirstOnLineUnaryPrefixExpressionsAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod1(byte b, int i)
    {
    }

    public void TestMethod2(int i)
    {
        TestMethod1(
            (byte)i,
            ++i);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that unary minus expression will not trigger any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestUnaryMinusNotReportedAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var x = - 3;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that unary plus expression will not trigger any diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestUnaryPlusNotReportedAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var x = + 3;
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1003SymbolsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1003CodeFixProvider();
        }
    }
}
