// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.OrderingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1217UsingStaticDirectivesMustBeOrderedAlphabetically,
        StyleCop.Analyzers.OrderingRules.UsingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1217UsingStaticDirectivesMustBeOrderedAlphabetically"/>.
    /// </summary>
    public class SA1217UnitTests
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
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    using Execute = System.Action;
    using static System.Array;
    using static System.Math;
}

namespace Bar
{
    using System;
    using static System.Array;
    using Execute = System.Action;
    using static System.Math;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
using Execute = System.Action;
using static System.Math;

public class Foo
{
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
    using System;
    using Execute = System.Action;
    using static System.Math;
    using static System.Array;
}

namespace Bar
{
    using static System.Math;
    using Execute = System.Action;
    using static System.Array;
    using System;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using static System.Array;
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
                Diagnostic().WithLocation(5, 5).WithArguments("System.Math", "System.Array"),
                Diagnostic().WithLocation(11, 5).WithArguments("System.Math", "System.Array"),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will not produce a diagnostic when the using directives have inline comments.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidUsingDirectivesWithInlineCommentsAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static /* B */ System.Array;
    using static /* A */ System.Math;
}
";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will produce the proper diagnostics when one of the using directives has a global prefixes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidUsingDirectivesWithGlobalPrefixAsync()
        {
            var testCode = @"namespace Foo
{
    using System;
    using Execute = System.Action;
    using static System.Math;
    using static global::System.Array;
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using static global::System.Array;
    using static System.Math;
    using Execute = System.Action;
}
";

            var expectedDiagnostic = Diagnostic().WithLocation(5, 5).WithArguments("System.Math", "global::System.Array");

            await VerifyCSharpFixAsync(testCode, expectedDiagnostic, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle preprocessor directives.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPreprocessorDirectivesAsync()
        {
            var testCode = @"
using System;
using Microsoft.Win32;
using MyList = System.Collections.Generic.List<int>;
using static System.Tuple;

#if true
using static System.String;
using static System.Math;
#else
using static System.String;
using static System.Math;
#endif";

            var fixedTestCode = @"
using System;
using Microsoft.Win32;
using static System.Tuple;
using MyList = System.Collections.Generic.List<int>;

#if true
using static System.Math;
using static System.String;
#else
using static System.String;
using static System.Math;
#endif";

            // else block is skipped
            var expected = Diagnostic().WithLocation(8, 1).WithArguments("System.String", "System.Math");

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
