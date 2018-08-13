// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
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
        /// Verifies that no diagnostics are reported for the valid block defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestBlockValidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar()
    {
        // valid block #1
        {
        }

        // valid block #2
        {
            Debug.Indent();
        }

        // valid block #3 (valid only for SA1500)
        { }

        // valid block #4 (valid only for SA1500)
        { Debug.Indent(); }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies diagnostics and codefixes for all invalid blocks.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestBlockInvalidAsync()
        {
            var testCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar()
    {
        // invalid block #1
        { Debug.Indent();
        }

        // invalid block #2
        {
            Debug.Indent(); }
    }
}";

            var fixedTestCode = @"using System.Diagnostics;

public class Foo
{
    public void Bar()
    {
        // invalid block #1
        {
            Debug.Indent();
        }

        // invalid block #2
        {
            Debug.Indent();
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(8, 9),
                Diagnostic().WithLocation(13, 29),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
