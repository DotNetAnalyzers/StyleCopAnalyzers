// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
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
        /// Verifies that no diagnostics are reported for the valid try ... catch ... finally statements defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics from the layout (SA15xx) series.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTryCatchFinallyValidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Valid try ... catch ... finally #1
        try
        {
        }
        catch (Exception)
        {
        }
        finally
        {
        }

        // Valid try ... catch ... finally #2
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }

        // Valid try ... catch ... finally #3 (Valid only for SA1500)
        try { } catch (Exception) { }

        // Valid try ... catch ... finally #4 (Valid only for SA1500)
        try { x += 1; } catch (Exception) { x += 2; }

        // Valid try ... catch ... finally #5 (Valid only for SA1500)
        try { } finally { }

        // Valid try ... catch ... finally #6 (Valid only for SA1500)
        try { x += 1; } finally { x += 3; }

        // Valid try ... catch ... finally #7 (Valid only for SA1500)
        try { x += 1; } catch (Exception) { x += 2; } finally { x += 3; }

        // Valid try ... catch ... finally #8 (Valid only for SA1500)
        try { x += 1; } catch (Exception) { x += 2; } finally { x += 3; }

        // Valid try ... catch ... finally #9 (Valid only for SA1500)
        try 
        { x += 1; } 
        catch (Exception) 
        { x += 2; } 
        finally 
        { x += 3; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid try ... catch ... finally statement definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestTryCatchFinallyInvalidAsync()
        {
            var testCode = @"using System;

public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Invalid try ... catch ... finally #1
        try {
        }
        catch (Exception) {
        }
        finally {
        }

        // Invalid try ... catch ... finally #2
        try {
            x += 1;
        }
        catch (Exception) {
            x += 2;
        }
        finally {
            x += 3;
        }

        // Invalid try ... catch ... finally #3 (Valid only for SA1500)
        try {
            x += 1; }
        catch (Exception) {
            x += 2; }
        finally {
            x += 3; }

        // Invalid try ... catch ... finally #4
        try { x += 1;
        }
        catch (Exception) { x += 2;
        }
        finally { x += 3;
        }

        // Invalid try ... catch ... finally #5 (Valid only for SA1500)
        try
        {
            x += 1; }
        catch (Exception)
        {
            x += 2; }
        finally
        {
            x += 3; }

        // Invalid try ... catch ... finally #6 (Valid only for SA1500)
        try
        { x += 1;
        }
        catch (Exception)
        { x += 2;
        }
        finally
        { x += 3;
        }
    }
}";

            var fixedTestCode = @"using System;

public class Foo
{
    private void Bar()
    {
        var x = 0;

        // Invalid try ... catch ... finally #1
        try
        {
        }
        catch (Exception)
        {
        }
        finally
        {
        }

        // Invalid try ... catch ... finally #2
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }

        // Invalid try ... catch ... finally #3 (Valid only for SA1500)
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }

        // Invalid try ... catch ... finally #4
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }

        // Invalid try ... catch ... finally #5 (Valid only for SA1500)
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }

        // Invalid try ... catch ... finally #6 (Valid only for SA1500)
        try
        {
            x += 1;
        }
        catch (Exception)
        {
            x += 2;
        }
        finally
        {
            x += 3;
        }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid try ... catch ... finally #1
                Diagnostic().WithLocation(10, 13),
                Diagnostic().WithLocation(12, 27),
                Diagnostic().WithLocation(14, 17),

                // Invalid try ... catch ... finally #2
                Diagnostic().WithLocation(18, 13),
                Diagnostic().WithLocation(21, 27),
                Diagnostic().WithLocation(24, 17),

                // Invalid try ... catch ... finally #3
                Diagnostic().WithLocation(29, 13),
                Diagnostic().WithLocation(30, 21),
                Diagnostic().WithLocation(31, 27),
                Diagnostic().WithLocation(32, 21),
                Diagnostic().WithLocation(33, 17),
                Diagnostic().WithLocation(34, 21),

                // Invalid try ... catch ... finally #4
                Diagnostic().WithLocation(37, 13),
                Diagnostic().WithLocation(39, 27),
                Diagnostic().WithLocation(41, 17),

                // Invalid try ... catch ... finally #5
                Diagnostic().WithLocation(47, 21),
                Diagnostic().WithLocation(50, 21),
                Diagnostic().WithLocation(53, 21),

                // Invalid try ... catch ... finally #6
                Diagnostic().WithLocation(57, 9),
                Diagnostic().WithLocation(60, 9),
                Diagnostic().WithLocation(63, 9),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
