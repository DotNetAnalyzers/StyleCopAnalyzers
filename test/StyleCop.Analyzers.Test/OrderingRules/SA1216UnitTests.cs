// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation"/>.
    /// </summary>
    public class SA1216UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when the using directives are ordered wrong.
        /// </summary>
        /// <param name="lineEnding">The line ending to use in the test code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestInvalidUsingDirectivesOrderingAsync(string lineEnding)
        {
            var testCode = @"namespace Foo
{
    {|#0:using static System.Math;|}
    using Execute = System.Action;
    using System;
}

namespace Bar
{
    using Execute = System.Action;
    {|#1:using static System.Array;|}
    using static System.Math;
    using System;
}
".ReplaceLineEndings(lineEnding);

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
".ReplaceLineEndings(lineEnding);

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.Win32;
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

            var fixedTestCode = @"
using System;
using Microsoft.Win32;
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
            var expected = Diagnostic().WithLocation(9, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
