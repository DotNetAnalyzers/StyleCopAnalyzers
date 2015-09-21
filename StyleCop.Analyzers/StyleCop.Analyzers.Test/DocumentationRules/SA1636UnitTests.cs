// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header will ignore spurious leading / trailing whitespaces (for multiple line comments)
        /// This is a regression for #1356
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessageAsync()
        {
            this.useMultiLineHeaderTestSettings = true;

            var testCode1 = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   copyright (c) FooCorp. All rights reserved.
//
//   Line #3
// </copyright>

namespace Bar
{
}
";

            var testCode2 = @"/* <copyright file=""Test1.cs"" company=""FooCorp"">
  copyright (c) FooCorp. All rights reserved.

  Line #3
</copyright> */

namespace Bar
{
}
";

            var testCode3 = @"/*
 * <copyright file=""Test2.cs"" company=""FooCorp"">
 *   copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 * </copyright>
 */

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(new[] { testCode1, testCode2, testCode3 }, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without XML header will ignore spurious leading / trailing whitespaces (for multiple line comments)
        /// This is a regression for #1356
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoXmlFileHeaderWillIgnoreLeadingAndTrailingWhitespaceAroundCopyrightMessageAsync()
        {
            this.useNoXmlMultiLineHeaderTestSettings = true;

            var testCode1 = @"//   copyright (c) FooCorp. All rights reserved.
//
//   Line #3

namespace Bar
{
}
";

            var testCode2 = @"/*
 *   copyright (c) FooCorp. All rights reserved.
 *
 *   Line #3
 */

namespace Bar
{
}
";

            var testCode3 = @"/*
  copyright (c) FooCorp. All rights reserved.

  Line #3
*/

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(new[] { testCode1, testCode2, testCode3 }, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(4, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(2, 8);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) without a leading star
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(3, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header will be fixed correctly (for multiple line comments) with a leading star
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(3, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(2, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1636Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FileHeaderCodeFixProvider();
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
