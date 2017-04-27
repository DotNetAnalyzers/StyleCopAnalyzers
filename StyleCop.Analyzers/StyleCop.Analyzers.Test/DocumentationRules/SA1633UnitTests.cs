﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1633 diagnostic.
    /// </summary>
    public class SA1633UnitTests : FileHeaderTestBase
    {
        private const string NoXmlMultiLineHeaderTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""copyright (c) {companyName}. All rights reserved."",
      ""xmlHeader"": false
    }
  }
}
";

        private const string DecoratedXmlMultiLineHeaderTestSettings = @"
{
  ""settings"": {
    ""documentationRules"": {
      ""companyName"": ""FooCorp"",
      ""copyrightText"": ""  Copyright (c) {companyName}. All rights reserved."",
      ""headerDecoration"": ""-----------------------------------------------------------------------"",
    }
  }
}
";

        private bool useNoXmlSettings;

        private bool useDecoratedXmlMultiLineHeaderTestSettings;

        /// <summary>
        /// Verifies that the analyzer will report <see cref="FileHeaderAnalyzers.SA1633DescriptorMissing"/> for
        /// projects using XML headers (the default) when the file is completely missing a header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public virtual async Task TestNoFileHeaderAsync()
        {
            var testCode = @"namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file with a valid header and no other content will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderNoContentAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file with a valid header and no other content will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidMultilineCommentFileHeadersAsync()
        {
            var testCode1 = @"/* <copyright file=""Test0.cs"" company=""FooCorp"">
  Copyright (c) FooCorp. All rights reserved.
</copyright> */
";

            var testCode2 = @"/*
<copyright file=""Test1.cs"" company=""FooCorp"">
  Copyright (c) FooCorp. All rights reserved.
</copyright>
*/
";

            var testCode3 = @"/*<copyright file=""Test2.cs"" company=""FooCorp"">
  Copyright (c) FooCorp. All rights reserved.
</copyright>*/
";

            var testCode4 = @"/*
 * <copyright file=""Test3.cs"" company=""FooCorp"">
 *   Copyright (c) FooCorp. All rights reserved.
 * </copyright>
*/
";

            await this.VerifyCSharpDiagnosticAsync(new[] { testCode1, testCode2, testCode3, testCode4 }, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header with leading directives will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithDirectivesAsync()
        {
            var testCode = @"#define MYDEFINE
#if (IGNORE_FILE_HEADERS)
#pragma warning disable SA1633
#endif
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            var expected = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header with leading whitespace will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithWhitespaceAsync()
        {
            var testCode = @"    // <copyright file=""Test0.cs"" company=""FooCorp"">
    //   Copyright (c) FooCorp. All rights reserved.
    // </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file without a header, but with leading trivia will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMissingFileHeaderWithLeadingTriviaAsync()
        {
            var testCode = @"#define MYDEFINE

namespace Foo
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

#define MYDEFINE

namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file without a header, but with leading trivia will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMissingFileHeaderWithDecorationAsync()
        {
            var testCode = @"namespace Foo
{
}
";
            var fixedCode = @"// -----------------------------------------------------------------------
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Foo
{
}
";

            this.useDecoratedXmlMultiLineHeaderTestSettings = true;

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header without XML structure will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNonXmlFileHeaderAsync()
        {
            var testCode = @"// Copyright (c) FooCorp. All rights reserved.

namespace Foo
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMalformed).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an invalid XML structure will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidXmlFileHeaderAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.

namespace Foo
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Foo
{
}
";

            var expected = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMalformed).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an invalid XML structure will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMalformedHeaderAsync()
        {
            var testCode = @"// <copyright test0.cs company=""FooCorp"">
#define MYDEFINE

namespace Foo
{
}
";
            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

#define MYDEFINE

namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMalformed).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that blank lines before a valid XML header will not produce a diagnostic message.
        /// This is a regression test for #1781.
        /// </summary>
        /// <param name="prefix">The string to add before the header.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("  ")]
        [InlineData("\t\t")]
        [InlineData(" \t")]
        [InlineData("  \r\n\t\r\n")]
        [InlineData("\r\n")]
        [InlineData("\r\n\r\n")]
        public async Task TestValidXmlFileHeaderWithLeadingBlankLinesAsync(string prefix)
        {
            var testCode = $@"{prefix}// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that blank lines before a valid header will not produce a diagnostic message.
        /// This is a regression test for #1781.
        /// </summary>
        /// <param name="prefix">The string to add before the header.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("  ")]
        [InlineData("\t\t")]
        [InlineData(" \t")]
        [InlineData("  \r\n\t\r\n")]
        [InlineData("\r\n")]
        [InlineData("\r\n\r\n")]
        public async Task TestValidFileHeaderWithLeadingBlankLinesAsync(string prefix)
        {
            this.useNoXmlSettings = true;
            var testCode = $@"{prefix}// copyright (c) FooCorp. All rights reserved.
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            if (this.useNoXmlSettings)
            {
                return NoXmlMultiLineHeaderTestSettings;
            }

            if (this.useDecoratedXmlMultiLineHeaderTestSettings)
            {
                return DecoratedXmlMultiLineHeaderTestSettings;
            }

            return base.GetSettings();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FileHeaderCodeFixProvider();
        }
    }
}
