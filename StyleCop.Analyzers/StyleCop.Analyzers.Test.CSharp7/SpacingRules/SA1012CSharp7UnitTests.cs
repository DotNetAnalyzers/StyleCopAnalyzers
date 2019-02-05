// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1012OpeningBracesMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1012CSharp7UnitTests : SA1012UnitTests
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
            int* data1 = stackalloc int[] { 1 , 1 };
            int* data2 = stackalloc int[] {1 , 1 };
            int* data3 = stackalloc int[]{ 1 , 1 };
            int* data4 = stackalloc int[]{1 , 1 };
            int* data5 = stackalloc int[] {
1 , 1 };
            int* data6 = stackalloc int[]
            {1 , 1 };
            int* data7 = stackalloc int[]
            {
                1 , 1 };
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
            int* data4 = stackalloc int[] { 1 , 1 };
            int* data5 = stackalloc int[] {
1 , 1 };
            int* data6 = stackalloc int[]
            { 1 , 1 };
            int* data7 = stackalloc int[]
            {
                1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 43),
                Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(9, 42),
                Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(10, 42),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(10, 42),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(14, 13),
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
            int* data1 = stackalloc[] { 1 , 1 };
            int* data2 = stackalloc[] {1 , 1 };
            int* data3 = stackalloc[]{ 1 , 1 };
            int* data4 = stackalloc[]{1 , 1 };
            int* data5 = stackalloc[] {
1 , 1 };
            int* data6 = stackalloc[]
            {1 , 1 };
            int* data7 = stackalloc[]
            {
                1 , 1 };
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
            int* data1 = stackalloc[] { 1 , 1 };
            int* data2 = stackalloc[] { 1 , 1 };
            int* data3 = stackalloc[] { 1 , 1 };
            int* data4 = stackalloc[] { 1 , 1 };
            int* data5 = stackalloc[] {
1 , 1 };
            int* data6 = stackalloc[]
            { 1 , 1 };
            int* data7 = stackalloc[]
            {
                1 , 1 };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(8, 39),
                Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(9, 38),
                Diagnostic().WithArguments(string.Empty, "preceded").WithLocation(10, 38),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(10, 38),
                Diagnostic().WithArguments(string.Empty, "followed").WithLocation(14, 13),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
