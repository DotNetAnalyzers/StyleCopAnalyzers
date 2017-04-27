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
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCopyrightElementWithMismatchingFileAttributeAsync()
        {
            var testCode = @"// <copyright file=""wrongfile.cs"" company=""FooCorp"">
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1638Descriptor).WithLocation(1, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1638Descriptor).WithLocation(2, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FileHeaderCodeFixProvider();
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
