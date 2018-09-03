// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1012OpeningBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1012OpeningBracesMustBeSpacedCorrectly"/>.
    /// </summary>
    public class SA1012UnitTests
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidBraceSpacingAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var x = new { };
        }

        public int TestProperty { get; set; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle end of file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestHandlesEndOfFileAsync()
        {
            var testCode = @"namespace TestNamespace
{";

            DiagnosticResult expected = DiagnosticResult.CompilerError("CS1513").WithLocation(2, 2).WithMessage("} expected");
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening braces in string interpolation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestStringInterpolationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test = 2;
            var x = $""{test}"";
            x = $"" {test}"";
            x = $""({test})"";
            x = $""( {test})"";
            x = $""{ test}"";
            x = $"" { test}"";
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            var test = 2;
            var x = $""{test}"";
            x = $"" {test}"";
            x = $""({test})"";
            x = $""( {test})"";
            x = $""{test}"";
            x = $"" {test}"";
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 19).WithArguments(" not", "followed"),
                Diagnostic().WithLocation(13, 20).WithArguments(" not", "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening braces in property declaration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestPropertyDeclarationAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get; set; }
        public int TestProperty2{ get; set; }
        public int TestProperty3 {get; set; }
        public int TestProperty4{get; set; }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public int TestProperty1 { get; set; }
        public int TestProperty2 { get; set; }
        public int TestProperty3 { get; set; }
        public int TestProperty4 { get; set; }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 33).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(7, 34).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(8, 33).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(8, 33).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening braces in nested braces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNestedBracesAsync()
        {
            var testCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int>{ { 1, 1 } };
            new Dictionary<int, int> {{ 1, 1 } };
            new Dictionary<int, int> { {1, 1 } };
            new Dictionary<int, int>{{1, 1 } };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    using System.Collections.Generic;

    public class TestClass
    {
        public void TestMethod()
        {
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
            new Dictionary<int, int> { { 1, 1 } };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(10, 37).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(11, 38).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(11, 39).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(12, 40).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(13, 37).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(13, 37).WithArguments(string.Empty, "followed"),
                Diagnostic().WithLocation(13, 38).WithArguments(string.Empty, "preceded"),
                Diagnostic().WithLocation(13, 38).WithArguments(string.Empty, "followed"),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMissingTokenAsync()
        {
            string testCode = @"
class ClassName
{
    void Method()
    {
        int[] x = new[] 0 };
    }
}
";

            DiagnosticResult[] expected =
            {
                DiagnosticResult.CompilerError("CS1514").WithMessage("{ expected").WithLocation(6, 25),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
