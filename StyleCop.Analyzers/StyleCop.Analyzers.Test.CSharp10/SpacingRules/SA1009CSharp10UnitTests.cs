// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1009ClosingParenthesisMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1009CSharp10UnitTests : SA1009CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestLambdaReturnTypeSpacingAsync()
        {
            var testCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int{|#0:)|}(int value) => (value, value);
    }
}
";

            var fixedCode = @"
class TestClass
{
    void M()
    {
        var projector = (int, int) (int value) => (value, value);
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ExpectedDiagnostics =
                {
                    Diagnostic(DescriptorFollowed).WithLocation(0),
                },
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionClosingParenthesisSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int value = 1;
        (value, int newValue {|#0:)|} = (2, 3);
    }
}";

            var fixedCode = @"public class TestClass
{
    public void TestMethod()
    {
        int value = 1;
        (value, int newValue) = (2, 3);
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic(DescriptorNotPreceded).WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task TestLineSpanDirectiveAsync()
        {
            // #line span directives are primarily used in generated code, so their parentheses are not checked
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
#line (1, 1) - (5, 60) 10 ""file.cs""
        int x = 0;
#line ( 2, 3 )-(4,5 ) ""file.cs""
        int y = 0;
#line (3, 4)  -  ( 5, 6) 10 ""file.cs""
        int z = 0;
#line default
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
