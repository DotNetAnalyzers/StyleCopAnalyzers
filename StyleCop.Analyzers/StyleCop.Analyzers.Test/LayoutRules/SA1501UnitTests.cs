// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1501StatementMustNotBeOnASingleLine,
        StyleCop.Analyzers.LayoutRules.SA1501CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    public class SA1501UnitTests
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
            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(6, 21), CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a lock statement with preprocessor trivia will trigger the correct warnings.
        /// </summary>
        /// <remarks>
        /// <para>The analyzer will only trigger on the second block, as the first block will be marked as
        /// DisabledTextTrivia due to the preprocessor statements.</para>
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

            await VerifyCSharpDiagnosticAsync(testCode, Diagnostic().WithLocation(10, 9), CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that lock statement with an invalid formatted block statement spread over multiple lines will not
        /// trigger a warning.
        /// </summary>
        /// <remarks><para>This will trigger SA1500.</para></remarks>
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(7, 11);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(6, 21);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement, when it starts on the same line as the parent.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockInSourceFileWithTabsAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 15),
                },
                FixedCode = fixedTestCode,
                UseTabs = true,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            var expected = Diagnostic().WithLocation(6, 35);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle preprocessor trivia.
        /// </summary>
        /// <remarks>
        /// <para>Only the second block will be fixed, as the first block is marked as DisabledTextTrivia.</para>
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

            var expected = Diagnostic().WithLocation(10, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(8, 13),
                Diagnostic().WithLocation(11, 13),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1172, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1172")]
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

            DiagnosticResult expectedDiagnostics = Diagnostic().WithLocation(7, 12);
            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(1172, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1172")]
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 12),
                    DiagnosticResult.CompilerError("CS0103").WithLocation(7, 14).WithMessage("The name 'Bar' does not exist in the current context"),
                    DiagnosticResult.CompilerError("CS1513").WithLocation(7, 37).WithMessage("} expected"),
                    DiagnosticResult.CompilerError("CS1513").WithLocation(7, 37).WithMessage("} expected"),
                },
                FixedCode = fixedTestCode,
                RemainingDiagnostics =
                {
                    DiagnosticResult.CompilerError("CS0103").WithLocation(9, 13).WithMessage("The name 'Bar' does not exist in the current context"),
                    DiagnosticResult.CompilerError("CS1513").WithLocation(11, 23).WithMessage("} expected"),
                    DiagnosticResult.CompilerError("CS1513").WithLocation(11, 23).WithMessage("} expected"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a statement followed by a block without braces will produce a warning.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestNoSA1503StatementWithoutBracesAsync(string statementText)
        {
            await new CSharpTest
            {
                TestCode = this.GenerateTestStatement(statementText),
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, statementText.Length + 10),
                },
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a <c>do</c> statement followed by a block without braces will produce a warning, and the
        /// code fix for this warning results in valid code.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503DoStatementAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 12),
                },
                FixedCode = fixedCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that consecutive single-line statements are properly reported and fixed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503ConsecutiveStatementsAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 24),
                    Diagnostic().WithLocation(8, 24),
                },
                FixedCode = fixedCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the behavior of SA1503 when SA1503 is suppressed for many different forms of <c>if</c> statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503IfStatementsAsync()
        {
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

            var incrementalFixedCode = @"using System.Diagnostics;
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


        if (i == 11) if (i == 12)
            Debug.Assert(false);
            else
            Debug.Assert(false);
        else if (i == 13)
            Debug.Assert(true);//24

    }
}";

            var batchFixedCode = @"using System.Diagnostics;
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


        if (i == 11)
            if (i == 12)
                Debug.Assert(false);
            else
                Debug.Assert(false);
        else if (i == 13)
            Debug.Assert(true);//24

    }
}";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(8, 14),
                    Diagnostic().WithLocation(10, 21),
                    Diagnostic().WithLocation(10, 58),
                    Diagnostic().WithLocation(14, 26),
                    Diagnostic().WithLocation(16, 21),
                    Diagnostic().WithLocation(16, 58),
                    Diagnostic().WithLocation(16, 84),
                    Diagnostic().WithLocation(18, 21),
                    Diagnostic().WithLocation(18, 33),
                    Diagnostic().WithLocation(18, 59),
                    Diagnostic().WithLocation(18, 85),
                    Diagnostic().WithLocation(21, 26),
                    Diagnostic().WithLocation(21, 52),
                    Diagnostic().WithLocation(21, 78),
                    Diagnostic().WithLocation(23, 22).WithSeverity(DiagnosticSeverity.Hidden),
                    Diagnostic().WithLocation(23, 35),
                    Diagnostic().WithLocation(24, 18),
                    Diagnostic().WithLocation(24, 57),
                },
                FixedCode = incrementalFixedCode,
                RemainingDiagnostics =
                {
                    Diagnostic().WithLocation(50, 22).WithSeverity(DiagnosticSeverity.Hidden),
                },
                BatchFixedCode = batchFixedCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a statement followed by a block with braces will produce no diagnostics results.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestNoSA1503StatementWithBracesAsync(string statementText)
        {
            await new CSharpTest
            {
                TestCode = this.GenerateFixedTestStatement(statementText),
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for an if .. else statement.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderForIfElseStatementAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 21),
                    Diagnostic().WithLocation(6, 46),
                },
                FixedCode = fixedTestCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will properly handle alternate indentations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithAlternateIndentationAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 15),
                },
                FixedCode = fixedTestCode,
                IndentationSize = 1,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle non-whitespace trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithNonWhitespaceTriviaAsync()
        {
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

            // The code fix will not make any changes.
            var fixedTestCode = testCode;

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                },
                FixedCode = fixedTestCode,
                NumberOfFixAllIterations = 0,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle multiple cases of missing braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithMultipleNestingsAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 21),
                    Diagnostic().WithLocation(6, 33),
                },
                FixedCode = fixedTestCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly handle the second pass of multiple cases of missing
        /// braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoSA1503CodeFixProviderWithMultipleNestingsSecondPassAsync()
        {
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

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(7, 25),
                },
                FixedCode = fixedTestCode,
                BatchFixedCode = batchFixedTestCode,
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will work properly for a statement.
        /// </summary>
        /// <param name="statementText">The source code for the first part of a compound statement whose child can be
        /// either a statement block or a single statement.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [MemberData(nameof(TestStatements))]
        public async Task TestNoSA1503CodeFixForStatementAsync(string statementText)
        {
            await new CSharpTest
            {
                TestCode = this.GenerateTestStatement(statementText),
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(6, 10 + statementText.Length),
                },
                FixedCode = this.GenerateFixedTestStatement(statementText),
                DisabledDiagnostics =
                {
                    SA1503BracesMustNotBeOmitted.DiagnosticId,
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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
