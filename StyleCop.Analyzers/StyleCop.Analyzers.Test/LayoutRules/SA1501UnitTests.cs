﻿namespace StyleCop.Analyzers.Test.LayoutRules
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
    /// Unit tests for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    public class SA1501UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1501StatementMustNotBeOnASingleLine.DiagnosticId;

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

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(7, 12)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1501StatementMustNotBeOnASingleLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1501CodeFixProvider();
        }
    }
}
