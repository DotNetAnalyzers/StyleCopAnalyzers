﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.SA1003CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1003SymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1003UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(10, 18).WithArguments("~"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(11, 18).WithArguments("~"),
                    Diagnostic(DescriptorNotFollowedByComment).WithLocation(13, 18).WithArguments("~"),
                    Diagnostic(DescriptorNotFollowedByComment).WithLocation(15, 18).WithArguments("~"),
                    Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(17, 20).WithArguments("(byte)"),
                    Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(18, 22).WithArguments("~"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(22, 26).WithArguments("++"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(27, 26).WithArguments("--"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(33, 15).WithArguments("++"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(39, 15).WithArguments("--"),
                },
                RemainingDiagnostics =
                {
                    Diagnostic(DescriptorNotFollowedByComment).WithLocation(12, 18).WithArguments("~"),
                    Diagnostic(DescriptorNotFollowedByComment).WithLocation(14, 18).WithArguments("~"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2471, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2471")]
        public async Task TestUnaryMemberAccessAsync()
        {
            var testCode = @"public class ClassName
{
    public unsafe void MethodName()
    {
        int? x = 0;
        int* y = null;

        x -- .ToString();
        x --.ToString();
        x-- .ToString();

        x ++ .ToString();
        x ++.ToString();
        x++ .ToString();

        x -- ?.ToString();
        x --?.ToString();
        x-- ?.ToString();

        x ++ ?.ToString();
        x ++?.ToString();
        x++ ?.ToString();

        y -- ->ToString();
        y --->ToString();
        y-- ->ToString();

        y ++ ->ToString();
        y ++->ToString();
        y++ ->ToString();
    }
}
";
            var fixedTestCode = @"public class ClassName
{
    public unsafe void MethodName()
    {
        int? x = 0;
        int* y = null;

        x--.ToString();
        x--.ToString();
        x--.ToString();

        x++.ToString();
        x++.ToString();
        x++.ToString();

        x--?.ToString();
        x--?.ToString();
        x--?.ToString();

        x++?.ToString();
        x++?.ToString();
        x++?.ToString();

        y--->ToString();
        y--->ToString();
        y--->ToString();

        y++->ToString();
        y++->ToString();
        y++->ToString();
    }
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(8, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(8, 11).WithArguments("--"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(9, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(10, 10).WithArguments("--"),

                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(12, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(12, 11).WithArguments("++"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(13, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(14, 10).WithArguments("++"),

                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(16, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(16, 11).WithArguments("--"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(17, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(18, 10).WithArguments("--"),

                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(20, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(20, 11).WithArguments("++"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(21, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(22, 10).WithArguments("++"),

                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(24, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(24, 11).WithArguments("--"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(25, 11).WithArguments("--"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(26, 10).WithArguments("--"),

                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(28, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(28, 11).WithArguments("++"),
                Diagnostic(DescriptorNotPrecededByWhitespace).WithLocation(29, 11).WithArguments("++"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(30, 10).WithArguments("++"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(8, 15).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 16).WithArguments("="),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(10, 15).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(10, 15).WithArguments("="),

                // invalid operators
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 20).WithArguments("+"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(14, 21).WithArguments("|"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(15, 20).WithArguments(">"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(15, 20).WithArguments(">"),

                // invalid lambda expressions
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 23).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(19, 24).WithArguments("=>"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(20, 23).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(20, 23).WithArguments("=>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(8, 20).WithArguments("?"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 21).WithArguments("?"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(10, 20).WithArguments("?"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(10, 20).WithArguments("?"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(12, 24).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 25).WithArguments(":"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(14, 24).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(14, 24).WithArguments(":"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(16, 20).WithArguments("?"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(16, 23).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(17, 21).WithArguments("?"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(17, 24).WithArguments(":"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 20).WithArguments("?"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 20).WithArguments("?"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(18, 22).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 22).WithArguments(":"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(20, 27).WithArguments(":"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(3, 31).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(8, 32).WithArguments(":"),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 31).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 31).WithArguments(":"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedTestCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorPrecededByWhitespace).WithLocation(5, 27).WithArguments(":"),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 29).WithArguments(":"),
                    Diagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 28).WithArguments(":"),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 28).WithArguments(":"),
                    Diagnostic(DescriptorFollowedByWhitespace).WithLocation(18, 9).WithArguments(":"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(22, 27).WithArguments(":"),
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(27, 29).WithArguments(":"),
                },
                RemainingDiagnostics =
                {
                    Diagnostic(DescriptorNotAtEndOfLine).WithLocation(26, 29).WithArguments(":"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(3, 28).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(5, 29).WithArguments("="),
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(7, 28).WithArguments("="),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(7, 28).WithArguments("="),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a simple for statement will not trigger any diagnostics.
        /// </summary>
        /// <remarks><para>This is a regression for issue #955.</para></remarks>
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the <c>=&gt;</c> operator for expression-bodied members triggers diagnostics as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestExpressionBodiedMembersAsync()
        {
            var testCode = @"namespace N1
{
    public class C1
    {
        public int Answer=>42; // Property
        public void DoStuff(int a)=>System.Console.WriteLine(a); // method
        public static C1 operator +(C1 a, C1 b)=>a; // operator
        public static explicit operator C1(int i)=>null; // conversion operator
        public int this[int index]=>23; // indexer
    }
}
";
            var fixedTestCode = @"namespace N1
{
    public class C1
    {
        public int Answer => 42; // Property
        public void DoStuff(int a) => System.Console.WriteLine(a); // method
        public static C1 operator +(C1 a, C1 b) => a; // operator
        public static explicit operator C1(int i) => null; // conversion operator
        public int this[int index] => 23; // indexer
    }
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(5, 26).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(5, 26).WithArguments("=>"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(6, 35).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(6, 35).WithArguments("=>"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(7, 48).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(7, 48).WithArguments("=>"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(8, 50).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(8, 50).WithArguments("=>"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(9, 35).WithArguments("=>"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 35).WithArguments("=>"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that pointer dereference will not trigger SA1003, as it is covered by SA1023.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1776, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1776")]
        public async Task TestPointerDereferenceNotReportedAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        unsafe
        {
            int * value = null;
            int blah = * value;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that colon in a ternary operator triggers SA1003.
        /// </summary>
        /// <param name="postfixOperator">The postfix operator to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestTernaryFollowedByColonAsync(string postfixOperator)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = x > 0 ? x{postfixOperator}:x{postfixOperator};
        var y2 = x > 0 ? x{postfixOperator} :x{postfixOperator};
        var y3 = x > 0 ? x{postfixOperator}: x{postfixOperator};
        var y4 = x > 0 ? x{postfixOperator} : x{postfixOperator};
    }}
}}
";
            var fixedCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = x > 0 ? x{postfixOperator} : x{postfixOperator};
        var y2 = x > 0 ? x{postfixOperator} : x{postfixOperator};
        var y3 = x > 0 ? x{postfixOperator} : x{postfixOperator};
        var y4 = x > 0 ? x{postfixOperator} : x{postfixOperator};
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(6, 27, 6, 29).WithArguments(postfixOperator),
                Diagnostic(DescriptorPrecededByWhitespace).WithSpan(6, 29, 6, 30).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(6, 29, 6, 30).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(7, 30, 7, 31).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(8, 27, 8, 29).WithArguments(postfixOperator),
                Diagnostic(DescriptorPrecededByWhitespace).WithSpan(8, 29, 8, 30).WithArguments(":"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that colon in a ternary operator triggers SA1003.
        /// </summary>
        /// <param name="prefixOperator">The prefix operator to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestTernaryPrecededByColonAsync(string prefixOperator)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = x > 0 ? {prefixOperator}x:{prefixOperator}x;
        var y2 = x > 0 ? {prefixOperator}x :{prefixOperator}x;
        var y3 = x > 0 ? {prefixOperator}x: {prefixOperator}x;
        var y4 = x > 0 ? {prefixOperator}x : {prefixOperator}x;
    }}
}}
";
            var fixedCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = x > 0 ? {prefixOperator}x : {prefixOperator}x;
        var y2 = x > 0 ? {prefixOperator}x : {prefixOperator}x;
        var y3 = x > 0 ? {prefixOperator}x : {prefixOperator}x;
        var y4 = x > 0 ? {prefixOperator}x : {prefixOperator}x;
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithSpan(6, 29, 6, 30).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(6, 29, 6, 30).WithArguments(":"),
                Diagnostic(DescriptorFollowedByWhitespace).WithSpan(7, 30, 7, 31).WithArguments(":"),
                Diagnostic(DescriptorPrecededByWhitespace).WithSpan(8, 29, 8, 30).WithArguments(":"),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that interpolated string format triggers SA1003, and does not require a preceding space.
        /// </summary>
        /// <param name="postfixOperator">The postfix operator to verify.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        [WorkItem(3073, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3073")]
        public async Task TestInterpolatedStringFormatAsync(string postfixOperator)
        {
            var testCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = $""{{x{postfixOperator}:N}}"";
        var y2 = $""{{x{postfixOperator} :N}}"";
        var y3 = $""{{x{postfixOperator}: N}}"";
        var y4 = $""{{x{postfixOperator} : N}}"";
    }}
}}
";
            var fixedCode = $@"public class TestClass
{{
    public void TestMethod()
    {{
        int x = 0;
        var y1 = $""{{x{postfixOperator}:N}}"";
        var y2 = $""{{x{postfixOperator}:N}}"";
        var y3 = $""{{x{postfixOperator}: N}}"";
        var y4 = $""{{x{postfixOperator}: N}}"";
    }}
}}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotFollowedByWhitespace).WithSpan(7, 22, 7, 24).WithArguments(postfixOperator),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithSpan(9, 22, 9, 24).WithArguments(postfixOperator),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that spacing errors in conditional directives are fixed correctly. This is a regression test for
        /// DotNetAnalyzers/StyleCopAnalyzers#1831.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestConditionalDirectivesCorrectlyFixedAsync()
        {
            var testCode = @"class Program
{
    static int Main(string[] args)
    {
#if! NOT
        return 1;
#endif

#if! NOT&&! Y
        return 1;
#endif
    }
}
";
            var fixedCode = @"class Program
{
    static int Main(string[] args)
    {
#if !NOT
        return 1;
#endif

#if !NOT && !Y
        return 1;
#endif
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(5, 4).WithArguments("!"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(5, 4).WithArguments("!"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(9, 4).WithArguments("!"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(9, 4).WithArguments("!"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(9, 9).WithArguments("&&"),
                Diagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 9).WithArguments("&&"),

                Diagnostic(DescriptorPrecededByWhitespace).WithLocation(9, 11).WithArguments("!"),
                Diagnostic(DescriptorNotFollowedByWhitespace).WithLocation(9, 11).WithArguments("!"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
