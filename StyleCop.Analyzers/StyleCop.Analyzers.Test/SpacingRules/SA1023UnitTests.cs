// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1023UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid dereference and access of symbols.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    unsafe void TestMethod()
    {
        int a = 1;
        int* x = &a;
        *x += 1;
        int*[] y;
        y = new int*[5];
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle invalid dereference and access of symbols.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestInvalidSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    unsafe void TestMethod()
    {
        int a = 1;
        int*x = &a;
        *x += 1;
        int * [] y;
        y = new int * [5];
        int
* z = x;
        a = *
x;
    }
}
";

            var fixedCode = @"public class TestClass
{
    unsafe void TestMethod()
    {
        int a = 1;
        int* x = &a;
        *x += 1;
        int*[] y;
        y = new int*[5];
        int* z = x;
        a = *x;
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(6, 12).WithArguments("be followed by a space"),
                this.CSharpDiagnostic().WithLocation(8, 13).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(8, 13).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(9, 21).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(11, 0).WithArguments("not appear at the beginning of a line"),
                this.CSharpDiagnostic().WithLocation(12, 13).WithArguments("not appear at the end of a line")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening parentheses.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestOpeningParenthesisNotReportedAsync()
        {
            var testCode = @"unsafe public class TestClass
{
    public void TestMethod1(int a)
    {
    }

    public void TestMethod2()
    {
        int x = 1;
        int* y = &x;
        TestMethod1(*y);
    }
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new OpenCloseSpacingCodeFixProvider();
        }
    }
}
