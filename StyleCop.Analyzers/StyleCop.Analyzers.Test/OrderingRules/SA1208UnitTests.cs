// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1208UnitTests
    {
        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreOnTopInCompilationAsync()
        {
            string usingsInCompilationUnit = @"using System;
using System.IO;

class A
{
}";

            await VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSystemUsingDirectivesWithEscapeSequenceAsync()
        {
            string usingsInNamespaceDeclaration = @"namespace Test
{
    using @System;
    using System.Diagnostics;
    using \u0053ystem.IO;
    using System.Threading;
}";

            await VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreNotOnTopInCompilationAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test2.cs", 2, 1).WithArguments("System", "Xyz"),
                    Diagnostic().WithLocation("/0/Test2.cs", 3, 1).WithArguments("System.IO", "Xyz"),
                    Diagnostic().WithLocation("/0/Test2.cs", 5, 1).WithArguments("System.Threading.Tasks", "Xyz"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenSystemUsingDirectivesAreNotOnTopInNamespaceAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test2.cs", 4, 5).WithArguments("System.Threading", "Namespace"),
                    Diagnostic().WithLocation("/0/Test2.cs", 5, 5).WithArguments("System.IO", "Namespace"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSystemUsingDirectivesInCompilationUnitAndInNamespaceDeclarationAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test3.cs", 2, 1).WithArguments("System.IO", "AnotherNamespace"),
                    Diagnostic().WithLocation("/0/Test3.cs", 8, 5).WithArguments("System.Threading", "Xyz"),
                    Diagnostic().WithLocation("/0/Test3.cs", 9, 5).WithArguments("System", "Xyz"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPlaceSystemUsingDirectivesWithGlobalContextualKeywordAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test3.cs", 8, 5).WithArguments("System.Threading", "Xyz"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(compilationUnitWithoutDiagnostic, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            await new CSharpTest
            {
                TestSources =
                {
                    source1,
                    source2,
                    source3,
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test2.cs", 6, 5).WithArguments("System.IO", "System.IO.Path"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(2, 1).WithArguments("System", "System.IO.Path");

            await VerifyCSharpDiagnosticAsync(namespaceDeclarationWithoutDiagnostic, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(source, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenThereArePreprocessorDirectivesCloseToUsingDirectivesAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test2.cs", 5, 5).WithArguments("System.Threading", "Namespace"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithInlineCommentsInUsingDirectivesAsync()
        {
            await new CSharpTest
            {
                TestSources =
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
}",
                },
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation("/0/Test2.cs", 4, 5).WithArguments("System.Threading", "Namespace"),
                    Diagnostic().WithLocation("/0/Test2.cs", 5, 5).WithArguments("System.IO", "Namespace"),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"using System;
using Microsoft.Win32;
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
using Microsoft.Win32;
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
                Diagnostic().WithLocation(8, 1).WithArguments("System.Threading", "Microsoft.CodeAnalysis.CSharp"),
                Diagnostic().WithLocation(9, 1).WithArguments("System.Collections", "Microsoft.CodeAnalysis.CSharp"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1818.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesWithGlobalAliasAsync()
        {
            var testCode = @"
namespace Foo
{
    extern alias corlib;
    using System;
    using System.Threading;
    using corlib::System;
    using Foo;
    using global::Foo;
    using global::System;
    using global::System.IO;
    using global::System.Linq;
    using Microsoft;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
