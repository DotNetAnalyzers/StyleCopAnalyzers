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
            var usingsInCompilationUnit = new[]
            {
                "namespace Xyz {}",
                "namespace AnotherNamespace {}",
                @"using TasksNamespace = System.Threading.Tasks;
using Xyz;
using System;
using System.IO;
using AnotherNamespace;
class A
{
}"
            };

            var usingsInNamespaceDeclaration = new[]
            {
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"namespace Test
{
    using Namespace;
    using System.Threading;
    using N = AnotherNamespace;
    using System.IO;
    using P = System.Threading.Tasks;
    class A
    {
    }
}"      };

            var expectedForCompilationUnit = new[]
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 1, 1).WithArguments("TasksNamespace", "AnotherNamespace"),
            };

            var expectedForNamespaceDeclaration = new[]
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 5, 5).WithArguments("N", "System.Threading.Tasks"),
            };

            await this.VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, expectedForCompilationUnit, CancellationToken.None);
            await this.VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, expectedForNamespaceDeclaration, CancellationToken.None);
        }

        [Fact]
        public async Task TestUsingAliasDirectivesWithGlobalContextualKeyword()
        {
            var sources = new[]
            {
                "namespace Xyz {}",
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"using global::AnotherNamespace;
using Name = global::System.Threading;
using global::System.IO;
namespace Test
{
    using Xyz;
    using System.Threading;
    using global::System;
    class A
    {
    }
}"
        };

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation("Test3.cs", 2, 1).WithArguments("Name", "System.IO"),
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithPreprocessorDirectives()
        {
            var sources = new[]
            {
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"#define DEBUG
namespace Test
{
    using Namespace;
    using System.Threading;
#if DEBUG
    using IO = System.IO;
#endif
    using AnotherNamespace;
    class A
    {
    }
}"
        };

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 7, 5).WithArguments("IO", "AnotherNamespace"),
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithInlineCommentsInUsingAliasDirectives()
        {
            var sources = new[]
            {
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"namespace Test
{
    using Namespace;
    using Threads = /* inline comment */ System.Threading;
    using System.IO;
    using /* comment */ AnotherNamespace;
    class A
    {
    }
}"      };

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 4, 5).WithArguments("Threads", "AnotherNamespace"),
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None);
        }

        [Fact]
        public async Task TestWithUsingStatic()
        {
            var namespaceDeclarationWithoutDiagnostic = @"namespace Test
{
    using System;
    using IO = System.IO;
    using static System.IO.Path;
    using Threads = System.Threading;
    using static System.IO.Directory;
    class A
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclarationWithoutDiagnostic, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
        }
    }
}