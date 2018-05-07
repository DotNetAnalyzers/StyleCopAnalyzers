// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    /// <remarks>
    /// The test cases can be found in the SA1500 subfolder.
    /// </remarks>
    public partial class SA1500UnitTests : CodeFixVerifier
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
                this.CSharpDiagnostic().WithLocation(6, 13),
                this.CSharpDiagnostic().WithLocation(7, 24),
                this.CSharpDiagnostic().WithLocation(9, 13),
                this.CSharpDiagnostic().WithLocation(9, 31),
                this.CSharpDiagnostic().WithLocation(11, 13),
                this.CSharpDiagnostic().WithLocation(11, 20),
                this.CSharpDiagnostic().WithLocation(14, 9),
                this.CSharpDiagnostic().WithLocation(14, 29),
                this.CSharpDiagnostic().WithLocation(15, 9),
                this.CSharpDiagnostic().WithLocation(15, 17),
                this.CSharpDiagnostic().WithLocation(16, 9),
                this.CSharpDiagnostic().WithLocation(16, 19),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1500BracesForMultiLineStatementsMustNotShareLine();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1500CodeFixProvider();
        }
    }
}
