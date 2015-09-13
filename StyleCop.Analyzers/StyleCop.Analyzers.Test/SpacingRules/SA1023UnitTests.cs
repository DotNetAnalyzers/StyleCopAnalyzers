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

        // The following cases are all regression tests for DotNetAnalyzers/StyleCopAnalyzers#1457
        x = (int*)null;
        x = *(int**)null;
        var t = typeof(char*);
        t = typeof(char**);
        var d1 = default(char*);
        var d2 = default(char**);
        a = *x * *x;
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

        // The following cases are all regression tests for DotNetAnalyzers/StyleCopAnalyzers#1457
        x = (int * )null;
        x = * (int* * )null;
        var t = typeof(char *);
        t = typeof(char* *);
        var d1 = default(char* );
        var d2 = default(char** );
        a = * x** x;
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

        // The following cases are all regression tests for DotNetAnalyzers/StyleCopAnalyzers#1457
        x = (int*)null;
        x = *(int**)null;
        var t = typeof(char*);
        t = typeof(char**);
        var d1 = default(char*);
        var d2 = default(char**);
        a = *x**x;
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
                this.CSharpDiagnostic().WithLocation(12, 13).WithArguments("not appear at the end of a line"),
                this.CSharpDiagnostic().WithLocation(16, 18).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(16, 18).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(17, 13).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(17, 19).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(17, 21).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(17, 21).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(18, 29).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(19, 24).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(19, 26).WithArguments("not be preceded by a space"),
                this.CSharpDiagnostic().WithLocation(20, 30).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(21, 31).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(22, 13).WithArguments("not be followed by a space"),
                this.CSharpDiagnostic().WithLocation(22, 17).WithArguments("not be followed by a space"),
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
            return new TokenSpacingCodeFixProvider();
        }
    }
}
