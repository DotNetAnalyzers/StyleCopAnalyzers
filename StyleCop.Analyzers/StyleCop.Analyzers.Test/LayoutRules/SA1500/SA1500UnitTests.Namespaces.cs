// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid namespace defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(1, 29),

                // InvalidNamespace2
                this.CSharpDiagnostic().WithLocation(4, 29),

                // InvalidNamespace3
                this.CSharpDiagnostic().WithLocation(8, 29),
                this.CSharpDiagnostic().WithLocation(9, 19),

                // InvalidNamespace4
                this.CSharpDiagnostic().WithLocation(11, 29),

                // InvalidNamespace5
                this.CSharpDiagnostic().WithLocation(16, 19),

                // InvalidNamespace6
                this.CSharpDiagnostic().WithLocation(19, 1)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
