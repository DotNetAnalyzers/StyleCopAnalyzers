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
    /// Unit tests for the SA1638 diagnostic.
    /// </summary>
    public class SA1638UnitTests : FileHeaderTestBase
    {
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

        private bool useDecoratedXmlMultiLineHeaderTestSettings;

        /// <summary>
        /// Verifies that a file header with a mismatching file attribute in the copyright element will produce the expected diagnostic message.
        /// </summary>
        /// <param name="lineEnding">The line ending to use in the test code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestCopyrightElementWithMismatchingFileAttributeAsync(string lineEnding)
        {
            var testCode = @"// {|#0:<copyright|} file=""wrongfile.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

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

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1638Descriptor).WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a mismatching file attribute in the copyright element will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCopyrightElementWithMismatchingFileAttributeAndDecorationAsync()
        {
            var testCode = @"// -----------------------------------------------------------------------
// <copyright file=""wrongfile.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bar
{
}
";
            var fixedCode = @"// -----------------------------------------------------------------------
// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bar
{
}
";

            this.useDecoratedXmlMultiLineHeaderTestSettings = true;

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1638Descriptor).WithLocation(2, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            if (this.useDecoratedXmlMultiLineHeaderTestSettings)
            {
                return DecoratedXmlMultiLineHeaderTestSettings;
            }

            return base.GetSettings();
        }
    }
}
