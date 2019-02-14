// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1011ClosingSquareBracketsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1011UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handles valid closing square brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacingOfOpenSquareBracketAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public int this[[CLSCompliant(true)]int index]
    {
        get
        {
            int[][] ints = new int[][
]
            {
                new int[5], new int
[5]
            };
            return ints[0][0];
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle spaces before a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustNotBePrecededBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int this[[CLSCompliant(true) ]int index ]
    {
        get
        {
            int[ ][ ] ints = new int [ ][
]
            {
                new int[5 ], new int
[5 ]
            };
            return ints[0 ][0 ];
        }
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public int this[[CLSCompliant(true) ]int index]
    {
        get
        {
            int[][] ints = new int [][
]
            {
                new int[5], new int
[5]
            };
            return ints[0][0];
        }
    }
}";

            DiagnosticResult[] expected =
            {
                // 5, 41 should be reported by SA1017
                Diagnostic().WithLocation(5, 52).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(9, 18).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(9, 21).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(9, 40).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(12, 27).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(13, 4).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(15, 27).WithArguments(" not",  "preceded"),
                Diagnostic().WithLocation(15, 31).WithArguments(" not",  "preceded"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle spaces before a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustNotBeFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public Action[] TestMethod()
    {
        var x = new Action[] []
        {
            new Action[5] , new Action[5]
        };
        var y = (x[0]);
        y = (x[0] );
        x[0][0]();
        x[0][0] ();
        return x[0] ;
    }
}";

            // spacing violations should be covered SA1010, SA1001, SA1009, SA1008, and then SA1002
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle member access.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketWithMemberAccessAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        var x = new object[] { };
        x[0]?.ToString();
        x[0] ?.ToString();
        x[0].ToString();
        x[0] .ToString();
    }
}
";

            // spacing violation should be reported by SA1019
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle string interpolation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketInStringInterpolationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod(object[] a)
    {
        var x = $""{a[0]}"";
        x = $""{a[0] }"";
    }
}
";

            // spacing violation should be reported by SA1013
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle required spaces after a closing square bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingSquareBracketMustBeFollowedBySpaceAsync()
        {
            const string testCode = @"using System;

public class Foo
{
    public int[]TestMethod(int[]a)
    {
        int[]ints  = new int[]{ };
        return ints;
    }
}";
            const string fixedCode = @"using System;

public class Foo
{
    public int[] TestMethod(int[] a)
    {
        int[] ints  = new int[] { };
        return ints;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 16).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(5, 32).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(7, 13).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(7, 30).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1066.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestClosingBracketInGenericArgumentAsync()
        {
            string testCode = @"using System.Collections.Generic;
public class ClassName
{
    public void TestMethod()
    {
        IEnumerable<object[]> a;
        IEnumerable<object[] > b;
        Dictionary<object[], object[ ]> c;
    }
}";
            string fixedCode = @"using System.Collections.Generic;
public class ClassName
{
    public void TestMethod()
    {
        IEnumerable<object[]> a;
        IEnumerable<object[] > b;
        Dictionary<object[], object[]> c;
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 38).WithArguments(" not", "preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestClosingBracketFollowedByNoSpaceAndIncrementOrDecrementOperatorAsync(string operatorText)
        {
            string validStatament = string.Format(
                @"var i = new int[1];
            i[0]{0};", operatorText);

            await this.TestWhitespaceInStatementOrDeclAsync(validStatament, null, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("++")]
        [InlineData("--")]
        public async Task TestClosingBracketFollowedBySpaceAndIncrementOrDecrementOperatorAsync(string operatorText)
        {
            string invalidStatament = string.Format(
                @"var i = new int[1];
            i[0] {0};", operatorText);

            string fixedStatament = string.Format(
                @"var i = new int[1];
            i[0]{0};", operatorText);

            DiagnosticResult expected = Diagnostic().WithLocation(8, 16).WithArguments(" not", "followed");

            await this.TestWhitespaceInStatementOrDeclAsync(invalidStatament, fixedStatament, expected).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
class ClassName
{
    void Method()
    {
        int[] x = new int[0;
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS0443").WithMessage("Syntax error; value expected").WithLocation(6, 28),
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, ',' expected").WithLocation(6, 28),
                DiagnosticResult.CompilerError("CS1003").WithMessage("Syntax error, ']' expected").WithLocation(6, 28),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2564, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2564")]
        public async Task TestArrayElementPointerDeferenceAsync()
        {
            var testCode = @"public class TestClass
{
    public unsafe string TestMethod(int[] a)
    {
        fixed (int* p = a)
        {
            int*[] xx = new[] { p };
            return xx[0]->ToString(""n2"");
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestColonCanFollowSquareBracketWhenPartOfInterpolationFormatClauseAsync()
        {
            string testCode = @"
class ClassName
{
    void Method()
    {
        var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
        var t = $""{ x[2]:C}"";
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestCommaCanFollowSquareBracketWhenPartOfInterpolationFormatClauseAsync()
        {
            string testCode = @"
class ClassName
{
    void Method()
    {
        var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
        var t = $""{ x[2],3}"";
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        private async Task TestWhitespaceInStatementOrDeclAsync(string originalStatement, string fixedStatement, params DiagnosticResult[] expected)
        {
            string template = @"namespace Foo
{{
    class Bar
    {{
        void DoIt()
        {{
            {0}
        }}
    }}
}}
";
            string originalCode = string.Format(template, originalStatement);
            string fixedCode = string.Format(template, fixedStatement ?? originalStatement);

            var test = new CSharpTest
            {
                TestCode = originalCode,
                FixedCode = fixedCode,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
