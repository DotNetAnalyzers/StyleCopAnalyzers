namespace StyleCop.Analyzers.Test.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis.Diagnostics;

    using StyleCop.Analyzers.DocumentationRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="SA1633FileMustHaveHeader"/> analyzer.
    /// </summary>
    public class SA1633UnitTests : FileHeaderTestBase
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source file and produce the correct diagnostic
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        public override async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1).WithArguments("is missing or not located at the top of the file.");
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file with a valid header and no other content will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderNoContentAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header with leading directives will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithDirectivesAsync()
        {
            var testCode = @"#define MYDEFINE
#if (IGNORE_FILE_HEADERS)
#pragma warning disable SA1633
#endif
// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a valid file header with leading whitespace will not produce a diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidFileHeaderWithWhitespaceAsync()
        {
            var testCode = @"    // <copyright file=""test0.cs"" company=""FooCorp"">
    //   Copyright (c) FooCorp. All rights reserved.
    // </copyright>

namespace Bar
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file without a header will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestMissingFileHeaderAsync()
        {
            var testCode = @"namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1).WithArguments("is missing or not located at the top of the file.");
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1).WithArguments("is missing or not located at the top of the file.");
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
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

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1).WithArguments("XML is invalid.");
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a file header with an invalid XML structure will produce the correct diagnostic message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidXmlFileHeaderAsync()
        {
            var testCode = @"// <copyright file=""test0.cs"" company=""FooCorp"">
//   Copyright (c) FooCorp. All rights reserved.

namespace Foo
{
}
";

            var expectedDiagnostic = this.CSharpDiagnostic().WithLocation(1, 1).WithArguments("XML is invalid.");
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1633FileMustHaveHeader();
        }
    }
}
