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
    /// Unit tests for <see cref="SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation"/>.
    /// </summary>
    public class SA1216UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInNamespaceAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using static System.Math;
    using Execute = System.Action;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives inside a namespace.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInMultipleNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using static System.Math;
    using Execute = System.Action;
}

namespace Bar
{
    using System;
    using static System.Array;
    using static System.Math;
    using Execute = System.Action;
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce diagnostics for correctly ordered using directives in the compilation unit.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesInCompilationUnitAsync()
        {
            var testCode = @"using System;
using static System.Array;
using static System.Math;
using Execute = System.Action;

public class Foo
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the using directives are ordered wrong.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingDirectivesOrderingAsync()
        {
            var testCode = @"namespace Foo
{
    using static System.Math;
    using Execute = System.Action;
    using System;
}

namespace Bar
{
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
    using System;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using static System.Math;
    using Execute = System.Action;
}

namespace Bar
{
    using System;
    using static System.Array;
    using static System.Math;
    using Execute = System.Action;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
                this.CSharpDiagnostic().WithLocation(11, 5)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.VisualStudio;
using static System.String;
using MyList = System.Collections.Generic.List<int>;

#if true
using System.Threading;
using static System.Math;
using System.Threading.Tasks;
#else
using System.Threading;
using static System.Math;
using System.Threading.Tasks;
#endif";

            var fixedTestCode = @"using System;
using Microsoft.VisualStudio;
using static System.String;
using MyList = System.Collections.Generic.List<int>;

#if true
using System.Threading;
using System.Threading.Tasks;
using static System.Math;
#else
using System.Threading;
using static System.Math;
using System.Threading.Tasks;
#endif";

            // else block is skipped
            var expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
