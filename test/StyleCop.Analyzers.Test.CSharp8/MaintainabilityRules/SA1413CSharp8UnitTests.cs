// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public partial class SA1413CSharp8UnitTests : SA1413CSharp7UnitTests
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

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3003, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3003")]
        public async Task TestPropertyPatternAsync()
        {
            var testCode = @"
public class TestClass
{
    public bool Test1(SomeType value) => value is SomeType { X: 1, Y: 2 };
    public bool Test2(SomeType value) => value is SomeType { X: 1, Y: 2, };
    public bool Test3(SomeType value) => value is SomeType
    {
        X: 1,
        [|Y: 2|]
    };
}

public class SomeType
{
    public int X { get; set; }

    public int Y { get; set; }
}
";

            var fixedCode = @"
public class TestClass
{
    public bool Test1(SomeType value) => value is SomeType { X: 1, Y: 2 };
    public bool Test2(SomeType value) => value is SomeType { X: 1, Y: 2, };
    public bool Test3(SomeType value) => value is SomeType
    {
        X: 1,
        Y: 2,
    };
}

public class SomeType
{
    public int X { get; set; }

    public int Y { get; set; }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
