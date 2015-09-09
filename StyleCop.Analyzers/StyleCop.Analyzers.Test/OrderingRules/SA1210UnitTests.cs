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

    /// <summary>
    /// Unit tests for <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/>.
    /// </summary>
    public class SA1210UnitTests : CodeFixVerifier
    {
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
            var testCode = @"using System.Threading;
using System.IO;
using System;
using System.Linq;";

            var fixedTestCode = @"using System;
using System.IO;
using System.Linq;
using System.Threading;
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(2, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var testCode = @"namespace Foo
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

            var fixedTestCode = @"namespace Foo
{
    using System;
    using System.Threading;
}

namespace Bar
{
    using System;
    using System.Threading;
    using Bar;
    using Foo;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(9, 5),
                this.CSharpDiagnostic().WithLocation(11, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithInlineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using /*A*/ System.Threading;
    using System.IO; //sth
}";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using System.IO; //sth
    using /*A*/ System.Threading;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(4, 5);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithGlobalKeywordAsync()
        {
            var testCode = @"using System.Threading;
using global::System.IO;
using global::System.Linq;
using global::System;
using XYZ = System.IO;

namespace Foo
{
    using global::Foo;
    using System;
}";

            var fixedTestCode = @"namespace Foo
{
    using System.Threading;
    using global::Foo;
    using global::System;
    using global::System.IO;
    using global::System.Linq;
    using XYZ = System.IO;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(3, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithNamespaceAliasQualifierAsync()
        {
            var testCode = @"using System.Threading;
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

            var fixedTestCode = @"namespace Foo
{
    using System.Threading;
    using global::Foo;
    using global::System;
    using global::System.IO;
    using global::System.Linq;
}";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
            var testCode = @"using System.IO;
using System;
using A2 = System.IO;
using A1 = System.Threading;";

            var fixedTestCode = @"using System;
using System.IO;
using A1 = System.Threading;
using A2 = System.IO;
";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(1, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUsingDirectivesWithNonWordCharactersAsync()
        {
            var testCode = @"namespace \u0041Test_ {}
namespace ATestA {}

namespace Test
{
    using Test;
    using \u0041Test_;
    using ATestA;
}";

            var fixedTestCode = @"namespace \u0041Test_ {}
namespace ATestA {}

namespace Test
{
    using \u0041Test_;
    using ATestA;
    using Test;
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 5);

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
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#else
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#endif";

            var fixedTestCode = @"using System;
using Microsoft.VisualStudio;
using MyList = System.Collections.Generic.List<int>;

#if true
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
#else
using Microsoft.CodeAnalysis.Diagnostics;
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
            yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
