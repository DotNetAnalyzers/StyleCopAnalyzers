// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;

    public abstract class NumberSignSpacingTestBase<TAnalyzer, TCodeFix>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        protected abstract string Sign
        {
            get;
        }

        private static DiagnosticResult[] EmptyDiagnosticResults
            => DiagnosticVerifier<TAnalyzer>.EmptyDiagnosticResults;

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

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "followed").WithLocation(7, 21);

            await VerifyCSharpFixAsync(testCode, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = Diagnostic().WithArguments(" not", "followed").WithLocation(8, 17);

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = Diagnostic().WithArguments(" not", "followed").WithLocation(8, 1);

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20),
                };

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 21),
                };

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(7, 20),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 20),
                };

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult expected = Diagnostic().WithArguments(" not", "followed").WithLocation(7, 25);

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 27),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 26),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 27),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 27),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "3");
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 23),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 22),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 23),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 23),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "0");
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 32),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 0");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 31),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 0");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 32),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 32),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 3");
            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "followed").WithLocation(7, 31),
            };

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "followed").WithLocation(7, 32),
            };

            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(test, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + "0");
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 28),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, this.Sign + " 0");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 27),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);

            test = string.Format(testFormat, " " + this.Sign + " 0");
            expected =
                new[]
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 28),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(7, 28),
                };
            await VerifyCSharpFixAsync(test, expected, fixedTest, CancellationToken.None).ConfigureAwait(false);
        }

        private static DiagnosticResult Diagnostic()
            => StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>.Diagnostic();

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => StyleCopDiagnosticVerifier<TAnalyzer>.VerifyCSharpDiagnosticAsync(source, expected, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult expected, string fixedSource, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, new[] { expected }, fixedSource, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
            => StyleCopCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCSharpFixAsync(source, expected, fixedSource, cancellationToken);
    }
}
