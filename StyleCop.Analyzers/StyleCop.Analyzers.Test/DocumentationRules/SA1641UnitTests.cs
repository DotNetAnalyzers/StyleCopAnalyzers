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
    /// Unit tests for the SA1641 diagnostic.
    /// </summary>
    public class SA1641UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that a file header with a company attribute in the copyright element that does not match the settings will produce the expected diagnostic message.
        /// </summary>
        /// <param name="lineEnding">The line ending to use in the test code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestCopyrightElementWithWrongCompanyAttributeAsync(string lineEnding)
        {
            var testCode = @"// {|#0:<copyright|} file=""Test0.cs"" company=""WrongCompany"">
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

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1641Descriptor).WithLocation(0);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a company attribute in the copyright element that only differs in case from the settings will produce the expected diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCopyrightElementWithInvalidCaseCompanyAttributeAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""foocorp"">
//   Copyright (c) FooCorp. All rights reserved.
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

            var expectedDiagnostic = Diagnostic(FileHeaderAnalyzers.SA1641Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
