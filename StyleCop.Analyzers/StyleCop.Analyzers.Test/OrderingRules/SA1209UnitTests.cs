// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    public class SA1209UnitTests
    {
        [Fact]
        public async Task TestWhenAliasUsingDirectivesArePlacedCorrectlyInCompilationAsync()
        {
            string usingsInCompilationUnit = @"using System;
using SomeNamespace = System.IO;

class A
{
}";

            await VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenAliasUsingDirectivesArePlacedCorrectlyInNamespaceAsync()
        {
            string usingsInNamespaceDeclaration = @"namespace Test
{
    using System;
    using SomeNamespace = System.IO;

    class A
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenUsingAliasDirectivesAreNotPlacedCorrectlyInCompilationAsync()
        {
            var testCodeCompilationUnit = @"using TasksNamespace = System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
using System.Linq;

class A
{
}";
            var fixedTestCodeCompilationUnit = @"using System;
using System.IO;
using System.Linq;
using System.Net;
using TasksNamespace = System.Threading.Tasks;

class A
{
}";

            DiagnosticResult expectedForCompilationUnit = Diagnostic().WithLocation(1, 1);

            await VerifyCSharpFixAsync(testCodeCompilationUnit, expectedForCompilationUnit, fixedTestCodeCompilationUnit, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWhenUsingAliasDirectivesAreNotPlacedCorrectlyInNamespaceAsync()
        {
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
            var fixedTestCodeNamespace = @"namespace Test
{
    using System.IO;
    using System.Net;
    using System.Threading;
    using L = System.Linq;
    using P = System.Threading.Tasks;

    class A
    {
    }
}";

            DiagnosticResult expectedForNamespaceDeclaration = Diagnostic().WithLocation(5, 5);

            await VerifyCSharpFixAsync(testCodeNamespace, expectedForNamespaceDeclaration, fixedTestCodeNamespace, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingAliasDirectivesWithGlobalContextualKeywordAsync()
        {
            var testCode = @"using global::System.Threading.Tasks;
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

            var fixedTestCode = @"using global::System.IO;
using global::System.Threading.Tasks;
using Name = global::System.Threading;

namespace Test
{
    using System.Text;
    using System.Threading;
    using global::System;

    class A
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation("Test0.cs", 2, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithPreprocessorDirectivesAsync()
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

            await VerifyCSharpDiagnosticAsync(compilationUnit, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithInlineCommentsInUsingAliasDirectivesAsync()
        {
            var testCode = @"namespace Test
{
    using System;
    using Threads = /* inline comment */ System.Threading;
    using System.IO;
    using /* comment */ System.Text;

    class A
    {
    }
}";

            var fixedTestCode = @"namespace Test
{
    using System;
    using System.IO;
    using /* comment */ System.Text;
    using Threads = /* inline comment */ System.Threading;

    class A
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation("Test0.cs", 4, 5);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestWithUsingStaticAsync()
        {
            var testCode = @"namespace Test
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

            var fixedTestCode = @"namespace Test
{
    using System;
    using System.Net;
    using static System.Math;
    using IO = System.IO;
    using Threads = System.Threading;

    class A
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation("Test0.cs", 4, 5);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;

#if true
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#else
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#endif";

            var fixedTestCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;

#if true
using Microsoft.CodeAnalysis;
using Threads = System.Threading;
#else
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#endif";

            // else block is skipped
            var expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
