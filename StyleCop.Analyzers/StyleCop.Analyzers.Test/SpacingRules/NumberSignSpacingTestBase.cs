namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public abstract class NumberSignSpacingTestBase : CodeFixVerifier
    {
        protected abstract string Sign
        {
            get;
        }

        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtEndOfLine()
        {
            string testCode = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = " + this.Sign + @"
3;
        }
    }
}
";

            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = " + this.Sign + @"3;
        }
    }
}
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 21);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtBeginningOfLine_LeadingTrivia()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x =
                {0};
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x =
                " + this.Sign + @"3;
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(8, 17);

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtBeginningOfLine_NoLeadingTrivia()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x =
{0};
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x =
" + this.Sign + @"3;
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterEquals()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x ={0};
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = " + this.Sign + @"3;
        }
    }
}
";

            string test;

            test = string.Format(testFormat, " " + this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20)
                };

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 21)
                };

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20),
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 20)
                };

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterBinaryOperator()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x = 1 " + this.Sign + @" {0};
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = 1 " + this.Sign + @" " + this.Sign + @"3;
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 25);

            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterCast()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x = (int){0};
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = (int)" + this.Sign + @"3;
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 27)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 26)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 27),
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 27)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorInParentheses()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x = ({0});
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = (" + this.Sign + @"3);
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 23)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 22)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 23),
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 23)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorInBrackets()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int[] x = new int[{0}];
        }}
    }}
}}
";

            // in all cases the final output should be the following
            string fixedTest = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int[] x = new int[" + this.Sign + @"0];
        }
    }
}
";

            string test;

            test = string.Format(testFormat, this.Sign + "0");
            await this.VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "0");
            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 32)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 0");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 31)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 0");
            expected =
                new[]
                {
                    this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 32),
                    this.CSharpDiagnostic().WithArguments(" not", "followed").WithLocation(7, 32)
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        protected override abstract CodeFixProvider GetCSharpCodeFixProvider();

        protected override abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();
    }
}
