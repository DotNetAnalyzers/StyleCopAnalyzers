// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using TestHelper;
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
                CodeFixIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeFixIndex = 1,
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyExpressionBodyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop => 1;
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                CodeFixIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeFixIndex = 1,
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult[] expected =
            {
                CompilerError("CS0501").WithMessage("'Foo.Prop.get' must declare a body because it is not marked abstract, extern, or partial").WithLocation(6, 9),
            };

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
                CodeFixIndex = 0,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics = { expected },
                FixedCode = fixedTestCodeMultiple,
                CodeFixIndex = 1,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
