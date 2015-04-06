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
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLiteral()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLiteralParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestLiteralDoubleParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestMethodCall()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestMethodCallParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ToString();
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestLocalMember()
        {
            var testCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = Local + Local.IndexOf('x');
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLocalMemberMemberAccess()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public string Local { get; set; }
    public void Bar()
    {
        string x = Local.ToString() + Local.IndexOf('x');
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestLocalMemberAssignment()
        {
            var testCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        this.Local = Local;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLocalMemberAssignmentParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        this.Local = Local;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestLocalMemberParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public int Local { get; set; }
    public void Bar()
    {
        int x = Local;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCast()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = (int)3;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCastParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = (int)3;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCastAssignment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = (int)3;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCastAssignmentParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x;
        x = (int)3;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCastMemberAccess()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ((int)3).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCastMemberAccessAssignment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x;
        x = ((int)3).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestUnaryOperators()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestUnaryOperatorsParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

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
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCheckedUnchecked()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * checked(5);
        x = 3 * unchecked(5);
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCheckedUncheckedParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int x = 3 * checked(5);
        x = 3 * unchecked(5);
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestNameOf()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = nameof(Foo) + ""Bar"";
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestNameOfParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = nameof(Foo) + ""Bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestIsExpression()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        bool x = """" is string;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIsExpressionMemberAccess()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ("""" is string).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestIsExpressionParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        bool x = """" is string;
        x = """" is string;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestAssignment()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = y = ""foo"";
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAssignmentParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string y;
        string x = y = ""foo"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestInnerAssignment()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConditional()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = true ? ""foo"" : ""bar"";
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestConditionalParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = true ? ""foo"" : ""bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestConditionalInner()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (true ? ""foo"" : ""bar"") + ""test"";
        string y = (true ? ""foo"" : ""bar"").ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCoalesce()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ""foo"" ?? ""bar"";
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestCoalesceParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        string x = ""foo"" ?? ""bar"";
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCoalesceInner()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (""foo"" ?? ""bar"") + ""test"";
        string y = (""foo"" ?? ""bar"").ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambda()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = v => v;
        System.Func<string, string> y = (v) => v;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestLambdaParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        System.Func<string, string> x = v => v;
        System.Func<string, string> y = (v) => v;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestLambdaInner()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = ((System.Func<string, string>)(v => v))(""foo"");
        string y = ((System.Func<string, string>)((v) => v))(""foo"");
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArray()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = new int[10];
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestArrayParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"public class Foo
{
    public void Bar()
    {
        int[] x = new int[10];
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestArrayInner()
        {
            var testCode = @"public class Foo
{
    public void Bar()
    {
        string x = (new int[10]).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestQuery()
        {
            var testCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = from y in new int[10] select y + 1;
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestQueryParenthesis()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);

            var fixedCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = from y in new int[10] select y + 1;
    }
}";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestQueryInner()
        {
            var testCode = @"using System.Linq;
public class Foo
{
    public void Bar()
    {
        var x = (from y in new int[10] select y + 1).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestAwaitInner()
        {
            var testCode = @"public class Foo
{
    public async void Bar()
    {
        var x = (await System.Threading.Tasks.Task.Delay(10).ContinueWith(task => System.Threading.Tasks.Task.FromResult(1))).ToString();
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestBinary()
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestInnerTriviaCodeFix()
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
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
        }

        [Fact]
        public async Task TestCodeFixWorksInTrivia()
        {
            var testCode = @"#if (NET20 || NET35)
// Foo
#endif";
            var fixedCode = @"#if NET20 || NET35
// Foo
#endif";
            await this.VerifyCSharpFixAsync(testCode, fixedCode);
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
