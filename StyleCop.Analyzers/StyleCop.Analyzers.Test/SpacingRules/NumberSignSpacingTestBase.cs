// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Verifiers;
    using Xunit;

    public abstract class NumberSignSpacingTestBase
    {
        protected abstract string Sign
        {
            get;
        }

        protected abstract DiagnosticAnalyzer Analyzer
        {
            get;
        }

        protected abstract CodeFixProvider CodeFix
        {
            get;
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtEndOfLineAsync()
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

            DiagnosticResult expected = this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 21);

            await this.VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtBeginningOfLine_LeadingTriviaAsync()
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

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.Diagnostic().WithArguments(" not", "followed").WithLocation(8, 17);

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAtBeginningOfLine_NoLeadingTriviaAsync()
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

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.Diagnostic().WithArguments(" not", "followed").WithLocation(8, 1);

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterEqualsAsync()
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

            string test = string.Format(testFormat, " " + this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20),
                };

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 21),
                };

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20),
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 20),
                };

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterBinaryOperatorAsync()
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

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 25);

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorAfterCastAsync()
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

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 27),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 26),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 27),
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 27),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorInParenthesesAsync()
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

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 23),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 22),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 23),
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 23),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPrefixUnaryOperatorInBracketsAsync()
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

            string test = string.Format(testFormat, this.Sign + "0");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "0");
            DiagnosticResult[] expected =
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 32),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 0");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 31),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 0");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 32),
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 32),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceBeforeUnaryOperatorInInterpolationAlignmentClauseAsync(string spacing)
        {
            string test = $@"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            string msg = $""{{5,{spacing}{this.Sign}3}}"";
        }}
    }}
}}
";

            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceAfterUnaryOperatorInInterpolationAlignmentClauseAsync()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            string msg = $""{{5,{0}}}"";
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
            string msg = $""{5," + this.Sign + @"3}"";
        }
    }
}
";

            string test = string.Format(testFormat, this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult[] expected =
            {
                this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 31),
            };

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceBeforeAndAfterUnaryOperatorInInterpolationAlignmentClauseAsync()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            string msg = $""{{5,{0}}}"";
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
            string msg = $""{5, " + this.Sign + @"3}"";
        }
    }
}
";

            string test = string.Format(testFormat, " " + this.Sign + "3");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            DiagnosticResult[] expected =
            {
                this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 32),
            };

            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2300, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2300")]
        public async Task TestPrefixUnaryOperatorInInterpolationBracesAsync()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            string x = $""{{{0}}}"";
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
            string x = $""{" + this.Sign + @"0}"";
        }
    }
}
";

            string test = string.Format(testFormat, this.Sign + "0");
            await this.VerifyCSharpDiagnosticAsync(test, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "0");
            DiagnosticResult[] expected =
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 28),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 0");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 27),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 0");
            expected =
                new[]
                {
                    this.Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 28),
                    this.Diagnostic().WithArguments(" not", "followed").WithLocation(7, 28),
                };
            await this.VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        private DiagnosticResult Diagnostic()
            => new DiagnosticResult(this.Analyzer.SupportedDiagnostics.Single());

        private Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(this)
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => this.VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var test = new CSharpTest(this)
            {
                TestCode = source,
                FixedCode = fixedSource,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }

        private class CSharpTest : StyleCopCodeFixVerifier<EmptyDiagnosticAnalyzer, EmptyCodeFixProvider>.CSharpTest
        {
            private readonly NumberSignSpacingTestBase testFixture;

            public CSharpTest(NumberSignSpacingTestBase testFixture)
            {
                this.testFixture = testFixture;
            }

            protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers()
            {
                yield return this.testFixture.Analyzer;
            }

            protected override IEnumerable<CodeFixProvider> GetCodeFixProviders()
            {
                yield return this.testFixture.CodeFix;
            }
        }
    }
}
