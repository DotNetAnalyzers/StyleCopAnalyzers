namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1635 diagnostic.
    /// </summary>
    public class SA1635UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that a file header with a copyright element in short hand notation will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithShorthandCopyrightAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp""/>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright element that contains only whitespace will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithWhitespaceOnlyCopyrightAsync()
        {
            var testCode =
                "// <copyright file=\"Test0.cs\" company=\"FooCorp\">\r\n" +
                "//     \r\n" +
                "// </copyright>\r\n" +
                "\r\n" +
                "namespace Bar\r\n" +
                "{\r\n" +
                "}\r\n";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
