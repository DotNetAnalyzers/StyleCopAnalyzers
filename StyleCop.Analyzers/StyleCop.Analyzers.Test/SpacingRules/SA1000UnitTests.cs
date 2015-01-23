namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1000KeywordsMustBeSpacedCorrectly"/> and
    /// <see cref="SA1000CodeFixProvider"/>.
    /// </summary>
    [TestClass]
    public class SA1000UnitTests : CodeFixVerifier
    {
        public string DiagnosticId { get; } = SA1000KeywordsMustBeSpacedCorrectly.DiagnosticId;

        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = @"";
            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestCatchallStatement()
        {
            string statement = @"try
{
}
catch
{
}
";

            await TestKeywordStatement(statement, EmptyDiagnosticResults, statement);
        }

        [TestMethod]
        public async Task TestCatchStatement()
        {
            string statementWithoutSpace = @"try
{
}
catch(Exception ex)
{
}
";

            string statementWithSpace = @"try
{
}
catch (Exception ex)
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'catch' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestFixedStatement()
        {
            string statementWithoutSpace = @"fixed(byte* b = &y)
{
}
";

            string statementWithSpace = @"fixed (byte* b = &y)
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'fixed' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestForStatement()
        {
            string statementWithoutSpace = @"for(int x = 0; x < 10; x++)
{
}
";

            string statementWithSpace = @"for (int x = 0; x < 10; x++)
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'for' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestForeachStatement()
        {
            string statementWithoutSpace = @"foreach(int x in new int[0])
{
}
";

            string statementWithSpace = @"foreach (int x in new int[0])
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'foreach' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestFromStatement()
        {
            string statementWithoutSpace = @"var result = from@x in y select x;";

            string statementWithSpace = @"var result = from @x in y select x;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'from' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 26)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestGroupStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
group@x by x.A into z
select z;";

            string statementWithSpace = @"var result = from x in y
group @x by x.A into z
select z;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'group' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestIfStatement()
        {
            string statementWithoutSpace = @"if(true)
{
}
";

            string statementWithSpace = @"if (true)
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'if' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestInStatement()
        {
            string statementWithoutSpace = @"var result = from x in@y select x;";

            string statementWithSpace = @"var result = from x in @y select x;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'in' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 33)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestIntoStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
group x by x.A into@z
select z;";

            string statementWithSpace = @"var result = from x in y
group x by x.A into @z
select z;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'into' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 16)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestJoinStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
join@a in b on x.A equals a.B
select z;";

            string statementWithSpace = @"var result = from x in y
join @a in b on x.A equals a.B
select z;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'join' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 16)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestLetStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
let@z = 3
select x;";

            string statementWithSpace = @"var result = from x in y
let @z = 3
select x;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'let' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestLockStatement()
        {
            string statementWithoutSpace = @"lock(new object())
{
}
";

            string statementWithSpace = @"lock (new object())
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'lock' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestOrderbyStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
orderby(x.A)
select z;";

            string statementWithSpace = @"var result = from x in y
orderby (x.A)
select z;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'orderby' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestReturnVoidStatement()
        {
            string statementWithoutSpace = @"return;";

            string statementWithSpace = @"return ;";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'return' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestReturnIntStatement()
        {
            string statementWithoutSpace = @"return(3);";

            string statementWithSpace = @"return (3);";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'return' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestSelectStatement()
        {
            string statementWithoutSpace = @"var result = from x in y select@x;";

            string statementWithSpace = @"var result = from x in y select @x;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'select' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 38)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestStackallocStatement()
        {
            string statementWithoutSpace = @"int* x = stackalloc@Int32[3];";

            string statementWithSpace = @"int* x = stackalloc @Int32[3];";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'stackalloc' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 22)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestSwitchStatement()
        {
            string statementWithoutSpace = @"switch(3)
{
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
default:
    break;
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'switch' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestThrowStatement()
        {
            string statementWithoutSpace = @"throw(new Exception());";

            string statementWithSpace = @"throw (new Exception());";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'throw' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestRethrowStatement()
        {
            string statementWithoutSpace = @"try
{
}
catch (Exception ex)
{
    throw;
}
";

            string statementWithSpace = @"try
{
}
catch (Exception ex)
{
    throw ;
}
";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'throw' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 12, 5)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestUsingStatement()
        {
            string statementWithoutSpace = @"using(default(IDisposable))
{
}
";

            string statementWithSpace = @"using (default(IDisposable))
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'using' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestWhereStatement()
        {
            string statementWithoutSpace = @"var result = from x in y
where(x.A)
select z;";

            string statementWithSpace = @"var result = from x in y
where (x.A)
select z;";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'where' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 8, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestWhileStatement()
        {
            string statementWithoutSpace = @"while(false)
{
}
";

            string statementWithSpace = @"while (false)
{
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'while' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 13)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        [Ignore]
        public void TestYieldStatement()
        {
            // There is no way to have a 'yield' keyword which is not followed by a space.
            Assert.Inconclusive("Not yet implemented.");
        }

        [TestMethod]
        public async Task TestCheckedStatement()
        {
            string statementWithoutSpace = @"int x = checked(3);";

            string statementWithSpace = @"int x = checked (3);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'checked' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestDefaultCaseStatement()
        {
            string statementWithoutSpace = @"switch (3)
{
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
default :
    break;
}
";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'default' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestDefaultValueStatement()
        {
            string statementWithoutSpace = @"int x = default(int);";

            string statementWithSpace = @"int x = default (int);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'default' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNameofStatement()
        {
            string statementWithoutSpace = @"string x = nameof(x);";

            string statementWithSpace = @"string x = nameof (x);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'nameof' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 24)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestSizeofStatement()
        {
            string statementWithoutSpace = @"int x = sizeof(int);";

            string statementWithSpace = @"int x = sizeof (int);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'sizeof' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestTypeofStatement()
        {
            string statementWithoutSpace = @"Type x = typeof(int);";

            string statementWithSpace = @"Type x = typeof (int);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'typeof' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 22)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestUncheckedStatement()
        {
            string statementWithoutSpace = @"int x = unchecked(3);";

            string statementWithSpace = @"int x = unchecked (3);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'unchecked' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewObjectStatement()
        {
            string statementWithoutSpace = @"int x = new@Int32();";

            string statementWithSpace = @"int x = new @Int32();";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 21)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestNewArrayStatement()
        {
            string statementWithoutSpace = @"int[] x = new@Int32[3];";

            string statementWithSpace = @"int[] x = new @Int32[3];";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 23)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestNewImplicitArrayStatement()
        {
            string statementWithoutSpace = @"int[] x = new[] { 3 };";

            string statementWithSpace = @"int[] x = new [] { 3 };";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace);

            // this case is handled by SA1026, so it shouldn't be reported here
            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_Type()
        {
            string statementWithSpace = @"public class Foo<T> where T : new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 43)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_TypeWithMultipleConstraints()
        {
            string statementWithSpace = @"public class Foo<T> where T : X, new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : X, new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 46)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_TypeWithClassConstraints()
        {
            string statementWithSpace = @"public class Foo<T> where T : class, new ()
{
}";

            string statementWithoutSpace = @"public class Foo<T> where T : class, new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 50)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_Method()
        {
            string statementWithSpace = @"public void Foo<T>() where T : new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 44)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_MethodWithMultipleConstraints()
        {
            string statementWithSpace = @"public void Foo<T>() where T : X, new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : X, new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 47)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestNewConstructorContraintStatement_MethodWithClassConstraints()
        {
            string statementWithSpace = @"public void Foo<T>() where T : class, new ()
{
}";

            string statementWithoutSpace = @"public void Foo<T>() where T : class, new()
{
}";

            await VerifyCSharpDiagnosticAsync(statementWithoutSpace, EmptyDiagnosticResults, CancellationToken.None);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'new' must not be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 51)
                            }
                    }
                };

            await TestKeywordStatement(statementWithSpace, expected, statementWithoutSpace);
        }

        [TestMethod]
        public async Task TestAwaitIdentifier()
        {
            string statementWithoutSpace = @"var result = await(x);";

            string statementWithSpace = @"var result = await (x);";

            await TestKeywordStatement(statementWithoutSpace, EmptyDiagnosticResults, statementWithoutSpace, asyncMethod: false);
            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, asyncMethod: false);
        }

        [TestMethod]
        public async Task TestAwaitStatement()
        {
            string statementWithoutSpace = @"var result = await(x);";

            string statementWithSpace = @"var result = await (x);";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace, asyncMethod: true);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'await' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 26)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace, asyncMethod: true);
        }

        [TestMethod]
        public async Task TestCaseStatement()
        {
            string statementWithoutSpace = @"switch (3)
{
case(3):
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
case (3):
default:
    break;
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'case' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 9, 1)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestGotoCaseStatement()
        {
            string statementWithoutSpace = @"switch (3)
{
case 2:
    goto case(3);

case 3:
default:
    break;
}
";

            string statementWithSpace = @"switch (3)
{
case 2:
    goto case (3);

case 3:
default:
    break;
}
";

            await TestKeywordStatement(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "The keyword 'case' must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 10, 10)
                            }
                    }
                };

            await TestKeywordStatement(statementWithoutSpace, expected, statementWithSpace);
        }

        private async Task TestKeywordStatement(string statement, DiagnosticResult[] expected, string fixedStatement, bool asyncMethod = false)
        {
            string testCodeFormat = @"namespace Namespace
{{
    class Type
    {{
        {0}void Foo()
        {{
            {1}
        }}
    }}
}}
";

            string asyncModifier = asyncMethod ? "async " : string.Empty;
            string testCode = string.Format(testCodeFormat, asyncModifier, statement);
            string fixedTest = string.Format(testCodeFormat, asyncModifier, fixedStatement);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(testCode, fixedTest, cancellationToken: CancellationToken.None);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1000CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1000KeywordsMustBeSpacedCorrectly();
        }
    }
}
