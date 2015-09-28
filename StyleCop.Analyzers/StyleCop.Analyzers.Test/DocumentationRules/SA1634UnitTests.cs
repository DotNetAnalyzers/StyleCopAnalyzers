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
    /// Unit tests for the SA1634 diagnostic.
    /// </summary>
    public class SA1634UnitTests : FileHeaderTestBase
    {
        private string multiLineSettings;

        /// <summary>
        /// Verifies that a file header without a copyright element will produce the expected diagnostic (none for the default case)
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMissingCopyrightTagAsync()
        {
            var testCode = @"// <author>
//   John Doe
// </author>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <author>
//   John Doe
// </author>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright element in short hand notation will not produce SA1634.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithShorthandCopyrightAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp""/>

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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1635Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a copyright element in wrong casing will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidFileHeaderWithCopyrightInWrongCaseAsync()
        {
            var testCode = @"// <Copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </Copyright>

namespace Bar
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <Copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </Copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Check multiple line copyright headers can be checked correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidMultiLineFileHeaderWithCopyrightLastAsync()
        {
            var testCode = @"// <author>
//   John Doe
// </author>
// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
//
// Licence is FooBar MIT.
// </copyright>

namespace Bar
{
}
";
            this.multiLineSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""Copyright (c) FooCorp. All rights reserved.\n\nLicence is FooBar MIT.""
    }
  }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that we keep leading spaces in a file header when adding copyright text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderKeepsLeadingWhiteSpaceWhenAddingCopyrightMessageAsync()
        {
            var testCode = @"    // <author>FooCorp</author>
    // <summary>
    //   FooCorp Bar class
    // </summary>

namespace Bar
{
}
";
            var fixedCode = @"    // <copyright file=""Test0.cs"" company=""FooCorp"">
    // Copyright (c) FooCorp. All rights reserved.
    // </copyright>
    // <author>FooCorp</author>
    // <summary>
    //   FooCorp Bar class
    // </summary>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 5);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with missing copyright text the fix leaves behind other comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderFixWithReplaceCopyrightTagTextAsync()
        {
            var testCode = @"// <author>
//   John Doe
// </author>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <author>
//   John Doe
// </author>
// <summary>This is a test file.</summary>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with missing copyright text and a multiline comment without leading stars the fix leaves behind other comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderForMultilineCommentWithoutLeadingStarsFixWithReplaceCopyrightTagTextAsync()
        {
            var testCode = @"/* <author>
     John Doe
   </author>
   <summary>This is a test file.</summary>
 */

namespace Bar
{
}
";
            var fixedCode = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
   Copyright (c) FooCorp. All rights reserved.
   </copyright>
   <author>
     John Doe
   </author>
   <summary>This is a test file.</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with missing copyright text and a multiline comment with leading stars the fix leaves behind other comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderForMultilineCommentWithLeadingStarsFixWithReplaceCopyrightTagTextAsync()
        {
            var testCode = @"/* <author>
 *   John Doe
 * </author>
 * <summary>This is a test file.</summary>
 */

namespace Bar
{
}
";
            var fixedCode = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
 * Copyright (c) FooCorp. All rights reserved.
 * </copyright>
 * <author>
 *   John Doe
 * </author>
 * <summary>This is a test file.</summary>
 */

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a multi line file header containing characters that need xml escaping gets fixed correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithMultiLineCommentAndFieldsNeedingXmlEscapingAsync()
        {
            this.multiLineSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""Foo & Bar \""quote\"" Corp"",
      ""copyrightText"": ""copyright (c) {companyName}. All rights reserved.\n\nLine #3""
    }
  }
}
";

            var testCode = @"// <author>FooCorp</author>
// <summary>Foo &amp; Bar Corp Bar Class</summary>
 
namespace Bar
{
}
";

            var fixedCode = @"// <copyright file=""Test0.cs"" company=""Foo &amp; Bar &quot;quote&quot; Corp"">
// copyright (c) Foo &amp; Bar ""quote"" Corp. All rights reserved.
//
// Line #3
// </copyright>
// <author>FooCorp</author>
// <summary>Foo &amp; Bar Corp Bar Class</summary>
 
namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1634Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FileHeaderCodeFixProvider();
        }

        protected override string GetSettings()
        {
            return this.multiLineSettings ?? base.GetSettings();
        }
    }
}
