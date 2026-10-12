// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp12.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp11.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public partial class SA1413CSharp12UnitTests : SA1413CSharp11UnitTests
    {
        [Theory]
        [InlineData("1, 2")]
        [InlineData("1, 2,")]
        [WorkItem(4007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4007")]
        public async Task VerifySingleLineCollectionExpressionAsync(string elements)
        {
            var testCode = $@"
namespace TestNamespace
{{
    public class TestClass
    {{
        private int[] values = [ {elements} ];
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4007")]
        public async Task VerifyMultiLineCollectionExpressionAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        private int[] values =
        [
            1,
            [|2|]
        ];
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        private int[] values =
        [
            1,
            2,
        ];
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4007, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4007")]
        public async Task VerifyMultiLineCollectionExpressionWithSpreadAndNestedElementsAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int[] other)
        {
            int[] values =
            [
                1,
                [|.. other|]
            ];

            Use(
            [
                [1, 2],
                [
                    3,
                    [|4|]
                ],
            ]);

            int[] empty =
            [
            ];
        }

        private static void Use(int[][] value)
        {
        }
    }
}
";

            var fixedCode = @"
namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod(int[] other)
        {
            int[] values =
            [
                1,
                .. other,
            ];

            Use(
            [
                [1, 2],
                [
                    3,
                    4,
                ],
            ]);

            int[] empty =
            [
            ];
        }

        private static void Use(int[][] value)
        {
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
