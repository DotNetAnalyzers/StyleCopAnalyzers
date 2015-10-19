// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1119UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.DiagnosticId;
        private const string ParenthesesDiagnosticId = SA1119StatementMustNotUseUnnecessaryParenthesis.ParenthesesDiagnosticId;

        private bool mainDiagnosticShouldBeDisabled = false;

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
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 26),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 26),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 31)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        public async Task TestParenthesisDiagnosticIsNotTriggeredIfParentDiagnosticIsDisabledAsync()
        {
            this.mainDiagnosticShouldBeDisabled = true;

            string testCode = @"class Foo
{
    public void Bar()
    {
        int i = (1 + 1);
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 23),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 23),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 42)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 35)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 30)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic(DiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 22),
                this.CSharpDiagnostic(ParenthesesDiagnosticId).WithLocation(6, 29)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1119StatementMustNotUseUnnecessaryParenthesis();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1119CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.mainDiagnosticShouldBeDisabled)
            {
                yield return DiagnosticId;
            }
        }
    }
}
