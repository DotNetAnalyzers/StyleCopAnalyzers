namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;

    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/>.
    /// </summary>
    public class SA1210UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySourceAsync()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProperOrderedUsingDirectivesInCompilationUnitAsync()
        {
            var compilationUnit = @"
using System;
using System.IO;
using System.Threading;";

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestProperOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var namespaceDeclaration = @"namespace Foo
{
    using System;
    using System.Threading;
}

namespace Bar
{
    using System;
    using Foo;
}
";

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInCompilationUnitAsync()
        {
            var compilationUnit = @"
using System.IO;
using System.Threading;
using System;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 1).WithArguments("System", "System.IO");

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var namespaceDeclaration = @"
namespace Foo
{
    using System.Threading;
    using System;
}

namespace Bar
{
    using Foo;
    using System.Threading;
    using System;
}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(5, 5).WithArguments("System", "System.Threading"),
                this.CSharpDiagnostic().WithLocation(11, 5).WithArguments("System.Threading", "Foo"),
                this.CSharpDiagnostic().WithLocation(12, 5).WithArguments("System", "Foo")
            };

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithInlineCommentsAsync()
        {
            var namespaceDeclaration = @"
namespace Foo
{
    using System;
    using /*A*/ System.Threading;
    using System.IO; //sth
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5).WithArguments("System.IO", "System.Threading");

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithGlobalKeywordAsync()
        {
            var compilationUnit = @"
using System.Threading;
using global::System.IO;
using global::System;

namespace Foo
{
    using global::Foo;
    using System;
}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 1).WithArguments("System.IO", "System.Threading"),
                this.CSharpDiagnostic().WithLocation(4, 1).WithArguments("System", "System.Threading"),
                this.CSharpDiagnostic().WithLocation(9, 5).WithArguments("System", "Foo")
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
        }
    }
}
