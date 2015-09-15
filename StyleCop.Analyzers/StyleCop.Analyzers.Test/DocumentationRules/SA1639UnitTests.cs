// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.DocumentationRules;
    using Xunit;

    /// <summary>
    /// Unit tests for the SA1639 diagnostic.
    /// </summary>
    public class SA1639UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that a file header without a summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithoutSummaryTagAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1639Descriptor).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an empty summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithEmptySummaryTagAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary/>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1639Descriptor).WithLocation(4, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with a whitespace only summary element will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestFileHeaderWithWhitespaceOnlySummaryTagAsync()
        {
            var testCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
// <summary>   </summary>

namespace Bar
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1639Descriptor).WithLocation(4, 4);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that this analyzer is disabled by default.
        /// </summary>
        [Fact]
        public void TestDisabledByDefault()
        {
            Assert.False(FileHeaderAnalyzers.SA1639Descriptor.IsEnabledByDefault);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            return Enumerable.Empty<string>();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            throw new System.NotImplementedException();
        }
    }
}
