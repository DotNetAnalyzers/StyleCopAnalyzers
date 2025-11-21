// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid namespace defined in this test.
        /// </summary>
        /// <remarks>
        /// <para>These are valid for SA1500 only, some will report other diagnostics.</para>
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceValidAsync()
        {
            var testCode = @"namespace ValidNamespace1
{
}

namespace ValidNamespace2
{
    using System;
}

namespace ValidNamespace3 { } /* Valid only for SA1500 */

namespace ValidNamespace4 { using System; }  /* Valid only for SA1500 */

namespace ValidNamespace5 /* Valid only for SA1500 */
{ using System; }  
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid namespace definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceInvalidAsync()
        {
            var testCode = @"namespace InvalidNamespace1 {
}

namespace InvalidNamespace2 {
    using System;
}

namespace InvalidNamespace3 {
    using System; }

namespace InvalidNamespace4 { using System;
}

namespace InvalidNamespace5
{
    using System; }

namespace InvalidNamespace6
{ using System;
}
";

            var fixedTestCode = @"namespace InvalidNamespace1
{
}

namespace InvalidNamespace2
{
    using System;
}

namespace InvalidNamespace3
{
    using System;
}

namespace InvalidNamespace4
{
    using System;
}

namespace InvalidNamespace5
{
    using System;
}

namespace InvalidNamespace6
{
    using System;
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                // InvalidNamespace1
                Diagnostic().WithLocation(1, 29),

                // InvalidNamespace2
                Diagnostic().WithLocation(4, 29),

                // InvalidNamespace3
                Diagnostic().WithLocation(8, 29),
                Diagnostic().WithLocation(9, 19),

                // InvalidNamespace4
                Diagnostic().WithLocation(11, 29),

                // InvalidNamespace5
                Diagnostic().WithLocation(16, 19),

                // InvalidNamespace6
                Diagnostic().WithLocation(19, 1),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an invalid namespace at the end of the source file will be handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNamespaceInvalidAtEndOfFileAsync()
        {
            var testCode = @"
namespace TestNamespace
{
  using System; }";

            var fixedTestCode = @"
namespace TestNamespace
{
  using System;
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(4, 17),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
