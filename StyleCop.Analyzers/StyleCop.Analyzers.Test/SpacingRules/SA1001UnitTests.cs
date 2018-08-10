// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1001CommasMustBeSpacedCorrectly"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1001UnitTests
    {
        [Fact]
        public async Task TestSpaceAfterCommaAsync()
        {
            string statement = "f(a, b);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNoSpaceAfterCommaAsync()
        {
            string statementWithoutSpace = @"f(a,b);";
            string statementWithSpace = @"f(a, b);";

            await this.TestCommaInStatementOrDeclAsync(statementWithSpace, EmptyDiagnosticResults, statementWithSpace).ConfigureAwait(false);

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 16);

            await this.TestCommaInStatementOrDeclAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaAsync()
        {
            string spaceBeforeComma = @"f(a , b);";
            string spaceOnlyAfterComma = @"f(a, b);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 17);

            await this.TestCommaInStatementOrDeclAsync(spaceBeforeComma, expected, spaceOnlyAfterComma).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaAtEndOfLineAsync()
        {
            string spaceBeforeComma = $"f(a ,{Environment.NewLine}b);";
            string spaceOnlyAfterComma = $"f(a,{Environment.NewLine}b);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 17);

            await this.TestCommaInStatementOrDeclAsync(spaceBeforeComma, expected, spaceOnlyAfterComma).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLastCommaInLineAsync()
        {
            string statement = $"f(a,{Environment.NewLine}b);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFirstCommaInLineAsync()
        {
            string testStatement = $"f(a{Environment.NewLine}, b);";
            string fixedStatement = $"f(a,{Environment.NewLine}b);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 1);

            await this.TestCommaInStatementOrDeclAsync(testStatement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommentBeforeFirstCommaInLineAsync()
        {
            string testStatement = $"f(a // comment{Environment.NewLine}, b);";
            string fixedStatement = $"f(a, // comment{Environment.NewLine}b);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 1);

            await this.TestCommaInStatementOrDeclAsync(testStatement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaFollowedByAngleBracketInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func< ,>);";
            string fixedStatement = @"var a = typeof(System.Func<,>);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedByAngleBracketInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func<,>);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedBySpaceFollowedByAngleBracketInFuncTypeAsync()
        {
            // This is correct by SA1001, and reported as an error by SA1015
            string statement = @"var a = typeof(System.Func<, >);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaFollowedByCommaInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func< ,,>);";
            string fixedStatement = @"var a = typeof(System.Func<,,>);";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedByCommaInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func<,,>);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedBySpaceFollowedByCommaInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func<, ,>);";
            string fixedStatement = @"var a = typeof(System.Func<,,>);";
            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 42);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedByBracketInArrayDeclAsync()
        {
            string statement = @"int[,] myArray;";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaFollowedByBracketInArrayDeclAsync()
        {
            string statement = @"int[, ,] myArray;";
            string fixedStatement = @"int[,,] myArray;";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 19);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceBeforeCommaWhenPartOfInterpolationAlignmentClauseAsync()
        {
            string statement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2] ,3}"";";
            string fixedStatement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2],3}"";";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 29);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceAfterCommaWhenPartOfInterpolationAlignmentClauseAsync()
        {
            string statement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2], 3}"";";
            string fixedStatement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2],3}"";";

            DiagnosticResult expected = Diagnostic().WithArguments(" not", "followed").WithLocation(8, 28);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceBeforeAndAfterCommaWhenPartOfInterpolationAlignmentClauseAsync()
        {
            string statement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2] , 3}"";";
            string fixedStatement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2],3}"";";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "preceded").WithLocation(8, 29),
                    Diagnostic().WithArguments(" not", "followed").WithLocation(8, 29),
                };

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceAfterCommaWithMinusWhenPartOfInterpolationAlignmentClauseAsync()
        {
            string statement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2], -3}"";";
            string fixedStatement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2],-3}"";";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(8, 28),
                };

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2289, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2289")]
        public async Task TestSpaceAfterCommaWithPlusWhenPartOfInterpolationAlignmentClauseAsync()
        {
            string statement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2], +3}"";";
            string fixedStatement = @"var x = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var t = $""{x[2],+3}"";";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithArguments(" not", "followed").WithLocation(8, 28),
                };

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceOnlyBeforeCommaAsync()
        {
            string spaceOnlyBeforeComma = @"f(a ,b);";
            string spaceOnlyAfterComma = @"f(a, b);";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(7, 17),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 17),
            };

            await this.TestCommaInStatementOrDeclAsync(spaceOnlyBeforeComma, expected, spaceOnlyAfterComma).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingCommaAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        int[] exp = { 0 0 };
    }
}
";

            DiagnosticResult expected = CompilerError("CS1003").WithMessage("Syntax error, ',' expected").WithLocation(6, 25);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommaFollowedByCommentAsync()
        {
            string testCode = @"
class ClassName
{
    void MethodName()
    {
        int[] exp = { 0,/*comment*/ 0 };
    }
}
";
            string fixedCode = @"
class ClassName
{
    void MethodName()
    {
        int[] exp = { 0, /*comment*/ 0 };
    }
}
";

            DiagnosticResult expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 24);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2468, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2468")]
        public async Task TestCodeFixCommaPlacementAsync()
        {
            var testCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test = (new[]
        {
            new Tuple<int, int>(1, 2)
           ,new Tuple<int, int>(3, 4)
           ,new Tuple<int, int>(5, 6)
        }).ToString();
    }
}
";

            var fixedCode = @"using System;

public class TestClass
{
    public void TestMethod()
    {
        var test = (new[]
        {
            new Tuple<int, int>(1, 2),
           new Tuple<int, int>(3, 4),
           new Tuple<int, int>(5, 6)
        }).ToString();
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(" not", "preceded").WithLocation(10, 12),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(10, 12),
                Diagnostic().WithArguments(" not", "preceded").WithLocation(11, 12),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(11, 12),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private Task TestCommaInStatementOrDeclAsync(string originalStatement, DiagnosticResult expected, string fixedStatement)
        {
            return this.TestCommaInStatementOrDeclAsync(originalStatement, new[] { expected }, fixedStatement);
        }

        private async Task TestCommaInStatementOrDeclAsync(string originalStatement, DiagnosticResult[] expected, string fixedStatement)
        {
            string template = @"namespace Foo
{{
    class Bar
    {{
        void Baz()
        {{
            {0}
        }}
        // The following fields and method are referenced by the tests and need definitions.
        int a, b;
        void f(int x, int y) {{ }}
    }}
}}
";
            string originalCode = string.Format(template, originalStatement);
            string fixedCode = string.Format(template, fixedStatement);

            var test = new CSharpTest
            {
                TestCode = originalCode,
                FixedCode = fixedCode,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
