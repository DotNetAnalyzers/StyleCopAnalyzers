namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1634FileHeaderMustShowCopyright"/> analyzer.
    /// </summary>
    public class SA1634UnitTests : FileHeaderTestBase
    {
        /// <inheritdoc/>
        protected override DiagnosticResult[] MissingCopyrightTagDiagnostics
        {
            get
            {
                return new[]
                {
                    this.CSharpDiagnostic().WithLocation(1, 1)
                };
            }
        }

        /// <summary>
        /// Verifies that a file header with a copyright element will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithCopyrightLastAsync()
        {
            var testCode = @"// <author>
//   John Doe
// </author>
// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright element in short hand notation will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithShorthandCopyrightAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp""/>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright element in wrong casing will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidFileHeaderWithCopyrightInWrongCaseAsync()
        {
            var testCode = @"// <Copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </Copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1634FileHeaderMustShowCopyright();
        }
    }
}
