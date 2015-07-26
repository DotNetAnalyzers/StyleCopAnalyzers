namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName"/>.
    /// </summary>
    public class SA1211UnitTests : DiagnosticVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInNamespaceAsync()
        {
            const string testCode = @"namespace Foo
{
    using System;
    using Character = System.Char;
    using @int = System.Int32;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in multiple namespaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInMultipleNamespacesAsync()
        {
            const string testCode = @"namespace Foo
{
    using System;
    using character = System.Char;
    using \u0069nt = System.Int32;
}

namespace Bar
{
    using System;
    using MemoryStream = System.IO.MemoryStream;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
}

namespace Spam
{
    using System;
    using Character = System.Char;
    using @int = System.Int32;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in the compilation unit.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInCompilationUnitAsync()
        {
            const string testCode = @"using System;
using character = System.Char;
using \u0069nt = System.Int32;
using StringBuilder = System.Text.StringBuilder;
using StringWriter = System.IO.StringWriter;

public class Foo
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the using directives are ordered wrong.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingDirectivesOrderingAsync()
        {
            const string testCode = @"namespace Foo
{
    using System;
    using \u0069nt = System.Int32;
    using character = System.Char;
}

namespace Bar
{
    using System;
    using Stream = System.IO.Stream;
    using StringBuilder = System.Text.StringBuilder;
    using StringWriter = System.IO.StringWriter;
    using MemoryStream = System.IO.MemoryStream;
}

namespace Spam
{
    using System;
    using @int = System.Int32;
    using Character = System.Char;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("character", "int"),
                this.CSharpDiagnostic().WithLocation(14, 5).WithArguments("MemoryStream", "Stream"),
                this.CSharpDiagnostic().WithLocation(21, 5).WithArguments("Character", "int")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName();
        }
    }
}
