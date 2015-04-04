namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1653PlaceTextInParagraphs"/>.
    /// </summary>
    public class SA1653UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestNoDocumentationAsync()
        {
            var testCode = @"
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSummaryDocumentationAsync()
        {
            var testCode = @"
/// <summary>
/// Summary.
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSimpleParagraphBlockAsync()
        {
            var testCode = @"
/// <remarks>
/// <para>Remarks.</para>
/// </remarks>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoSimpleParagraphBlockAsync()
        {
            var testCode = @"
/// <remarks>
/// <para>Remarks.</para>
/// <para>Remarks.</para>
/// </remarks>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleBlockElementsAsync()
        {
            var testCode = @"
/// <remarks>
/// <!-- Supported XML documentation comment block-level elements -->
/// <code>Remarks.</code>
/// <list><item>Item</item></list>
/// <note><para>Remarks.</para></note>
/// <para>Remarks.</para>
/// <!-- Supported SHFB elements which may be block-level elements -->
/// <inheritdoc/>
/// <token>SomeTokenName</token>
/// <include />
/// <!-- Supported HTML block-level elements -->
/// <div>Remarks.</div>
/// <p>Remarks.</p>
/// </remarks>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSimpleInlineBlockAsync()
        {
            var testCode = @"
/// <remarks>Remarks.</remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks><para>Remarks.</para></remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 14);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiLineInlineBlockAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Remarks.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiLineParagraphInlineBlockAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// Line 2.
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Remarks.
/// Line 2.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFirstParagraphInlineBlockAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// <para>Paragraph 2.</para>
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Remarks.</para>
/// <para>Paragraph 2.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInlineParagraphAndCodeAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// <code>Code.</code>
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Remarks.</para>
/// <code>Code.</code>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeAndInlineParagraphAsync()
        {
            var testCode = @"
/// <remarks>
/// <code>Code.</code>
/// Remarks.
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <code>Code.</code>
/// <para>Remarks.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeInlineParagraphsWithOtherElementsAsync()
        {
            var testCode = @"
/// <remarks>
/// Leading remarks.
/// <code>Code.</code>
/// <para>Remarks.</para>
/// <note>Note.</note>
/// Closing remarks.
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Leading remarks.</para>
/// <code>Code.</code>
/// <para>Remarks.</para>
/// <note><para>Note.</para></note>
/// <para>Closing remarks.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(6, 11),
                this.CSharpDiagnostic().WithLocation(7, 5),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSeeIsAnInlineElementAsync()
        {
            var testCode = @"
/// <remarks>
/// Leading remarks.
/// <see cref=""ClassName""/>
/// <para>Remarks.</para>
/// <note>Note.</note>
/// Closing remarks.
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <para>Leading remarks.
/// <see cref=""ClassName""/></para>
/// <para>Remarks.</para>
/// <note><para>Note.</para></note>
/// <para>Closing remarks.</para>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(6, 11),
                this.CSharpDiagnostic().WithLocation(7, 5),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1653PlaceTextInParagraphs();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1653CodeFixProvider();
        }
    }
}
