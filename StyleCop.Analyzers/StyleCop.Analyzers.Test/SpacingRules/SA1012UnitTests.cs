// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1012OpeningCurlyBracketsMustBeSpacedCorrectly"/>
    /// </summary>
    public class SA1012UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that the analyzer will properly handle valid opening curly brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestValidCurlyBracketSpacingAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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

            DiagnosticResult expected = new DiagnosticResult
            {
                Id = "CS1513",
                Message = "} expected",
                Severity = DiagnosticSeverity.Error,
            };

            expected = expected.WithLocation(2, 2);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening curly brackets in string interpolation.
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
                this.CSharpDiagnostic().WithLocation(12, 19).WithArguments(" not", "followed"),
                this.CSharpDiagnostic().WithLocation(13, 20).WithArguments(" not", "followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening curly brackets in property declaration.
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
                this.CSharpDiagnostic().WithLocation(6, 33).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(7, 34).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(8, 33).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(8, 33).WithArguments(string.Empty, "followed"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the analyzer will properly handle opening curly brackets in nested curly brackets.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNestedCurlyBracketsAsync()
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
                this.CSharpDiagnostic().WithLocation(10, 37).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(11, 38).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(11, 39).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(12, 40).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(13, 37).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(13, 37).WithArguments(string.Empty, "followed"),
                this.CSharpDiagnostic().WithLocation(13, 38).WithArguments(string.Empty, "preceded"),
                this.CSharpDiagnostic().WithLocation(13, 38).WithArguments(string.Empty, "followed")
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode).ConfigureAwait(false);
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
                new DiagnosticResult
                {
                    Id = "CS1514",
                    Severity = DiagnosticSeverity.Error,
                    Message = "{ expected",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 25) }
                }
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1012OpeningCurlyBracketsMustBeSpacedCorrectly();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TokenSpacingCodeFixProvider();
        }
    }
}
