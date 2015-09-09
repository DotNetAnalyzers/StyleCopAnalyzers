// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    public class SA1501UnitTests : CodeFixVerifier
    {
        private bool suppressSA1503 = false;

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
        /// Verifies that lock statement with single line block statement will trigger a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLockWithSingleLineBlockAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) { Debug.Assert(true); }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(6, 21), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a lock statement with preprocessor trivia will trigger the correct warnings.
        /// </summary>
        /// <remarks>
        /// The analyzer will only trigger on the second block, as the first block will be marked as DisabledTextTrivia due to the preprocessor statements.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLockWithPreProcessorTriviaAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) 
#if MYTEST 
        { Debug.Assert(true); } 
#else 
        { Debug.Assert(false); } 
#endif
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, this.CSharpDiagnostic().WithLocation(10, 9), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that lock statement with a block statement spread over multiple lines will not trigger a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLockWithMultilineBlockAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) 
        { 
            Debug.Assert(true); 
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that lock statement with an invalid formatted block statement spread over multiple lines will not trigger a warning.
        /// </summary>
        /// <remarks>This will trigger SA1500.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLockWithInvalidMultilineBlockAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) { Debug.Assert(true); 
        }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line anonymous method definition is allowed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineAnonymousMethodIsAllowedAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    delegate void MyDelegate(int x);

    public void Bar(int i)
    {
        MyDelegate d = delegate(int x) { Debug.WriteLine(x); };
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line lambda expression definition is allowed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineLambdaExpressionIsAllowedAsync()
        {
            string testCode = @"using System;
using System.Diagnostics;

public class Foo
{
    public void Bar(int i)
    {
        var test = new Action<int>(value => { Debug.WriteLine(value); });
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line method definition is not flagged by this analyzer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineMethodIsAllowedAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar() { Debug.Assert(true); }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line property accessors are not flagged by this analyzer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLinePropertyAccessorsAreAllowedAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public int Bar
    {
        get { return 0; }
        set { }
    }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this)
          { Debug.Assert(true); }
    }
}";
            string fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this)
        {
            Debug.Assert(true);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement, when it starts on the same line as the parent.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockWithParentOnSameLineAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) { Debug.Assert(true); }
    }
}";
            string fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this)
        {
            Debug.Assert(true);
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement, when it starts on the same line as the parent.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockInSourceFileWithTabsAsync()
        {
            this.UseTabs = true;

            string testCode =
                "using System.Diagnostics;\r\n" +
                "public class Foo\r\n" +
                "{\r\n" +
                "\tpublic void Bar(int i)\r\n" +
                "\t{\r\n" +
                "\t\tlock (this) { Debug.Assert(true); }\r\n" +
                "\t}\r\n" +
                "}\r\n";

            string fixedTestCode =
                "using System.Diagnostics;\r\n" +
                "public class Foo\r\n" +
                "{\r\n" +
                "\tpublic void Bar(int i)\r\n" +
                "\t{\r\n" +
                "\t\tlock (this)\r\n" +
                "\t\t{\r\n" +
                "\t\t\tDebug.Assert(true);\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n" +
                "}\r\n";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle non-whitespace trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyHandlesTriviaAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) /* comment */ { /* comment2 */ Debug.Assert(true); /* comment3 */ } /* comment4 */
    }
}";
            string fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) /* comment */
        { /* comment2 */
            Debug.Assert(true); /* comment3 */
        } /* comment4 */
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle preprocessor trivia.
        /// </summary>
        /// <remarks>
        /// Only the second block will be fixed, as the first block is marked as DisabledTextTrivia.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyHandlesPreProcessorTriviaAsync()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this)
#if MYTEST
        { Debug.Assert(true); }
#else
        { Debug.Assert(false); }
#endif
    }
}";

            string fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this)
#if MYTEST
        { Debug.Assert(true); }
#else
        {
            Debug.Assert(false);
        }
#endif
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single line expression with a lambda expression or anonymous method will not trigger this diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestSingleLineExpressionWithLambdaOrAnonymousMethodAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        Func<int> test = () => { return 5; }; 
        EventHandler d = delegate(object s, EventArgs e) { };
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multiple line expression with a single line lambda expression or a single line anonymous method will trigger the expected diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMultipleLineExpressionWithSingleLineLambdaOrAnonymousMethodAsync()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        Func<int> test = () =>
            { return 5; };

        EventHandler d = delegate (object s, EventArgs e)
            { };
    }
}
";

            var fixedTestCode = @"using System;

public class Foo
{
    public void Bar()
    {
        Func<int> test = () =>
            {
                return 5;
            };

        EventHandler d = delegate (object s, EventArgs e)
            {
            };
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(8, 13),
                this.CSharpDiagnostic().WithLocation(11, 13)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Test for issue 1172, https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1172
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRegressionIssue1172Async()
        {
            var testCode = @"using System;

public class Foo
{
    public void Bar()
    {
        do { Bar(); } while (false);
    }
}
";

            var fixedTestCode = @"using System;

public class Foo
{
    public void Bar()
    {
        do
        {
            Bar();
        }
        while (false);
    }
}
";

            DiagnosticResult expectedDiagnostics = this.CSharpDiagnostic().WithLocation(7, 12);

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Test for issue 1172, https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1172
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestRegressionIssue1172WithHalfClassAsync()
        {
            var testCode = @"using System;

class TypeName
{
    void MethodName()
    {
        do { Bar(); } while (false);
";

            var fixedTestCode = @"using System;

class TypeName
{
    void MethodName()
    {
        do
        {
            Bar();
        }
        while (false);
";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestNoSA1503StatementWithoutCurlyBracketsAsync(string statementText)
        {
            this.suppressSA1503 = true;

            var expected = this.CSharpDiagnostic().WithLocation(6, statementText.Length + 10);
            await this.VerifyCSharpDiagnosticAsync(this.GenerateTestStatement(statementText), expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a <c>do</c> statement followed by a block without curly braces will produce a warning, and the
        /// code fix for this warning results in valid code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503DoStatementAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        do Debug.Assert(true);
        while (false);
    }
}";
            var fixedCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        do
            Debug.Assert(true);
        while (false);
    }
}";

            var expected = this.CSharpDiagnostic().WithLocation(6, 12);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that consecutive single-line statements are properly reported and fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503ConsecutiveStatementsAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System;
using System.Diagnostics;
public class TypeName
{
    public void Bar(string x, string y)
    {
        if (x == null) throw new ArgumentNullException(nameof(x));
        if (y == null) throw new ArgumentNullException(nameof(y));
    }
}";
            var fixedCode = @"using System;
using System.Diagnostics;
public class TypeName
{
    public void Bar(string x, string y)
    {
        if (x == null)
            throw new ArgumentNullException(nameof(x));
        if (y == null)
            throw new ArgumentNullException(nameof(y));
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(7, 24),
                this.CSharpDiagnostic().WithLocation(8, 24),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the behavior of SA1503 when SA1503 is suppressed for many different forms of <c>if</c> statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503IfStatementsAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else Debug.Assert(false);//8

        if (i == 1) Debug.Assert(true); else if (i == 2) Debug.Assert(false);//10

        if (i == 3)
            Debug.Assert(true);
        else if (i == 4) Debug.Assert(false);//14

        if (i == 5) Debug.Assert(true); else if (i == 6) Debug.Assert(false); else Debug.Assert(false);//16

        if (i == 7) if (i == 8) Debug.Assert(false); else Debug.Assert(false); else Debug.Assert(true);//18

        if (i == 9)
            if (i == 10) Debug.Assert(false); else Debug.Assert(false); else Debug.Assert(true);//21

        if (i == 11) if (i == 12) Debug.Assert(false);
            else Debug.Assert(false); else if (i == 13) Debug.Assert(true);//24
    }
}";

            var fixedCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);//8


        if (i == 1)
            Debug.Assert(true);
        else if (i == 2)
            Debug.Assert(false);//10


        if (i == 3)
            Debug.Assert(true);
        else if (i == 4)
            Debug.Assert(false);//14


        if (i == 5)
            Debug.Assert(true);
        else if (i == 6)
            Debug.Assert(false);
        else
            Debug.Assert(false);//16


        if (i == 7)
            if (i == 8)
                Debug.Assert(false);
            else
                Debug.Assert(false);
        else
            Debug.Assert(true);//18


        if (i == 9)
            if (i == 10)
                Debug.Assert(false);
            else
                Debug.Assert(false);
            else
            Debug.Assert(true);//21


        if (i == 11) if (i == 12) Debug.Assert(false);
            else
    Debug.Assert(false);
else if (i == 13)
    Debug.Assert(true);//24

    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 14),
                this.CSharpDiagnostic().WithLocation(10, 21),
                this.CSharpDiagnostic().WithLocation(14, 26),
                this.CSharpDiagnostic().WithLocation(16, 21),
                this.CSharpDiagnostic().WithLocation(18, 21),
                this.CSharpDiagnostic().WithLocation(21, 26),
                this.CSharpDiagnostic().WithLocation(24, 18),
            };

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
        public async Task TestNoSA1503StatementWithCurlyBracketsAsync(string statementText)
        {
            this.suppressSA1503 = true;

            await this.VerifyCSharpDiagnosticAsync(this.GenerateFixedTestStatement(statementText), EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an if / else statement followed by a block without curly braces will produce a warning.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503IfElseStatementWithoutCurlyBracketsAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0) Debug.Assert(true); else Debug.Assert(false);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 21);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that nested if statements followed by a block without curly braces will produce warnings.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503MultipleIfStatementsWithoutCurlyBracketsAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 21);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderForIfElseStatementAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0) Debug.Assert(true); else Debug.Assert(false);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            Debug.Assert(true);
        else
            Debug.Assert(false);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will properly handle alternate indentations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithAlternateIndentationAsync()
        {
            this.IndentationSize = 1;
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
 public void Bar(int i)
 {
  if (i == 0) Debug.Assert(true);
 }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class TypeName
{
 public void Bar(int i)
 {
  if (i == 0)
   Debug.Assert(true);
 }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle non-whitespace trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithNonWhitespaceTriviaAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
            Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
#pragma warning restore
        if (i == 0)
            Debug.Assert(true);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle multiple cases of missing brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithMultipleNestingsAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0) if (i == 0) Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            if (i == 0)
                Debug.Assert(true);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle the second pass of multiple cases of missing
        /// brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithMultipleNestingsSecondPassAsync()
        {
            this.suppressSA1503 = true;

            var testCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            if (i == 0) Debug.Assert(true);
    }
}";

            var fixedTestCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            if (i == 0)
                Debug.Assert(true);
    }
}";

            var batchFixedTestCode = @"using System.Diagnostics;
public class TypeName
{
    public void Bar(int i)
    {
        if (i == 0)
            if (i == 0)
                Debug.Assert(true);
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode, batchFixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1501StatementMustNotBeOnASingleLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1501CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            if (this.suppressSA1503)
            {
                yield return SA1503CurlyBracketsMustNotBeOmitted.DiagnosticId;
            }
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for a statement.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        private async Task TestNoSA1503CodeFixForStatementAsync(string statementText)
        {
            this.suppressSA1503 = true;
            await this.VerifyCSharpFixAsync(this.GenerateTestStatement(statementText), this.GenerateFixedTestStatement(statementText)).ConfigureAwait(false);
        }

        private string GenerateTestStatement(string statementText)
        {
            var testCodeFormat = @"using System.Diagnostics;
public class TypeName : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT# Debug.Assert(true);
    }

    public void Dispose() {}
}";
            return testCodeFormat.Replace("#STATEMENT#", statementText);
        }

        private string GenerateFixedTestStatement(string statementText)
        {
            var fixedTestCodeFormat = @"using System.Diagnostics;
public class TypeName : System.IDisposable
{
    public unsafe void Bar(int i)
    {
        #STATEMENT#
            Debug.Assert(true);
    }

    public void Dispose() {}
}";
            return fixedTestCodeFormat.Replace("#STATEMENT#", statementText);
        }
    }
}
