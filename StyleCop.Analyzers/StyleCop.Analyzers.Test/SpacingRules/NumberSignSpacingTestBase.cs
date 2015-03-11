namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Xunit;
    using TestHelper;

    public abstract class NumberSignSpacingTestBase : CodeFixVerifier
    {
        protected abstract string Sign
        {
            get;
        }

        protected abstract string SignName
        {
            get;
        }

        protected abstract string DiagnosticId
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

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 17)
                            }
                    }
                };
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 17)
                            }
                    }
                };
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, " " + this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    }
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 25)
                            }
                    }
                };
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 26)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    }
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
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 22)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    }
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
            int[] x = new int[" + this.Sign + @"3];
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, this.Sign + "3");
            expected = EmptyDiagnosticResults;
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 31)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + this.Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = this.DiagnosticId,
                        Message = this.SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    }
                };
            await this.VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await this.VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        protected override abstract CodeFixProvider GetCSharpCodeFixProvider();

        protected override abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();
    }
}
