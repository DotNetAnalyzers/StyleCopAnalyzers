// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    public class SA1504UnitTests : CodeFixVerifier
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeSingle, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeMultiple, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeSingle, codeFixIndex: 0).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeMultiple, codeFixIndex: 1).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeMultiple, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeMultiple).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPropertyExpressionBodyAsync()
        {
            var testCode = @"
public class Foo
{
    public int Prop => 1;
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 9);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeSingle, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeMultiple, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeSingle, codeFixIndex: 0).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeMultiple, codeFixIndex: 1).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                new DiagnosticResult()
                {
                    Id = "CS0501",
                    Message = "'Foo.Prop.get' must declare a body because it is not marked abstract, extern, or partial",
                    Severity = DiagnosticSeverity.Error,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 9) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeSingle, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeMultiple, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeSingle, codeFixIndex: 0).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCodeMultiple, codeFixIndex: 1).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1504AllAccessorsMustBeSingleLineOrMultiLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1504CodeFixProvider();
        }
    }
}
