// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.OrderingRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    public class SA1208UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreOnTopInCompilationAsync()
        {
            string usingsInCompilationUnit = @"using System;
using System.IO;

class A
{
}";

            await this.VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreOnTopInNamespaceAsync()
        {
            string usingsInNamespaceDeclaration = @"namespace Test
{
    using System;
    using System.IO;

    class A
    {
    }
}";

            await this.VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreNotOnTopInCompilationAsync()
        {
            var usingsInCompilationUnit = new[]
            {
                "namespace Xyz {}",
                "namespace AnotherNamespace {}",
                @"using Xyz;
using System;
using System.IO;
using AnotherNamespace;
using System.Threading.Tasks;

class A
{
}"
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 2, 1).WithArguments("System", "Xyz"),
                this.CSharpDiagnostic().WithLocation("Test2.cs", 3, 1).WithArguments("System.IO", "Xyz"),
                this.CSharpDiagnostic().WithLocation("Test2.cs", 5, 1).WithArguments("System.Threading.Tasks", "Xyz")
            };

            await this.VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreNotOnTopInNamespaceAsync()
        {
            var usingsInNamespaceDeclaration = new[]
            {
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"namespace Test
{
    using Namespace;
    using System.Threading;
    using System.IO;
    using AnotherNamespace;

    class A
    {
    }
}"
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 4, 5).WithArguments("System.Threading", "Namespace"),
                this.CSharpDiagnostic().WithLocation("Test2.cs", 5, 5).WithArguments("System.IO", "Namespace")
            };

            await this.VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSystemUsingDirectivesInCompilationUnitAndInNamespaceDeclarationAsync()
        {
            var sources = new[]
            {
                "namespace Xyz {}",
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"using AnotherNamespace;
using System.IO;
using Namespace;

namespace Test
{
    using Xyz;
    using System.Threading;
    using System;

    class A
    {
    }
}" };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test3.cs", 2, 1).WithArguments("System.IO", "AnotherNamespace"),
                this.CSharpDiagnostic().WithLocation("Test3.cs", 8, 5).WithArguments("System.Threading", "Xyz"),
                this.CSharpDiagnostic().WithLocation("Test3.cs", 9, 5).WithArguments("System", "Xyz")
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPlaceSystemUsingDirectivesWithGlobalContextualKeywordAsync()
        {
            var sources = new[]
            {
                "namespace Xyz {}",
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"using global::AnotherNamespace;
using global::System.IO;
using Namespace;

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

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test3.cs", 2, 1).WithArguments("global::System.IO", "global::AnotherNamespace"),
                this.CSharpDiagnostic().WithLocation("Test3.cs", 8, 5).WithArguments("System.Threading", "Xyz"),
                this.CSharpDiagnostic().WithLocation("Test3.cs", 9, 5).WithArguments("global::System", "Xyz")
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithUsingAliasDirectivesAsync()
        {
            var compilationUnitWithoutDiagnostic = @" using System;
using A = System.IO;

class A
{
}";

            var source1 = "namespace Xyz { public class XyzType {} }";
            var source2 = "namespace AnotherNamespace {}";
            var source3 = @"using System;
namespace Test 
{
    using System;
    using Alias = System.IO.Path;
    using System.IO;

    class A
    {
    }
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 6, 5).WithArguments("System.IO", "System.IO.Path")
            };

            await this.VerifyCSharpDiagnosticAsync(compilationUnitWithoutDiagnostic, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(new[] { source1, source2, source3 }, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithUsingStaticCSharpSixFeatureAsync()
        {
            var namespaceDeclarationWithoutDiagnostic = @"namespace Test
{
    using System;
    using System.IO;
    using static System.IO.Path;

    class A
    {
    }
}";

            var source = @"using static System.IO.Path;
using System;
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(2, 1).WithArguments("System", "System.IO.Path");

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclarationWithoutDiagnostic, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(source, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenThereArePreprocessorDirectivesCloseToUsingDirectivesAsync()
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
    using System.IO;
#endif
    using AnotherNamespace;

    class A
    {
    }
}"
        };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 5, 5).WithArguments("System.Threading", "Namespace")
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithInlineCommentsInUsingDirectivesAsync()
        {
            var sources = new[]
            {
                "namespace Namespace {}",
                "namespace AnotherNamespace {}",
                @"namespace Test
{
    using Namespace;
    using /* inline comment */ System.Threading;
    using System.IO;
    using /* comment */ AnotherNamespace;

    class A
    {
    }
}"
            };

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation("Test2.cs", 4, 5).WithArguments("System.Threading", "Namespace"),
                this.CSharpDiagnostic().WithLocation("Test2.cs", 5, 5).WithArguments("System.IO", "Namespace")
            };

            await this.VerifyCSharpDiagnosticAsync(sources, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"using System;
using Microsoft.VisualStudio;
using MyList = System.Collections.Generic.List<int>;
using Microsoft.CodeAnalysis;

#if true
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using System.Collections;
#if true
using System.Collections.Generic;
#endif
#else
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using System.Collections;
#endif";

            var fixedTestCode = @"using System;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using MyList = System.Collections.Generic.List<int>;

#if true
using System.Collections;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
#if true
using System.Collections.Generic;
#endif
#else
using Microsoft.CodeAnalysis.CSharp;
using System.Threading;
using System.Collections;
#endif";

            // else block is skipped
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(8, 1).WithArguments("System.Threading", "Microsoft.CodeAnalysis.CSharp"),
                this.CSharpDiagnostic().WithLocation(9, 1).WithArguments("System.Collections", "Microsoft.CodeAnalysis.CSharp")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
