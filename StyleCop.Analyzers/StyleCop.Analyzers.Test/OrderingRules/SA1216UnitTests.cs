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
    /// Unit tests for <see cref="SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives"/>.
    /// </summary>
    public class SA1216UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle an empty source.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInNamespaceAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Math;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInMultipleNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Math;
}

namespace Bar
{
    using System;
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
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
            var testCode = @"using System;
using Execute = System.Action;
using static System.Array;
using static System.Math;

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
            var testCode = @"namespace Foo
{
    using static System.Math;
    using Execute = System.Action;
    using System;
}

namespace Bar
{
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
    using System;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(12, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1216UsingStaticDirectivesMustBePlacedAfterOtherUsingDirectives();
        }
    }
}
