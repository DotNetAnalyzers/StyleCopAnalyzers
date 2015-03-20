namespace StyleCop.Analyzers.Test.LayoutRules
{
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
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that lock statement with single line block statement will trigger a warning.
        /// </summary>
        [Fact]
        public async Task TestLockWithSingleLineBlock()
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
        [Fact]
        public async Task TestLockWithPreProcessorTrivia()
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
        [Fact]
        public async Task TestLockWithMultilineBlock()
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
        [Fact]
        public async Task TestLockWithInvalidMultilineBlock()
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
        [Fact]
        public async Task TestSingleLineAnonymousMethodIsAllowed()
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
        [Fact]
        public async Task TestSingleLineLambdaExpressionIsAllowed()
        {
            string testCode = @"using System.Diagnostics;
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
        [Fact]
        public async Task TestSingleLineMethodIsAllowed()
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
        [Fact]
        public async Task TestSingleLinePropertyAccessorsAreAllowed()
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
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlock()
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
        [Fact]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockWithParentOnSameLine()
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
        [Fact(Skip = "Disabled until pull request #522 is merged.")]
        public async Task TestCodeFixProviderCorrectlyExpandsBlockInSourceFileWithTabs()
        {
            string testCode = 
                "using System.Diagnostics;\r\n" + 
                "public class Foo\r\n" + 
                "{\r\n" + 
                "\tpublic void Bar(int i)" + 
                "\t{\r\n" +
                "\t\tlock (this) { Debug.Assert(true); }\r\n" + 
                "\t}" +
                "}";

            string fixedTestCode =
                "using System.Diagnostics;\r\n" +
                "public class Foo\r\n" +
                "{\r\n" +
                "\tpublic void Bar(int i)" +
                "\t{\r\n" +
                "\t\tlock (this)"+ 
                "\t\t{\r\n" + 
                "\t\t\tDebug.Assert(true);\r\n" + 
                "\t\t}\r\n" +
                "\t}" +
                "}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle non-whitespace trivia.
        /// </summary>
        [Fact]
        public async Task TestCodeFixProviderCorrectlyHandlesTrivia()
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
        [Fact]
        public async Task TestCodeFixProviderCorrectlyHandlesPreProcessorTrivia()
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

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1501StatementMustNotBeOnASingleLine();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1501CodeFixProvider();
        }
    }
}
