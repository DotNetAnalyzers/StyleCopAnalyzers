﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1010CSharp7UnitTests : SA1010UnitTests
    {
        [Fact]
        public async Task TestStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int [ ] { 1 , 1 };
            int* data2 = stackalloc int [] { 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1 };
            int* data4 = stackalloc int[
] { 1 , 1 };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc int[] { 1 , 1 };
            int* data2 = stackalloc int[] { 1 , 1 };
            int* data3 = stackalloc int[] { 1 , 1 };
            int* data4 = stackalloc int[
] { 1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(7, 41),
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 41),
                Diagnostic(DescriptorNotPreceded).WithLocation(8, 41),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc [ ] { 1 , 1 };
            int* data2 = stackalloc [] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 };
            int* data4 = stackalloc[
] { 1 , 1 };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public unsafe void TestMethod()
        {
            int* data1 = stackalloc [] { 1 , 1 };
            int* data2 = stackalloc [] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 };
            int* data4 = stackalloc[
] { 1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(7, 37),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
