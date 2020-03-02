// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1636 diagnostic.
    /// </summary>
    public class SA1636UnitTests : FileHeaderTestBase
    {
        private const string MultiLineHeaderTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""copyright (c) {companyName}. All rights reserved.\n\nLine #3""
    }
  }
}
";

        private const string NoXmlMultiLineHeaderTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""copyright (c) {companyName}. All rights reserved.\n\nLine #3"",
      ""xmlHeader"": false
    }
  }
}
";

        private bool useMultiLineHeaderTestSettings;
        private bool useNoXmlMultiLineHeaderTestSettings;

        /// <summary>
        /// Verifies that a file header with a copyright message that is different than in the settings will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithDifferentCopyrightMessageAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   My custom copyright message.
// </copyright>

namespace Bar
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright message that differs only in case from the settings will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithInvalidCaseCopyrightMessageAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage1Async()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   copyright (c) FooCorp. All rights reserved.
//
//   Line #3
// </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage2Async()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
  copyright (c) FooCorp. All rights reserved.

  Line #3
</copyright> */

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage3Async()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/*
 * <copyright file=""Test0.cs"" company=""FooCorp"">
 *   copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 * </copyright>
 */

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without XML header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestNoXmlFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage1Async()
        {
            this.useNoXmlMultiLineHeaderTestSettings = true;

            var testCode = @"//   copyright (c) FooCorp. All rights reserved.
//
//   Line #3

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without XML header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestNoXmlFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage2Async()
        {
            this.useNoXmlMultiLineHeaderTestSettings = true;

            var testCode = @"/*
 *   copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 */

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without XML header will ignore spurious leading / trailing whitespaces (for multiple line comments).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1356, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1356")]
        public async Task TestNoXmlFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessage3Async()
        {
            this.useNoXmlMultiLineHeaderTestSettings = true;

            var testCode = @"/*
  copyright (c) FooCorp. All rights reserved.

  Line #3
*/

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an incorrect copyright text the fix only replaces the text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderFixWithReplaceCopyrightTagTextAsync()
        {
            var testCode = @"// <author>
//   John Doe
// </author>
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) Not FooCorp. All rights reserved.
// </copyright>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";
            var fixedCode = @"// <author>
//   John Doe
// </author>
// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(4, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we keep leading spaces in a file header when fixing text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderKeepsLeadingWhiteSpaceWhenFixingCopyrightMessageAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"    // <author>FooCorp</author>
    // <copyright file=""Test0.cs"" company=""FooCorp"">
    //  Not copyright (c) FooCorp. All rights reserved.
    //
    //  Line #3
    // </copyright>
    // <summary>
    //   FooCorp Bar class
    // </summary>

namespace Bar
{
}
";
            var fixedCode = @"    // <author>FooCorp</author>
    // <copyright file=""Test0.cs"" company=""FooCorp"">
    // copyright (c) FooCorp. All rights reserved.
    //
    // Line #3
    // </copyright>
    // <summary>
    //   FooCorp Bar class
    // </summary>

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(2, 8);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) without a leading star.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMultiLineCommentAndNoLeadingStarsFixingCopyrightMessageStaysMultiLineAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/*
   <author>FooCorp</author>
   <copyright file=""Test0.cs"" company=""FooCorp"">
     NOT copyright (c) FooCorp. All rights reserved.

     Line #3
   </copyright>
   <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var fixedCode = @"/*
   <author>FooCorp</author>
   <copyright file=""Test0.cs"" company=""FooCorp"">
   copyright (c) FooCorp. All rights reserved.

   Line #3
   </copyright>
   <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(3, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) with a leading star.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMultiLineCommentAndLeadingStarsFixingCopyrightMessageStaysMultiLineAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/*
 * <author>FooCorp</author>
 * <copyright file=""Test0.cs"" company=""FooCorp"">
 *   NOT copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 * </copyright>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var fixedCode = @"/*
 * <author>FooCorp</author>
 * <copyright file=""Test0.cs"" company=""FooCorp"">
 * copyright (c) FooCorp. All rights reserved.
 *
 * Line #3
 * </copyright>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(3, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) with a leading star
        /// and the initial line at the top of the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMultiLineCommentAndAuthorAtTopAndLeadingStarsFixingCopyrightMessageStaysMultiLineAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/* <author>FooCorp</author>
 * <copyright file=""Test0.cs"" company=""FooCorp"">
 *   NOT copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 * </copyright>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var fixedCode = @"/* <author>FooCorp</author>
 * <copyright file=""Test0.cs"" company=""FooCorp"">
 * copyright (c) FooCorp. All rights reserved.
 *
 * Line #3
 * </copyright>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(2, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) with a leading star
        /// and the initial line at the top of the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMultiLineCommentAndCopyrightAtTopAndLeadingStarsFixingCopyrightMessageStaysMultiLineAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
 *   NOT copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 * </copyright>
 * <author>FooCorp</author>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var fixedCode = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
 * copyright (c) FooCorp. All rights reserved.
 *
 * Line #3
 * </copyright>
 * <author>FooCorp</author>
 * <summary>FooCorp Bar Class</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            if (this.useMultiLineHeaderTestSettings)
            {
                return MultiLineHeaderTestSettings;
            }

            if (this.useNoXmlMultiLineHeaderTestSettings)
            {
                return NoXmlMultiLineHeaderTestSettings;
            }

            return base.GetSettings();
        }
    }
}
