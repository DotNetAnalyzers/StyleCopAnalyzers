// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1206DeclarationKeywordsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1206CodeFixProvider>;

    public partial class SA1206CodeFixProviderCSharp8UnitTests : SA1206CodeFixProviderCSharp7UnitTests
    {
        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyKeywordReorderingInStructMethodAsync()
        {
            var testCode = @"
public struct S
{
    readonly {|#0:public|} void M()
    {
    }
}";

            var fixedCode = @"
public struct S
{
    public readonly void M()
    {
    }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "readonly");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyKeywordReorderingInStructPropertyAsync()
        {
            var testCode = @"
public struct S
{
    readonly {|#0:internal|} int Value { get; }
}";

            var fixedCode = @"
public struct S
{
    internal readonly int Value { get; }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("internal", "readonly");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyKeywordReorderingWithOtherModifiersAsync()
        {
            var testCode = @"
public struct S
{
    readonly override {|#0:public|} string ToString() => string.Empty;
}";

            var fixedCode = @"
public struct S
{
    public readonly override string ToString() => string.Empty;
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "override");
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task VerifyReadonlyKeywordReorderingWithAccessAndStaticAsync()
        {
            var testCode = @"
public struct S
{
    readonly {|#0:static|} {|#1:public|} int {|#2:M|}() => 0;
}";

            var fixedCode = @"
public struct S
{
    public static readonly int {|#2:M|}() => 0;
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments("static", "readonly"),
                    Diagnostic().WithLocation(1).WithArguments("public", "readonly"),

                    // /0/Test0.cs(4,32): error CS8657: Static member 'S.M()' cannot be marked 'readonly'.
                    DiagnosticResult.CompilerError("CS8657").WithLocation(2).WithArguments("S.M()"),
                },
            };

            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
