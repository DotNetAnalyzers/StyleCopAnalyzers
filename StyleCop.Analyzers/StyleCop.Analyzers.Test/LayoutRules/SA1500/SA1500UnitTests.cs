// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    /// <remarks>
    /// <para>The test cases can be found in the SA1500 subfolder.</para>
    /// </remarks>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that a complex multiple fix scenario is handled correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2566, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2566")]
        public async Task VerifyFixAllAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public static void Sample()
    {
        try {
            if (false) {
                return;
            } else if (false) {
                return;
            } else {
                return;
            }
        } catch (Exception) {
        } catch {
        } finally {
        }
    }
}
";

            var fixedTestCode = @"using System;
public class TestClass
{
    public static void Sample()
    {
        try
        {
            if (false)
            {
                return;
            }
            else if (false)
            {
                return;
            }
            else
            {
                return;
            }
        }
        catch (Exception)
        {
        }
        catch
        {
        }
        finally
        {
        }
    }
}
";

            DiagnosticResult[] expectedDiagnostics =
            {
                Diagnostic().WithLocation(6, 13),
                Diagnostic().WithLocation(7, 24),
                Diagnostic().WithLocation(9, 13),
                Diagnostic().WithLocation(9, 31),
                Diagnostic().WithLocation(11, 13),
                Diagnostic().WithLocation(11, 20),
                Diagnostic().WithLocation(14, 9),
                Diagnostic().WithLocation(14, 29),
                Diagnostic().WithLocation(15, 9),
                Diagnostic().WithLocation(15, 17),
                Diagnostic().WithLocation(16, 9),
                Diagnostic().WithLocation(16, 19),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
