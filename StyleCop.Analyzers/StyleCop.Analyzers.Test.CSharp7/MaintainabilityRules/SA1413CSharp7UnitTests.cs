// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public class SA1413CSharp7UnitTests : SA1413UnitTests
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
            int* data1 = stackalloc int[] { 1, 1 };
            int* data2 = stackalloc int[] { 1, 1, };
            int* data3 = stackalloc int[]
            {
                1,
                1
            };
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
            int* data1 = stackalloc int[] { 1, 1 };
            int* data2 = stackalloc int[] { 1, 1, };
            int* data3 = stackalloc int[]
            {
                1,
                1,
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 17),
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
            int* data1 = stackalloc[] { 1, 1 };
            int* data2 = stackalloc[] { 1, 1, };
            int* data3 = stackalloc[]
            {
                1,
                1
            };
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
            int* data1 = stackalloc[] { 1, 1 };
            int* data2 = stackalloc[] { 1, 1, };
            int* data3 = stackalloc[]
            {
                1,
                1,
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(12, 17),
            };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
