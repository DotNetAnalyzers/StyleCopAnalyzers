// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/>.
    /// </summary>
    public class SA1210UnitTests
    {
        [Fact]
        public async Task TestProperOrderedUsingDirectivesInCompilationUnitAsync()
        {
            var compilationUnit = @"using System;
using System.IO;
using System.Threading;";

            await VerifyCSharpDiagnosticAsync(compilationUnit, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(namespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(2, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(9, 5),
                Diagnostic().WithLocation(11, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2336, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2336")]
        public async Task TestUsingDirectivesCaseSensitivityAsync()
        {
            var testCode = @"namespace First
{
    using Second;
    using second;
}

namespace Second { }
namespace second { }";

            var fixedTestCode = @"namespace First
{
    using second;
    using Second;
}

namespace Second { }
namespace second { }";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(4, 5);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            var fixedTestCode = @"using System.Threading;
using global::System;
using global::System.IO;
using global::System.Linq;
using XYZ = System.IO;

namespace Foo
{
    using System;
    using global::Foo;
}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithNamespaceAliasQualifierAsync()
        {
            var testCode = @"extern alias corlib;
using System.Threading;
using corlib::System;
using global::System.IO;
using global::System.Linq;
using global::System;
using global::Foo;
using Foo;
using Microsoft;

namespace Foo
{
    using global::Foo;
    using System;
}";

            var fixedTestCode = @"extern alias corlib;
using System.Threading;
using corlib::System;
using Foo;
using global::Foo;
using global::System;
using global::System.IO;
using global::System.Linq;
using Microsoft;

namespace Foo
{
    using System;
    using global::Foo;
}";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(5, 1),
                    Diagnostic().WithLocation(6, 1),
                    Diagnostic().WithLocation(7, 1),
                },
                FixedCode = fixedTestCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(namespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(1, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 5);
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
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#else
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#endif";

            var fixedTestCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;

#if true
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
#else
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#endif";

            // else block is skipped
            var expected = Diagnostic().WithLocation(7, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// This is a regression test for DotNetAnalyzers/StyleCopAnalyzers#1897.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInNamespaceDeclarationWithFileHeaderAsync()
        {
            var testCode = @"// <copyright file=""VoiceCommandService.cs"" company=""Foo Corporation"">
// Copyright (c) FooCorporation. All rights reserved.
// </copyright>

namespace Foo.Garage.XYZ
{
    using System;
    using Newtonsoft.Json;
    using Foo.Garage.XYZ;
}

namespace Newtonsoft.Json
{
}
";

            var fixedTestCode = @"// <copyright file=""VoiceCommandService.cs"" company=""Foo Corporation"">
// Copyright (c) FooCorporation. All rights reserved.
// </copyright>

namespace Foo.Garage.XYZ
{
    using System;
    using Foo.Garage.XYZ;
    using Newtonsoft.Json;
}

namespace Newtonsoft.Json
{
}
";

            // The same diagnostic is reported multiple times due to a bug in Roslyn 1.0
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(8, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the first using statement will preserve its leading comment.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestLeadingCommentForFirstUsingInNamespaceIsPreservedAsync()
        {
            var testCode = @"namespace TestNamespace
{
    // With test comment
    using System;
    using TestNamespace;
    using Newtonsoft.Json;
}

namespace Newtonsoft.Json
{
}
";

            var fixedTestCode = @"namespace TestNamespace
{
    // With test comment
    using System;
    using Newtonsoft.Json;
    using TestNamespace;
}

namespace Newtonsoft.Json
{
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(5, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
