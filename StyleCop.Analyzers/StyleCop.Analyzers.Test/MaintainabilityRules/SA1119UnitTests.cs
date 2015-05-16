namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1119UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId;
        private const string ParenthesesDiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.ParenthesesDiagnosticId;

        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 19)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 18),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 18),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 21),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 31)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ToString();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 55),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 55),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 59),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = Local.ToString() + Local.IndexOf('x');
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 9),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 9),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 20)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        this.Local = Local;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 23)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        int x = Local;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 24)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (int)3;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 20)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = (int)3;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(7, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(7, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(7, 17),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(8, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(8, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(8, 17),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(9, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(9, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(9, 16),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(10, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(10, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(10, 16),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(11, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(11, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(11, 16),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 21),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 21),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 32),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 30),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * checked(5);
        x = 3 * unchecked(5);
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 32)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = nameof(Foo) + ""Bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 18),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 18),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 31),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 13),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = """" is string;
        x = """" is string;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 30)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = y = ""foo"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 41)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = true ? ""foo"" : ""bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 20),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 35)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ""foo"" ?? ""bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 41),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 41),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 48),
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 41),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 41),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 50),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = v => v;
        System.Func<string, string> y = (v) => v;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(5, 19),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 19),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(5, 31)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = new int[10];
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                    this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 17),
                    this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 52)
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = from y in new int[10] select y + 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 15),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 15),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 19)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"public class Foo
{
    public string Bar()
    {
        string foo = """";
        return foo;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1119StatementMustNotUseUnnecessaryParenthesis();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1119CodeFixProvider();
        }
    }
}
