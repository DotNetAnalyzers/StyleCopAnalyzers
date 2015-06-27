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
            var compilationUnit = @"using System;
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
            var compilationUnit = @"using System.Threading;
using System.IO;
using System;
using System.Linq;";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var namespaceDeclaration = @"namespace Foo
{
    using System.Threading;
    using System;
}

namespace Bar
{
    using Foo;
    using Bar;
    using System.Threading;
    using System;
}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(9, 5),
                this.CSharpDiagnostic().WithLocation(11, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithInlineCommentsAsync()
        {
            var namespaceDeclaration = @"namespace Foo
{
    using System;
    using /*A*/ System.Threading;
    using System.IO; //sth
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 5);

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithGlobalKeywordAsync()
        {
            var compilationUnit = @"using System.Threading;
using global::System.IO;
using global::System.Linq;
using global::System;
using XYZ = System.IO;

namespace Foo
{
    using global::Foo;
    using System;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithNamespaceAliasQualifierAsync()
        {
            var compilationUnit = @"using System.Threading;
using global::System.IO;
using global::System.Linq;
using global::System;
using global::Foo;
using Foo;

namespace Foo
{
    using global::Foo;
    using System;
}";

            DiagnosticResult[] expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidOrderedUsingDirectivesWithStaticUsingDirectivesAsync()
        {
            var namespaceDeclaration = @"namespace Foo
{
    using System;
    using Foo;
    using static System.Uri;
    using static System.Math;
}";

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingWithUsingAliasDirectivesAsync()
        {
            var compilationUnit = @"using System.IO;
using System;
using A2 = System.IO;
using A1 = System.Threading;";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingDirectivesWithNonWordCharactersAsync()
        {
            var compilationUnit = @"namespace \u0041Test_ {}
namespace ATestA {}

namespace Test
{
    using Test;
    using \u0041Test_;
    using ATestA;
}";
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
        }
    }
}
