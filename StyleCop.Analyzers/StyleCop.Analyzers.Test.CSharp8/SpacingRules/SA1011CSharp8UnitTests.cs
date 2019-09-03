// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1011CSharp8UnitTests : SA1011CSharp7UnitTests
    {
        /// <summary>
        /// Verify that declaring a null reference type works for arrays.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(2927, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2927")]
        public async Task VerifyNullableContextWithArraysAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            byte[]? data = null;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(2900, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2900")]
        public async Task VerifyNullableContextWithArrayReturnsAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public byte[]? TestMethod()
        {
            return null;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
