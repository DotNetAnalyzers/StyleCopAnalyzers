// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1503CurlyBracketsMustNotBeOmitted"/>.
    /// </summary>
    public class SA1503UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Gets the statements that will be used in the theory test cases.
        /// </summary>
        /// <value>
        /// The statements that will be used in the theory test cases.
        /// </value>
        public static IEnumerable<object[]> TestStatements
        {
            get
            {
                yield return new[] { "if (i == 0)" };
                yield return new[] { "while (i == 0)" };
                yield return new[] { "for (var j = 0; j < i; j++)" };
                yield return new[] { "foreach (var j in new[] { 1, 2, 3 })" };
                yield return new[] { "lock (this)" };
                yield return new[] { "using (this)" };
                yield return new[] { "fixed (byte* ptr = new byte[10])" };
            }
        }

        /// <summary>
        /// Verifies that a statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestStatementWithoutCurlyBracketsAsync(string statementText)
        {
            var expected = this.CSharpDiagnostic().WithLocation(7, 13);
            await this.VerifyCSharpDiagnosticAsync(this.GenerateTestStatement(statementText), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a <c>do</c> statement followed by a block without curly braces will produce a warning, and the
        /// code fix for this warning results in valid code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestDoStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        do
            Debug.Assert(true);
        while (false);
    }
}";
            var fixedCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        do
        {
            Debug.Assert(true);
        }
        while (false);
    }
}";

            var expected = this.CSharpDiagnostic().WithLocation(7, 13);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestStatementWithCurlyBracketsAsync(string statementText)
        {
            await this.VerifyCSharpDiagnosticAsync(this.GenerateFixedTestStatement(statementText), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if / else statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);
    }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(7, 13),
                this.CSharpDiagnostic().WithLocation(9, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a block with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseStatementWithCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if statement followed by a else if, followed by an else statement, all blocks with curly braces will produce no diagnostics results.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestIfElseIfElseStatementWithCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else if (i == 1)
        {
            Debug.Assert(false);
        }
        else
        {
            Debug.Assert(true);
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested if statements followed by a block without curly braces will produce warnings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleIfStatementsWithoutCurlyBracketsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(6, 21),
                this.CSharpDiagnostic().WithLocation(6, 33)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderForIfElseStatementAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);
    }
}";

            // While it looks like only one instance is fixed, what actually happened is fixing the first warning caused
            // the second warning to change from SA1503 to SA1520. Since the testing framework is only testing SA1503,
            // from its perspective all warnings have been corrected so it stops iterating.
            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
            Debug.Assert(false);
    }
}";

            // The batch fixer does correct all problems in one pass.
            var batchFixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            Debug.Assert(true);
        }
        else
        {
            Debug.Assert(false);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, batchNewSource: batchFixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will properly handle alternate indentations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithAlternateIndentationAsync()
        {
            this.IndentationSize = 1;

            var testCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
 }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
 public void Bar(int i)
 {
  if (i == 0)
  {
   Debug.Assert(true);
  }
 }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly handle non-whitespace trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithNonWhitespaceTriviaAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
            Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
        {
            Debug.Assert(true);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly handle multiple cases of missing brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderWithMultipleNestingsAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (i == 0)
        {
            if (i == 0)
            {
                Debug.Assert(true);
            }
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will not perform a code fix when statement contains a preprocessor directive.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixWithPreprocessorDirectivesAsync()
        {
            var testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        if (true)
#if !DEBUG
            Debug.WriteLine(""Foobar"");
#endif
        Debug.WriteLine(""Foobar 2"");
    }
}";

            await this.VerifyCSharpFixAsync(testCode, testCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1503CurlyBracketsMustNotBeOmitted();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1503CodeFixProvider();
        }

        /// <summary>
        /// Verifies that the codefix provider will work properly for a statement.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory, MemberData(nameof(TestStatements))]
        private async Task TestCodeFixForStatementAsync(string statementText)
        {
            await this.VerifyCSharpFixAsync(this.GenerateTestStatement(statementText), this.GenerateFixedTestStatement(statementText)).ConfigureAwait(false);
        }

        private string GenerateTestStatement(string statementText)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class Foo : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT#
            Debug.Assert(true);
    }

    public void Dispose() {}
}";
            return testCodeFormat.Replace("#STATEMENT#", statementText);
        }

        private string GenerateFixedTestStatement(string statementText)
        {
            var fixedTestCodeFormat = @"using System.Diagnostics;
public class Foo : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT#
        {
            Debug.Assert(true);
        }
    }

    public void Dispose() {}
}";
            return fixedTestCodeFormat.Replace("#STATEMENT#", statementText);
        }
    }
}
