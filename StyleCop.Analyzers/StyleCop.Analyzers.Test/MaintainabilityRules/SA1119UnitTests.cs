// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis,
        Analyzers.MaintainabilityRules.SA1119CodeFixProvider>;

    public class SA1119UnitTests
    {
        protected const string DiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId;
        protected const string ParenthesesDiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.ParenthesesDiagnosticId;

        [Fact]
        public async Task TestLiteralAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = (1);
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 17, 5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 19),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLiteralDoubleParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = ((1));
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 17, 5, 22),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    Diagnostic(DiagnosticId).WithSpan(5, 18, 5, 21),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 18),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 21),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (ToString());
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 20, 5, 32),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 31),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ToString();
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalMemberAsync()
        {
            var testCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = Local + Local.IndexOf('x');
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalMemberMemberAccessAsync()
        {
            var testCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = (Local).ToString() + Local.IndexOf(('x'));
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 20, 6, 27),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                    Diagnostic(DiagnosticId).WithSpan(6, 55, 6, 60),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 55),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 59),
                };

            var fixedCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = Local.ToString() + Local.IndexOf('x');
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalMemberAssignmentAsync()
        {
            var testCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        this.Local = Local;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalMemberAssignmentParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        (this.Local) = Local;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 9, 6, 21),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 9),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                };

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        this.Local = Local;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalMemberParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        int x = (Local);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 17, 6, 24),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 23),
                };

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        int x = Local;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = (int)3;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = ((int)3);
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 17, 5, 25),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 24),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (int)3;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastAssignmentAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = (int)3;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastAssignmentParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = ((int)3);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 13, 6, 21),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = (int)3;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastMemberAccessAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ((int)3).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastMemberAccessAssignmentAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x;
        x = ((int)3).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorsAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3;
        x = x++;
        x = x--;
        x = ++x;
        x = ~x;
        x = +x;
        x = -x;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorsParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3;
        x = (x++);
        x = (x--);
        x = (++x);
        x = (~x);
        x = (+x);
        x = (-x);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 13, 6, 18),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    Diagnostic(DiagnosticId).WithSpan(7, 13, 7, 18),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(7, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(7, 17),
                    Diagnostic(DiagnosticId).WithSpan(8, 13, 8, 18),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(8, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(8, 17),
                    Diagnostic(DiagnosticId).WithSpan(9, 13, 9, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(9, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(9, 16),
                    Diagnostic(DiagnosticId).WithSpan(10, 13, 10, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(10, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(10, 16),
                    Diagnostic(DiagnosticId).WithSpan(11, 13, 11, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(11, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(11, 16),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3;
        x = x++;
        x = x--;
        x = ++x;
        x = ~x;
        x = +x;
        x = -x;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCheckedUncheckedAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * checked(5);
        x = 3 * unchecked(5);
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCheckedUncheckedParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * (checked(5));
        x = 3 * (unchecked(5));
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 21, 5, 33),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 21),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 32),
                    Diagnostic(DiagnosticId).WithSpan(6, 17, 6, 31),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 30),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * checked(5);
        x = 3 * unchecked(5);
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNameOfAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = nameof(Foo) + ""Bar"";
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNameOfParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (nameof(Foo)) + ""Bar"";
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 20, 5, 33),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 32),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = nameof(Foo) + ""Bar"";
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIsExpressionAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = """" is string;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIsExpressionMemberAccessAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ("""" is string).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIsExpressionParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = ("""" is string);
        x = ("""" is string);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 18, 5, 32),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 18),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 31),
                    Diagnostic(DiagnosticId).WithSpan(6, 13, 6, 27),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = """" is string;
        x = """" is string;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAssignmentAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = y = ""foo"";
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAssignmentParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = (y = ""foo"");
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 20, 6, 31),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 30),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = y = ""foo"";
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInnerAssignmentAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = (y = ""foo"") + ""bar"";
        x = (y = ""foo"").ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = true ? ""foo"" : ""bar"";
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (true ? ""foo"" : ""bar"");
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 20, 5, 42),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 41),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = true ? ""foo"" : ""bar"";
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalInnerAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (true ? ""foo"" : ""bar"") + ""test"";
        string y = (true ? ""foo"" : ""bar"").ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCoalesceAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ""foo"" ?? ""bar"";
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCoalesceParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (""foo"" ?? ""bar"");
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 20, 5, 36),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 35),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ""foo"" ?? ""bar"";
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCoalesceInnerAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (""foo"" ?? ""bar"") + ""test"";
        string y = (""foo"" ?? ""bar"").ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = v => v;
        System.Func<string, string> y = (v) => v;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = (v => v);
        System.Func<string, string> y = ((v) => v);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 41, 5, 49),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 41),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 48),
                    Diagnostic(DiagnosticId).WithSpan(6, 41, 6, 51),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 41),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 50),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = v => v;
        System.Func<string, string> y = (v) => v;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaInnerAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ((System.Func<string, string>)(v => v))(""foo"");
        string y = ((System.Func<string, string>)((v) => v))(""foo"");
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = new int[10];
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayParenthesisAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = (new int[10]);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(5, 19, 5, 32),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 19),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(5, 31),
                };

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = new int[10];
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayInnerAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (new int[10]).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQueryAsync()
        {
            var testCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = from y in new int[10] select y + 1;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQueryParenthesisAsync()
        {
            var testCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = (from y in new int[10] select y + 1);
    }
}";
            DiagnosticResult[] expected =
                {
                    Diagnostic(DiagnosticId).WithSpan(6, 17, 6, 53),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 52),
                };

            var fixedCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = from y in new int[10] select y + 1;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQueryInnerAsync()
        {
            var testCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = (from y in new int[10] select y + 1).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAwaitInnerAsync()
        {
            var testCode = @"public class Foo
{
    public async void Bar()
    {
        var x = (await System.Threading.Tasks.Task.Delay(10).ContinueWith(task => System.Threading.Tasks.Task.FromResult(1))).ToString();
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBinaryAsync()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        var x = 1 + 1 + 1;
        var y = (1 + 1) + (1 + 1);
        var z = 1 * ~(1 + 1);
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInnerTriviaCodeFixAsync()
        {
            var testCode = @"public class Foo
{
        private bool Test()
        {
            string x = """";
            return (x == null || x.GetType() == typeof(object)
#if !(NET35 || NET20 || PORTABLE40)
                    || x.GetType() == typeof(string)
#endif
                );
        }
}";
            var fixedCode = @"public class Foo
{
        private bool Test()
        {
            string x = """";
            return x == null || x.GetType() == typeof(object)
#if !(NET35 || NET20 || PORTABLE40)
                    || x.GetType() == typeof(string)
#endif
                ;
        }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 20, 10, 18),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(10, 17),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixWorksInTriviaAsync()
        {
            var testCode = @"#if (NET20 || NET35)
// Foo
#endif";
            var fixedCode = @"#if NET20 || NET35
// Foo
#endif";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(1, 5, 1, 21),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(1, 5),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(1, 20),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoLeadingTriviaAsync()
        {
            var testCode = @"public class Foo
{
    public string Bar()
    {
        string foo = """";
        return(foo);
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 15, 6, 20),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 15),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 19),
            };

            var fixedCode = @"public class Foo
{
    public string Bar()
    {
        string foo = """";
        return foo;
    }
}";
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesisInInterpolatedStringThatShouldBeRemovedAsync()
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1284
            string testCode = @"class Foo
{
    public void Bar()
    {
        bool flag = false;
        string data = $""{(flag)}"";
    }
}";
            string fixedCode = @"class Foo
{
    public void Bar()
    {
        bool flag = false;
        string data = $""{ flag}"";
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 26, 6, 32),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 31),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesisInInterpolatedStringThatShouldNotBeRemovedAsync()
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1284
            string testCode = @"class Foo
{
    public void Bar()
    {
        bool flag = false;
        string data = $""{ (flag ? ""yep"" : ""nope"")}"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesisInInterpolatedStringThatShouldNotBeRemovedWithAssignmentAsync()
        {
            // This is a regression test for https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1284
            string testCode = @"class Foo
{
    public void Bar()
    {
        bool flag = false;
        string foo;
        string data = $""{ (foo = flag ? ""yep"" : ""nope"")}"";
        data = $""{ (foo = foo += flag ? ""yep"" : ""nope"")}"";
        data = $""{ (foo += foo += foo += foo += foo += flag ? ""yep"" : ""nope"")}"";
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixDoesNotRemoveSpacesAsync()
        {
            var testCode = @"public class Foo
{
    public string Bar()
    {
        string foo = """";
        return     (     foo     )     ;
    }
}";
            var fixedCode = @"public class Foo
{
    public string Bar()
    {
        string foo = """";
        return          foo          ;
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 20, 6, 35),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 34),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestParenthesisDiagnosticIsNotTriggeredIfParentDiagnosticIsDisabledAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        int i = (1 + 1);
    }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                DisabledDiagnostics =
                {
                    DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMemberAccessExpressionAroundConditionalMemberAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { intValue = 5 };
        int number = (myObject?.intValue).GetValueOrDefault();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementAccessExpressionAroundConditionalMemberAccessExpressionAsync()
        {
            // In this case removing the parenthesis is an syntactical(because char is a value type) change to the code.
            // If (myObject?.Foo)[0] would be a reference type removing parenthesis would be a semantic change
            // because (myObject?.Foo)[0] crashes if myObject is null, but myObject?.Foo[0] evaluates to null.
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { Foo = ""Bar"" };
        char number = (myObject?.Foo)[0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalMemberAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { intValue = 5 };
        int? number = (myObject?.intValue);
    }
}";

            string fixedCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { intValue = 5 };
        int? number = myObject?.intValue;
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 23, 6, 43),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 23),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 42),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalMemberAccessExpressionAroundConditionalMemberAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { Foo = """" };
        var number = (myObject?.Foo)?.ToString();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalElementExpressionAroundConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { Foo = """" };
        var number = (myObject?.Foo)?[0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMemberAccessExpressionAroundElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { Foo = ""Bar"" };
        var number = (myObject.Foo)[0];
    }
}";

            string fixedCode = @"class Foo
{
    public void Bar()
    {
        var myObject = new { Foo = ""Bar"" };
        var number = myObject.Foo[0];
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 22, 6, 36),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 35),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMemberAccessExpressionAroundConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string foo = """";
        var number = (foo?[0]).ToString();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementAccessExpressionAroundConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string[] foo = new string[0];
        char number = (foo?[0])[0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string foo = null;
        var number = (foo?[0]);
    }
}";

            string fixedCode = @"class Foo
{
    public void Bar()
    {
        string foo = null;
        var number = foo?[0];
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 22, 6, 31),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 30),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalMemberAccessExpressionAroundConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string foo = null;
        var number = (foo?[0])?.ToString();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConditionalElementAccessExpressionAroundConditionalElementAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string[] foo = null;
        var number = (foo?[0])?[0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestElementMemberAccessExpressionAroundElementMemberAccessExpressionAsync()
        {
            string testCode = @"class Foo
{
    public void Bar()
    {
        string[] foo = null;
        var number = (foo[0])[0];
    }
}";

            string fixedCode = @"class Foo
{
    public void Bar()
    {
        string[] foo = null;
        var number = foo[0][0];
    }
}";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 22, 6, 30),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 29),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a preprocessor statement with unnecessary parenthesis is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2069, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2069")]
        public async Task VerifyThatPreprocessorStatementIsHandledCorrectlyAsync()
        {
            string testCode = @"
public class Program
{
    public static void Main(string[] args)
    {
#if(DEBUG)

#endif

#if (DEBUG)

#endif
    }
}
";

            string fixedCode = @"
public class Program
{
    public static void Main(string[] args)
    {
#if DEBUG

#endif

#if DEBUG

#endif
    }
}
";
            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 4, 6, 11),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 4),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(6, 10),
                Diagnostic(DiagnosticId).WithSpan(10, 5, 10, 12),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(10, 5),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(10, 11),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2992")]
        public async Task VerifyCodeFixDoesNotInsertUnnecessarySpacesAsync()
        {
            var testCode = @"using System;
internal class Program
{
    private static readonly DateTime MaxDate = new DateTime((new DateTime(2101, 1, 28, 23, 59, 59, 999)).Ticks + 9999);

    private static void Main()
    {
        Console.WriteLine(MaxDate);
    }
}
";

            var fixedCode = @"using System;
internal class Program
{
    private static readonly DateTime MaxDate = new DateTime(new DateTime(2101, 1, 28, 23, 59, 59, 999).Ticks + 9999);

    private static void Main()
    {
        Console.WriteLine(MaxDate);
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(4, 61, 4, 105),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(4, 61),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(4, 104),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
