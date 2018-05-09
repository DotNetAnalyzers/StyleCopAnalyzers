// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace"/> for the special case
    /// where <see cref="OrderingSettings.SystemUsingDirectivesFirst"/> is <see langword="false"/>.
    /// </summary>
    public class SA1210CombinedSystemDirectivesUnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(9, 5),
                this.CSharpDiagnostic().WithLocation(11, 5),
            };

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
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
            };

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
                this.CSharpDiagnostic().WithLocation(1, 1),
                this.CSharpDiagnostic().WithLocation(3, 1),
                this.CSharpDiagnostic().WithLocation(4, 1),
                this.CSharpDiagnostic().WithLocation(5, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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

            await this.VerifyCSharpDiagnosticAsync(namespaceDeclaration, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(2, 1),
                this.CSharpDiagnostic().WithLocation(7, 1),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
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

            return CombinedUsingDirectivesTestSettings;
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDisabledDiagnostics()
        {
            // Using directive appeared previously in this namespace
            yield return "CS0105";
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
