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
    /// This class contains unit tests for <see cref="SA1654UseChildBlocksConsistently"/>.
    /// </summary>
    public class SA1654UnitTests : CodeFixVerifier
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
/// <summary>
/// <para>Summary.</para>
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTwoSimpleParagraphBlockAsync()
        {
            var testCode = @"
/// <summary>
/// <para>Summary.</para>
/// <para>Summary.</para>
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultipleBlockElementsAsync()
        {
            var testCode = @"
/// <summary>
/// <!-- Supported XML documentation comment block-level elements -->
/// <code>Summary.</code>
/// <list><item>Item</item></list>
/// <note><para>Summary.</para></note>
/// <para>Summary.</para>
/// <!-- Supported SHFB elements which may be block-level elements -->
/// <inheritdoc/>
/// <token>SomeTokenName</token>
/// <include />
/// <!-- Supported HTML block-level elements -->
/// <div>Summary.</div>
/// <p>Summary.</p>
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSimpleInlineBlockAsync()
        {
            var testCode = @"
/// <summary>Summary.</summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultiLineParagraphInlineBlockAsync()
        {
            var testCode = @"
/// <summary>
/// Remarks.
/// Line 2.
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFirstParagraphInlineBlockAsync()
        {
            var testCode = @"
/// <summary>
/// Summary.
/// <para>Paragraph 2.</para>
/// </summary>
public class ClassName
{
}";

            var fixedCode = @"
/// <summary>
/// <para>Summary.</para>
/// <para>Paragraph 2.</para>
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFirstParagraphInlineBlockInRemarksAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// <para>Paragraph 2.</para>
/// </remarks>
public class ClassName
{
}";

            // reported as SA1653
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInlineParagraphAndCodeAsync()
        {
            var testCode = @"
/// <summary>
/// Summary.
/// <code>Code.</code>
/// </summary>
public class ClassName
{
}";

            var fixedCode = @"
/// <summary>
/// <para>Summary.</para>
/// <code>Code.</code>
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInlineParagraphAndCodeInRemarksAsync()
        {
            var testCode = @"
/// <remarks>
/// Remarks.
/// <code>Code.</code>
/// </remarks>
public class ClassName
{
}";

            // reported as SA1653
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeAndInlineParagraphAsync()
        {
            var testCode = @"
/// <summary>
/// <code>Code.</code>
/// Summary.
/// </summary>
public class ClassName
{
}";

            var fixedCode = @"
/// <summary>
/// <code>Code.</code>
/// <para>Summary.</para>
/// </summary>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeAndInlineParagraphInRemarksAsync()
        {
            var testCode = @"
/// <remarks>
/// <code>Code.</code>
/// Remarks.
/// </remarks>
public class ClassName
{
}";

            // reported as SA1653
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeInlineParagraphsWithOtherElementsAsync()
        {
            var testCode = @"
/// <summary>
/// Leading summary.
/// <code>Code.</code>
/// <para>Summary.</para>
/// <note>Note.</note>
/// Closing summary.
/// </summary>
public class ClassName
{
}";

            var fixedCode = @"
/// <summary>
/// <para>Leading summary.</para>
/// <code>Code.</code>
/// <para>Summary.</para>
/// <note>Note.</note>
/// <para>Closing summary.</para>
/// </summary>
public class ClassName
{
}";

            // the <note> element is also covered by SA1653, even when it appears inside the <summary> element.
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(7, 5),
            };
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThreeInlineParagraphsWithOtherElementsInRemarksAsync()
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

            // reported as SA1653
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSeeIsAnInlineElementAsync()
        {
            var testCode = @"
/// <summary>
/// Leading summary.
/// <see cref=""ClassName""/>.
/// </summary>
public class ClassName
{
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1654UseChildBlocksConsistently();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new BlockLevelDocumentationCodeFixProvider();
        }
    }
}
