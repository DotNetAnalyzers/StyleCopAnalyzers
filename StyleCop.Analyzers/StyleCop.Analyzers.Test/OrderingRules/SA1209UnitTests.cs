namespace StyleCop.Analyzers.Test
{
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using System.Threading;
    using System.Threading.Tasks;

    using TestHelper;
    using Xunit;

    public class SA1209UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestEmptySource()
        {
            var testCode = string.Empty;
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestWhenAliasUsingDirectivesArePlacedCorrectly()
        {
            string usingsInCompilationUnit = @"using System;
using SomeNamespace = System.IO;

class A
{
}";

            string usingsInNamespaceDeclaration = @"namespace Test
{
    using System;
    using SomeNamespace = System.IO;

    class A
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, EmptyDiagnosticResults, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None);
        }

        [Fact]
        public async Task TestWhenUsingAliasDirectivesAreNotPlacedCorrectly()
        {
            var testCodeCompilationUnit = @"using TasksNamespace = System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
using System.Linq;
class A
{
}";

            var testCodeNamespace = @"namespace Test
{
    using System.Net;
    using System.Threading;
    using L = System.Linq;
    using System.IO;
    using P = System.Threading.Tasks;
    class A
    {
    }
}";

            DiagnosticResult[] expectedForCompilationUnit =
            {
                this.CSharpDiagnostic().WithLocation(1, 1)
            };

            DiagnosticResult[] expectedForNamespaceDeclaration =
            {
                this.CSharpDiagnostic().WithLocation(5, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCodeCompilationUnit, expectedForCompilationUnit, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(testCodeNamespace, expectedForNamespaceDeclaration, CancellationToken.None);
        }

        [Fact]
        public async Task TestUsingAliasDirectivesWithGlobalContextualKeyword()
        {
            var compilationUnit = @"using global::System.Threading.Tasks;
using Name = global::System.Threading;
using global::System.IO;
namespace Test
{
    using System.Text;
    using System.Threading;
    using global::System;
    class A
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test0.cs", 2, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithPreprocessorDirectives()
        {
            var compilationUnit = @"#define DEBUG
namespace Test
{
    using System;
    using System.Threading;
#if DEBUG
    using IO = System.IO;
#endif
    using System.Text;
    class A
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test0.cs", 7, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithInlineCommentsInUsingAliasDirectives()
        {
            var namespaceDeclaration = @"namespace Test
{
    using System;
    using Threads = /* inline comment */ System.Threading;
    using System.IO;
    using /* comment */ System.Text;
    class A
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test0.cs", 4, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithUsingStatic()
        {
            var namespaceDeclaration = @"namespace Test
{
    using System;
    using IO = System.IO;
    using System.Net;
    using Threads = System.Threading;
    using static System.Math;
    class A
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test0.cs", 4, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, expected, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
        }
    }
}