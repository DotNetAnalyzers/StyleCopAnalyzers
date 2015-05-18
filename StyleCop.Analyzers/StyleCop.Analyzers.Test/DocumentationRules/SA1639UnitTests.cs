namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1639FileHeaderMustHaveSummary"/> analyzer.
    /// </summary>
    public class SA1639UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that a file header without a summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithoutSummaryTagAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an empty summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithEmptySummaryTagAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary/>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(4, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a whitespace only summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithWhitespaceOnlySummaryTagAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary>   </summary>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(4, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that this analyzer is disabled by default.
        /// </summary>
        [Fact]
        public void TestDisabledByDefault()
        {
            var analyzer = new SA1639FileHeaderMustHaveSummary();

            Assert.Equal(analyzer.SupportedDiagnostics.Length, 1);
            Assert.False(analyzer.SupportedDiagnostics[0].IsEnabledByDefault);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1639FileHeaderMustHaveSummary();
        }
    }
}
