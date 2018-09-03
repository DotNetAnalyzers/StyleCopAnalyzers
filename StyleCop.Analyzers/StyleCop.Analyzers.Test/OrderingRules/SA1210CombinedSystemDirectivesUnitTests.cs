// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.CustomDiagnosticVerifier<StyleCop.Analyzers.OrderingRules.SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>;

    /// <summary>
    /// Unit tests for <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/> for the special case
    /// where <see cref="OrderingSettings.SystemUsingDirectivesFirst"/> is <see langword="false"/>.
    /// </summary>
    public class SA1210CombinedSystemDirectivesUnitTests
    {
        [Fact]
        public async Task TestProperOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var namespaceDeclaration = @"namespace Food
{
    using System;
    using System.Threading;
}

namespace Bar
{
    using Food;
    using System;
}
";

            await VerifyCSharpDiagnosticAsync(namespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesInNamespaceDeclarationAsync()
        {
            var testCode = @"namespace Food
{
    using System.Threading;
    using System;
}

namespace Bar
{
    using Food;
    using Bar;
    using System.Threading;
    using System;
}";

            var fixedTestCode = @"namespace Food
{
    using System;
    using System.Threading;
}

namespace Bar
{
    using Bar;
    using Food;
    using System;
    using System.Threading;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(9, 5),
                Diagnostic().WithLocation(11, 5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, allowNewCompilerDiagnostics: false, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithGlobalKeywordAsync()
        {
            var testCode = @"using System.Threading;
using global::System.IO;
using global::System.Linq;
using global::System;
using XYZ = System.IO;

namespace Food
{
    using global::Food;
    using System;
}";

            var fixedTestCode = @"using global::System;
using global::System.IO;
using global::System.Linq;
using System.Threading;
using XYZ = System.IO;

namespace Food
{
    using global::Food;
    using System;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(3, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, allowNewCompilerDiagnostics: false, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidOrderedUsingDirectivesWithNamespaceAliasQualifierAsync()
        {
            var testCode = @"using System.Threading;
using global::System.IO;
using global::System.Linq;
using global::System;
using global::Food;
using Food;

namespace Food
{
    using global::Food;
    using System;
}";

            var fixedTestCode = @"using Food;
using global::Food;
using global::System;
using global::System.IO;
using global::System.Linq;
using System.Threading;

namespace Food
{
    using global::Food;
    using System;
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1, 1),
                Diagnostic().WithLocation(3, 1),
                Diagnostic().WithLocation(4, 1),
                Diagnostic().WithLocation(5, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, allowNewCompilerDiagnostics: true, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestValidOrderedUsingDirectivesWithStaticUsingDirectivesAsync()
        {
            var namespaceDeclaration = @"namespace Food
{
    using Food;
    using System;
    using static System.Uri;
    using static System.Math;
}";

            await VerifyCSharpDiagnosticAsync(namespaceDeclaration, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
using Microsoft.Win32;
using System;
using MyList = System.Collections.Generic.List<int>;

#if true
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
#else
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
#endif";

            // else block is skipped
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(2, 1),
                Diagnostic().WithLocation(7, 1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, allowNewCompilerDiagnostics: false, CancellationToken.None).ConfigureAwait(false);
        }

        private static Task VerifyCSharpDiagnosticAsync(string source, DiagnosticResult[] expected, CancellationToken cancellationToken)
            => VerifyCSharpFixAsync(source, expected, fixedSource: null, allowNewCompilerDiagnostics: false, cancellationToken);

        private static Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, bool allowNewCompilerDiagnostics, CancellationToken cancellationToken)
        {
            const string CombinedUsingDirectivesTestSettings = @"
{
  ""settings"": {
    ""orderingRules"": {
      ""systemUsingDirectivesFirst"": false
    }
  }
}
";

            var test = new StyleCopCodeFixVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace, UsingCodeFixProvider>.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = CombinedUsingDirectivesTestSettings,
                AllowNewCompilerDiagnostics = allowNewCompilerDiagnostics,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
