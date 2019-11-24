// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public class SA1413CSharp8UnitTests : SA1413CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3056, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3056")]
        public async Task TestSwitchExpressionAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int input)
        {
            int data1 = input switch { 0 => 1, 1 => 2 };
            int data2 = input switch { 0 => 1, 1 => 2, };
            int data3 = input switch
            {
                0 => 1,
                [|1 => 2|]
            };
        }
    }
}
";

            var fixedCode = @"namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int input)
        {
            int data1 = input switch { 0 => 1, 1 => 2 };
            int data2 = input switch { 0 => 1, 1 => 2, };
            int data3 = input switch
            {
                0 => 1,
                1 => 2,
            };
        }
    }
}
";

            await VerifyCSharpFixAsync(LanguageVersion.CSharp8, testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
