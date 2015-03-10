namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using StyleCop.Analyzers.LayoutRules;

    using TestHelper;
    using System.Linq;


    /// <summary>
    /// Unit tests for <see cref="SA1501StatementMustNotBeOnASingleLine"/>.
    /// </summary>
    [TestClass]
    public class SA1501UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1501StatementMustNotBeOnASingleLine.DiagnosticId;

        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        [TestMethod]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that lock statement with single line block statement will trigger a warning.
        /// </summary>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(6, 21), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a lock statement with preprocessor trivia will trigger the correct warnings.
        /// </summary>
        /// <remarks>
        /// The analyzer will only trigger on the second block, as the first block will be marked as DisabledTextTrivia due to the preprocessor statements.
        /// </remarks>
        [TestMethod]
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

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GenerateExpectedWarning(10, 9), CancellationToken.None);
        }

        /// <summary>
        /// Verifies that lock statement with a block statement spread over multiple lines will not trigger a warning.
        /// </summary>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that lock statement with an invalid formatted block statement spread over multiple lines will not trigger a warning.
        /// </summary>
        /// <remarks>This will trigger SA1500.</remarks>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single line anonymous method definition is allowed.
        /// </summary>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single line lambda expression definition is allowed.
        /// </summary>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single line method definition is not flagged by this analyzer.
        /// </summary>
        [TestMethod]
        public async Task TestSingleLineMethodIsAllowed()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar() { Debug.Assert(true); }
}";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that a single line property accessors are not flagged by this analyzer.
        /// </summary>
        [TestMethod]
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement.
        /// </summary>
        [TestMethod]
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

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement, when it starts on the same line as the parent.
        /// </summary>
        [TestMethod]
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

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly expand the block to a multiline statement, when it starts on the same line as the parent.
        /// </summary>
        [TestMethod]
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

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle non-whitespace trivia.
        /// </summary>
        [TestMethod]
        public async Task TestCodeFixProviderCorrectlyHandlesTrivia()
        {
            string testCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) /* comment */ { /* comment2 */ Debug.Assert(true); }
    }
}";
            string fixedTestCode = @"using System.Diagnostics;
public class Foo
{
    public void Bar(int i)
    {
        lock (this) /* comment */
        { 
            /* comment2 */
            Debug.Assert(true); 
        }
    }
}";

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
        }

        /// <summary>
        /// Verifies that the code fix provider will correctly handle preprocessor trivia.
        /// </summary>
        /// <remarks>
        /// Only the second block will be fixed, as the first block is marked as DisabledTextTrivia.
        /// </remarks>
        [TestMethod]
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

            await this.VerifyCSharpFixAsync(testCode, fixedTestCode);
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

        private DiagnosticResult[] GenerateExpectedWarning(int line, int column)
        {
            return new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Statement must not be on a single line",
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
                    }
                };
        }
    }
}
