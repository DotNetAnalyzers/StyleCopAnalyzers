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
    /// This class contains unit tests for <see cref="SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind"/>.
    /// </summary>
    /// <remarks>
    /// <para>This set of tests includes tests with the same inputs as <see cref="SA1653UnitTests"/> and
    /// <see cref="SA1654UseChildBlocksConsistently"/> to ensure
    /// <see cref="SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind"/> is not also reported in those
    /// cases.</para>
    /// </remarks>
    public class SA1655UnitTests : CodeFixVerifier
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

            // reported as SA1654
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            // reported as SA1654
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            // reported as SA1654
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            // reported as SA1653 and SA1654
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

        [Fact]
        public async Task TestExceptionElementsAsync()
        {
            var testCode = @"
using System;
public class ClassName
{
    /// <exception cref=""ArgumentNullException"">If <paramref name=""x""/> is <see langword=""null""/>.</exception>
    /// <exception cref=""ArgumentException"">
    /// If <paramref name=""x""/> is empty.
    /// <para>-or-</para>
    /// <para>If <paramref name=""y""/> is empty.</para>
    /// </exception>
    public void MethodName(string x, string y)
    {
    }
}";

            var fixedCode = @"
using System;
public class ClassName
{
    /// <exception cref=""ArgumentNullException""><para>If <paramref name=""x""/> is <see langword=""null""/>.</para></exception>
    /// <exception cref=""ArgumentException"">
    /// If <paramref name=""x""/> is empty.
    /// <para>-or-</para>
    /// <para>If <paramref name=""y""/> is empty.</para>
    /// </exception>
    public void MethodName(string x, string y)
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(5, 49);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestListItemsAsync()
        {
            var testCode = @"
/// <remarks>
/// <list type=""bullet"">
/// <item>Item 1.</item>
/// <item><para>Item 2.</para></item>
/// </list>
/// </remarks>
public class ClassName
{
}";

            var fixedCode = @"
/// <remarks>
/// <list type=""bullet"">
/// <item><para>Item 1.</para></item>
/// <item><para>Item 2.</para></item>
/// </list>
/// </remarks>
public class ClassName
{
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 11);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1655UseChildBlocksConsistentlyAcrossElementsOfTheSameKind();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new BlockLevelDocumentationCodeFixProvider();
        }
    }
}
