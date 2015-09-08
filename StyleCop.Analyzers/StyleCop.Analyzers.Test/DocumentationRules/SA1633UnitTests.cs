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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMissing).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
#define MYDEFINE

namespace Foo
{
}
";

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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMalformed).WithLocation(1, 1);
            await
                this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None)
                    .ConfigureAwait(false);

            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>

namespace Foo
{
}
";
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

            var expectedDiagnostic = this.CSharpDiagnostic(FileHeaderAnalyzers.SA1633DescriptorMalformed).WithLocation(1, 1);
            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostic, CancellationToken.None).ConfigureAwait(false);

            var fixedCode = @"// <copyright file=""Test0.cs"" company=""FooCorp"">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
#define MYDEFINE

namespace Foo
{
}
";

            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1633CodeFixProvider();
        }
    }
}
