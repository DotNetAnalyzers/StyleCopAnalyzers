// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1504AllAccessorsMustBeSingleLineOrMultiLine,
        StyleCop.Analyzers.LayoutRules.SA1504CodeFixProvider>;

    public class SA1504UnitTests
    {
        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        public async Task TestPropertyGetInOneLineSetInMultipleLinesAsync(string propertyDeclaration)
        {
            var testCode = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get {{ return 1; }}
        set
        {{
        }}
    }}
}}";

            var fixedTestCodeSingle = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get {{ return 1; }}
        set {{ }}
    }}
}}";

            var fixedTestCodeMultiple = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get
        {{
            return 1;
        }}

        set
        {{
        }}
    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeSingle,
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        public async Task TestPropertyGetInOneLineSetInMultipleLinesWithMultipleStatementsAsync(string propertyDeclaration)
        {
            var testCode = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get {{ return backingField; }}
        set
        {{
            this.backingField = value;
            this.OnPropertyChanged();
        }}
    }}

    private int backingField;

    private void OnPropertyChanged()
    {{

    }}
}}";
            var fixedTestCodeMultiple = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get
        {{
            return backingField;
        }}

        set
        {{
            this.backingField = value;
            this.OnPropertyChanged();
        }}
    }}

    private int backingField;

    private void OnPropertyChanged()
    {{

    }}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCodeMultiple, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        public async Task TestPropertyGetAndSetOnOneLineWithCommentsAsync(string propertyDeclaration)
        {
            var testCode = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        // here's get
        get {{ return 1; }}
        set {{ }}
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        public async Task TestPropertyOnlyGetterAsync(string propertyDeclaration)
        {
            var testCode = $@"
public class Foo
{{
    public {propertyDeclaration}
    {{
        get 
        {{
            return 1; 
        }}
    }}
}}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAutoPropertyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get;set;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyExpressionBodyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop => 1;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddIsOnMultipleLinesAndRemoveIsOnOneLineAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        add
        {
            this.nameChanged += value;
        }
        remove { this.nameChanged -= value; }
    }
}";

            var fixedTestCodeSingle = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            var fixedTestCodeMultiple = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        add
        {
            this.nameChanged += value;
        }

        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeSingle,
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventAddAndRemoveAreOnOneLineWithCommentAsync()
        {
            var testCode = @"
public class Foo
{
    private System.EventHandler nameChanged;

    public event System.EventHandler NameChanged
    {
        // this is add
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a syntactically correct invalid accessor declaration will not report SA1504.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestAccessorWithoutBodyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        get;

        set
        {
        }
    }
}";

            var expected = this.GetExpectedResultAccessorWithoutBody();
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the code fix will properly handle kinds of inline comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyWithMultipleInlineCommentsAsync()
        {
            var testCode = @"
public class Foo
{
    /// <summary>
    /// Gets the test property.
    /// </summary>
    public int Prop
    {
        /* c1 */ get /* c2 */ { /* c3 */ return 1; /* c4 */ } /* c5 */
        /* c6 */
        protected
        /* c7 */ internal
        /* c8 */ set /* c9 */
        {
            /* c10 */
        } /* c11 */
    }
}";

            var fixedTestCodeSingle = @"
public class Foo
{
    /// <summary>
    /// Gets the test property.
    /// </summary>
    public int Prop
    {
        /* c1 */ get /* c2 */ { /* c3 */ return 1; /* c4 */ } /* c5 */
        /* c6 */ protected /* c7 */ internal /* c8 */ set /* c9 */ { /* c10 */ } /* c11 */
    }
}";

            var fixedTestCodeMultiple = @"
public class Foo
{
    /// <summary>
    /// Gets the test property.
    /// </summary>
    public int Prop
    {
        /* c1 */ get /* c2 */
        { /* c3 */
            return 1; /* c4 */
        } /* c5 */

        /* c6 */ protected /* c7 */ internal /* c8 */ set /* c9 */
        { /* c10 */
        } /* c11 */
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 18);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeSingle,
                CodeActionIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeActionIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an attribute on its own line does not make a single-line accessor count as multi-line.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3434, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3434")]
        public async Task TestAttributeOnSingleLineAccessorDoesNotReportAsync()
        {
            var testCode = @"
public class ClassName
{
    public string InitialTargets
    {
        [System.Diagnostics.DebuggerStepThrough]
        get { return this.initialTargets; }

        set { this.initialTargets = value; }
    }

    private string initialTargets;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an attribute on a single-line accessor does not hide a multi-line accessor.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(3434, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3434")]
        public async Task TestAttributeDoesNotHideMultiLineAccessorAsync()
        {
            var testCode = @"
public class ClassName
{
    public string InitialTargets
    {
        [System.Diagnostics.DebuggerStepThrough]
        {|#0:get|} { return this.initialTargets; }

        set
        {
            this.initialTargets = value;
        }
    }

    private string initialTargets;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(0);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that expanding a block-bodied getter preserves setter trivia and supports Fix All.
        /// </summary>
        /// <param name="propertyDeclaration">The property or indexer declaration.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        [WorkItem(4310, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4310")]
        public async Task TestMultipleLineFixExpandsGetterAndPreservesSetterTriviaAsync(string propertyDeclaration)
        {
            var testCode = $$"""

public class Foo
{
    public {{propertyDeclaration}}
    {
        {|#0:get|} { /* before */ return 1; /* after */ } // getter

        // Setter
        set // setter
        { // opening brace

            // Before the first statement
#if true
            System.Console.WriteLine(1); // trailing comment

            /* Block
               comment */
            System.Console.WriteLine(2);
#endif

            // Before the last statement
            System.Console.WriteLine(value);

            // Before the closing brace
        } // setter end
    }
}
""";
            var fixedCode = $$"""

public class Foo
{
    public {{propertyDeclaration}}
    {
        get
        { /* before */
            return 1; /* after */
        } // getter

        // Setter
        set // setter
        { // opening brace

            // Before the first statement
#if true
            System.Console.WriteLine(1); // trailing comment

            /* Block
               comment */
            System.Console.WriteLine(2);
#endif

            // Before the last statement
            System.Console.WriteLine(value);

            // Before the closing brace
        } // setter end
    }
}
""";
            testCode += testCode.Replace("class Foo", "class Bar").Replace("#0", "#1");
            fixedCode += fixedCode.Replace("class Foo", "class Bar");
            await VerifyCSharpFixAsync(
                testCode,
                new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that expanding a block-bodied setter preserves getter trivia and supports Fix All.
        /// </summary>
        /// <param name="propertyDeclaration">The property or indexer declaration.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("int Prop")]
        [InlineData("int this[int index]")]
        [WorkItem(4310, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4310")]
        public async Task TestMultipleLineFixExpandsSetterAndPreservesGetterTriviaAsync(string propertyDeclaration)
        {
            var testCode = $$"""

public class Foo
{
    public {{propertyDeclaration}}
    {
        {|#0:get|} // getter
        { // opening brace

            // Before the first statement
#if true
            System.Console.WriteLine(1); // trailing comment

            /* Block
               comment */
            System.Console.WriteLine(2);
#endif

            // Before the last statement
            return 1;

            // Before the closing brace
        } // getter end

        // Setter
        set { /* before */ System.Console.WriteLine(value); /* after */ } // setter
    }
}
""";
            var fixedCode = $$"""

public class Foo
{
    public {{propertyDeclaration}}
    {
        get // getter
        { // opening brace

            // Before the first statement
#if true
            System.Console.WriteLine(1); // trailing comment

            /* Block
               comment */
            System.Console.WriteLine(2);
#endif

            // Before the last statement
            return 1;

            // Before the closing brace
        } // getter end

        // Setter
        set
        { /* before */
            System.Console.WriteLine(value); /* after */
        } // setter
    }
}
""";
            testCode += testCode.Replace("class Foo", "class Bar").Replace("#0", "#1");
            fixedCode += fixedCode.Replace("class Foo", "class Bar");
            await VerifyCSharpFixAsync(
                testCode,
                new[] { Diagnostic().WithLocation(0), Diagnostic().WithLocation(1) },
                fixedCode,
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comments and blank lines in event accessors are preserved.
        /// </summary>
        /// <param name="expandAdd">Whether the add accessor starts on one line.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [WorkItem(4310, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4310")]
        public async Task TestMultipleLineFixPreservesEventTriviaAsync(bool expandAdd)
        {
            var body = @"
        {
            // Before
            System.Console.WriteLine(value);

            // After
        }";
            var add = expandAdd ? "        {|#0:add|} { }" : "        {|#0:add|}" + body;
            var remove = expandAdd ? "        remove" + body : "        remove { }";
            var fixedAdd = expandAdd ? "        add\r\n        {\r\n        }" : add.Replace("{|#0:add|}", "add");
            var fixedRemove = expandAdd ? remove : "        remove\r\n        {\r\n        }";
            var testCode = $@"
public class Foo
{{
    public event System.EventHandler Changed
    {{
{add}
{remove}
    }}
}}";
            var fixedCode = $@"
public class Foo
{{
    public event System.EventHandler Changed
    {{
{fixedAdd}

{fixedRemove}
    }}
}}";
            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that comment-only bodies and blank lines inside empty bodies survive expansion.
        /// </summary>
        /// <param name="contents">The existing setter body contents.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\r\n            // Keep\r\n\r\n            /* Keep too */\r\n")]
        [InlineData("\r\n\r\n")]
        [InlineData("\r\n            /* Keep */\r\n\r\n")]
        [InlineData("\r\n            /* Keep\r\n               both lines */\r\n")]
        [InlineData("\r\n#if true\r\n            // Keep\r\n#endif\r\n")]
        [InlineData("\r\n#if false\r\n            disabled text\r\n#endif\r\n")]
        [WorkItem(4310, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4310")]
        public async Task TestMultipleLineFixPreservesEmptyBodyTriviaAsync(string contents)
        {
            var testCode = @"
public class Foo
{
    public int Prop
    {
        {|#0:get|} { return 1; }
        set
        {" + contents + @"        }
    }
}";
            var fixedCode = @"
public class Foo
{
    public int Prop
    {
        get
        {
            return 1;
        }

        set
        {" + contents + @"        }
    }
}";
            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                FixedCode = fixedCode,
                CodeActionEquivalenceKey = "SA1504CodeFixProviderMultipleLines",
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultAccessorWithoutBody()
        {
            return new DiagnosticResult[]
            {
                DiagnosticResult.CompilerError("CS0501").WithMessage("'Foo.Prop.get' must declare a body because it is not marked abstract, extern, or partial").WithLocation(6, 9),
            };
        }
    }
}
