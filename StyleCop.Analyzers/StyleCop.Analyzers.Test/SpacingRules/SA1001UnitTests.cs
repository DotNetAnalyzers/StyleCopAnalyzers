// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1001CommasMustBeSpacedCorrectly"/> and
    /// <see cref="TokenSpacingCodeFixProvider"/>.
    /// </summary>
    public class SA1001UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 16);

            await this.TestCommaInStatementOrDeclAsync(statementWithoutSpace, expected, statementWithSpace).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaAsync()
        {
            string spaceBeforeComma = @"f(a , b);";
            string spaceOnlyAfterComma = @"f(a, b);";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 17);

            await this.TestCommaInStatementOrDeclAsync(spaceBeforeComma, expected, spaceOnlyAfterComma).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaAtEndOfLineAsync()
        {
            string spaceBeforeComma = $"f(a ,{Environment.NewLine}b);";
            string spaceOnlyAfterComma = $"f(a,{Environment.NewLine}b);";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 17);

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
            string statement = $"f(a{Environment.NewLine}, b);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommentBeforeFirstCommaInLineAsync()
        {
            string statement = $"f(a // comment{Environment.NewLine}, b);";
            await this.TestCommaInStatementOrDeclAsync(statement, EmptyDiagnosticResults, statement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceBeforeCommaFollowedByAngleBracketInFuncTypeAsync()
        {
            string statement = @"var a = typeof(System.Func< ,>);";
            string fixedStatement = @"var a = typeof(System.Func<,>);";

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 41);

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
            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 42);

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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 19);

            await this.TestCommaInStatementOrDeclAsync(statement, expected, fixedStatement).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSpaceOnlyBeforeCommaAsync()
        {
            string spaceOnlyBeforeComma = @"f(a ,b);";
            string spaceOnlyAfterComma = @"f(a, b);";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithArguments(" not", "preceded").WithLocation(7, 17),
                this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(7, 17)
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

            DiagnosticResult[] expected =
            {
                new DiagnosticResult
                {
                    Id = "CS1003",
                    Severity = DiagnosticSeverity.Error,
                    Message = "Syntax error, ',' expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 25) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithArguments(string.Empty, "followed").WithLocation(6, 24);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1001CommasMustBeSpacedCorrectly();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
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

            await this.VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(originalCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
