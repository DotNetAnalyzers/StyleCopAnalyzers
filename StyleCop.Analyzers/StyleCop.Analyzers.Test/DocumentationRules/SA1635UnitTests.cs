// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.DocumentationRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1635 diagnostic.
    /// </summary>
    public class SA1635UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that a file header with a copyright element in short hand notation will produce the expected diagnostic message.
        /// </summary>
        /// <param name="lineEnding">The line ending to use in the test code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestFileHeaderWithShorthandCopyrightAsync(string lineEnding)
        {
            var testCode = @"// {|#0:<copyright|} file=""Test0.cs"" company=""FooCorp""/>

namespace Bar
{
}
".ReplaceLineEndings(lineEnding);
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
".ReplaceLineEndings(lineEnding);

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
            string fixedCode =
                "// <copyright file=\"Test0.cs\" company=\"FooCorp\">\r\n" +
                "// Copyright (c) FooCorp. All rights reserved.\r\n" +
                "// </copyright>\r\n" +
                "\r\n" +
                "namespace Bar\r\n" +
                "{\r\n" +
                "}\r\n";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
