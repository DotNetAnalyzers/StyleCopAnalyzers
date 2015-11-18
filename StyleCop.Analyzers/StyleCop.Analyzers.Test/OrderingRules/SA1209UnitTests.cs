// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;

    public class SA1209UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestWhenAliasUsingDirectivesArePlacedCorrectlyInCompilationAsync()
        {
            string usingsInCompilationUnit = @"using System;
using SomeNamespace = System.IO;

class A
{
}";

            await this.VerifyCSharpDiagnosticAsync(usingsInCompilationUnit, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(usingsInNamespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expectedForCompilationUnit = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCodeCompilationUnit, expectedForCompilationUnit, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeCompilationUnit, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCodeCompilationUnit, fixedTestCodeCompilationUnit).ConfigureAwait(false);
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

            DiagnosticResult expectedForNamespaceDeclaration = this.CSharpDiagnostic().WithLocation(5, 5);

            await this.VerifyCSharpDiagnosticAsync(testCodeNamespace, expectedForNamespaceDeclaration, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCodeNamespace, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCodeNamespace, fixedTestCodeNamespace).ConfigureAwait(false);
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

            var fixedTestCode = @"namespace Test
{
    using System.Text;
    using System.Threading;
    using global::System;
    using global::System.IO;
    using global::System.Threading.Tasks;
    using Name = global::System.Threading;

    class A
    {
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation("Test0.cs", 2, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(compilationUnit, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation("Test0.cs", 4, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation("Test0.cs", 4, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.VisualStudio;
using MyList = System.Collections.Generic.List<int>;

#if true
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#else
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#endif";

            var fixedTestCode = @"using System;
using Microsoft.VisualStudio;
using MyList = System.Collections.Generic.List<int>;

#if true
using Microsoft.CodeAnalysis;
using Threads = System.Threading;
#else
using Threads = System.Threading;
using Microsoft.CodeAnalysis;
#endif";

            // else block is skipped
            var expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
