// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1413UseTrailingCommasInMultiLineInitializers,
        StyleCop.Analyzers.MaintainabilityRules.SA1413CodeFixProvider>;

    public partial class SA1413CSharp9UnitTests : SA1413CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3240, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3240")]
        public async Task VerifyWithInitializerAsync()
        {
            var testCode = @"namespace TestNamespace
{
    public record R
    {
        public int A { get; set; }

        public int B { get; set; }

        void M()
        {
            _ = this with { A = 1, B = 2 };
            _ = this with { A = 1, B = 2, };
            _ = this with
            {
                A = 1,
                [|B = 2|]
            };
        }
    }
}
";
            var fixedCode = @"namespace TestNamespace
{
    public record R
    {
        public int A { get; set; }

        public int B { get; set; }

        void M()
        {
            _ = this with { A = 1, B = 2 };
            _ = this with { A = 1, B = 2, };
            _ = this with
            {
                A = 1,
                B = 2,
            };
        }
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3240, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3240")]
        public async Task VerifyWithInitializerNestedDiagnosticsAsync()
        {
            var testCode = @"public record R1(int X, R2 r2);
public record R2(int Y);

public class C
{
    public void M(R1 r1, R2 r2)
    {
        _ = r1 with
        {
            X = 0,
            [|r2 = r2 with
            {
                [|Y = 0|]
            }|]
        };
    }
}
";
            var fixedCode = @"public record R1(int X, R2 r2);
public record R2(int Y);

public class C
{
    public void M(R1 r1, R2 r2)
    {
        _ = r1 with
        {
            X = 0,
            r2 = r2 with
            {
                Y = 0,
            },
        };
    }
}
";

            await new CSharpTest()
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
