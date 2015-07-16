namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1004DocumentationLinesMustBeginWithSingleSpace"/>.
    /// </summary>
    public class SA1004UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixedExampleAsync()
        {
            string testCode = @"
public class TypeName
{
    /// <summary>
    /// The summary text.
    /// </summary>
    /// <param name=""x"">The document root.</param>
    /// <param name=""y"">The XML header token.</param>
    private void Method1(int x, int y)
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestRuleExampleAsync()
        {
            string testCode = @"
public class TypeName
{
    ///<summary>
    ///The summary text.
    ///</summary>
    ///   <param name=""x"">The document root.</param>
    ///    <param name=""y"">The XML header token.</param>
    private void Method1(int x, int y)
    {
    }
}
";

            // Currently the extra indentation for <param> elements is not checked.
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 8),
                this.CSharpDiagnostic().WithLocation(5, 8),
                this.CSharpDiagnostic().WithLocation(6, 8),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task TestEmptyCommentLinesAsync()
        {
            string testCode = @"
public class TypeName
{
    /// <summary>
    ///
    /// </summary>
    private void Method1()
    {
    }

    /// <summary>
    /// </summary>
    ///
    private void Method2()
    {
    }

    ///
    /// <summary>
    /// </summary>
    private void Method3()
    {
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1004DocumentationLinesMustBeginWithSingleSpace();
        }
    }
}
