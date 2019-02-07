// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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
                Diagnostic().WithLocation(3, 5),
                Diagnostic().WithLocation(11, 5),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
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

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode).ConfigureAwait(false);
        }
    }
}
