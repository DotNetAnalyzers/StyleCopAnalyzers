namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    public abstract class NumberSignSpacingTestBase : CodeFixVerifier
    {
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

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

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPrefixUnaryOperatorAtEndOfLine()
        {
            string testCode = @"namespace Namespace
{
    class Type
    {
        void Foo()
        {
            int x = " + Sign + @"
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
            int x = " + Sign + @"3;
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
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
                " + Sign + @"3;
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 17)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
" + Sign + @"3;
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 17)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
            int x = " + Sign + @"3;
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, " " + Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 20)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
        public async Task TestPrefixUnaryOperatorAfterBinaryOperator()
        {
            string testFormat = @"namespace Namespace
{{
    class Type
    {{
        void Foo()
        {{
            int x = 1 " + Sign + @" {0};
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
            int x = 1 " + Sign + @" " + Sign + @"3;
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 25)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
            int x = (int)" + Sign + @"3;
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 26)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 27)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
            int x = (" + Sign + @"3);
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 22)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        [TestMethod]
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
            int[] x = new int[" + Sign + @"3];
        }
    }
}
";

            string test;
            DiagnosticResult[] expected;

            test = string.Format(testFormat, Sign + "3");
            expected = EmptyDiagnosticResults;
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + "3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 31)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);

            test = string.Format(testFormat, " " + Sign + " 3");
            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    },
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = SignName + " sign must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 32)
                            }
                    }
                };
            await VerifyCSharpDiagnosticAsync(test, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(test, fixedTest, cancellationToken: CancellationToken.None);
        }

        protected override abstract CodeFixProvider GetCSharpCodeFixProvider();

        protected override abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();
    }
}
