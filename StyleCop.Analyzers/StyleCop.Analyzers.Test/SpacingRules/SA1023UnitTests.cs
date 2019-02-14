// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1023DereferenceAndAccessOfSymbolsMustBeSpacedCorrectly,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                Diagnostic(DescriptorFollowed).WithLocation(6, 12),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 13),
                Diagnostic(DescriptorNotFollowed).WithLocation(8, 13),
                Diagnostic(DescriptorNotPreceded).WithLocation(9, 21),
                Diagnostic(DescriptorNotFollowed).WithLocation(9, 21),
                Diagnostic(DescriptorNotAtBeginningOfLine).WithLocation(11, 1),
                Diagnostic(DescriptorNotAtEndOfLine).WithLocation(12, 13),
                Diagnostic(DescriptorNotPreceded).WithLocation(16, 18),
                Diagnostic(DescriptorNotFollowed).WithLocation(16, 18),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 13),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 19),
                Diagnostic(DescriptorNotPreceded).WithLocation(17, 21),
                Diagnostic(DescriptorNotFollowed).WithLocation(17, 21),
                Diagnostic(DescriptorNotPreceded).WithLocation(18, 29),
                Diagnostic(DescriptorNotFollowed).WithLocation(19, 24),
                Diagnostic(DescriptorNotPreceded).WithLocation(19, 26),
                Diagnostic(DescriptorNotFollowed).WithLocation(20, 30),
                Diagnostic(DescriptorNotFollowed).WithLocation(21, 31),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 13),
                Diagnostic(DescriptorNotFollowed).WithLocation(22, 17),
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
