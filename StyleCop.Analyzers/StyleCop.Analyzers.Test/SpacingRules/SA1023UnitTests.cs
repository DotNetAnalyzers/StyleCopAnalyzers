// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1023UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic().WithLocation(6, 12).WithArguments("be followed by a space"),
                Diagnostic().WithLocation(8, 13).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(8, 13).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(9, 21).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(9, 21).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(11, 0).WithArguments("not appear at the beginning of a line"),
                Diagnostic().WithLocation(12, 13).WithArguments("not appear at the end of a line"),
                Diagnostic().WithLocation(16, 18).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(16, 18).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(17, 13).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(17, 19).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(17, 21).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(17, 21).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(18, 29).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(19, 24).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(19, 26).WithArguments("not be preceded by a space"),
                Diagnostic().WithLocation(20, 30).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(21, 31).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(22, 13).WithArguments("not be followed by a space"),
                Diagnostic().WithLocation(22, 17).WithArguments("not be followed by a space"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
