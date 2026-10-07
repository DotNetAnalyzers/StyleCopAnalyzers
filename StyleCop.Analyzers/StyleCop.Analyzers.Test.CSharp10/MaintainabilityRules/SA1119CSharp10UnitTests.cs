// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.MaintainabilityRules.SA1119StatementMustNotUseUnnecessaryParenthesis,
        StyleCop.Analyzers.MaintainabilityRules.SA1119CodeFixProvider>;

    public partial class SA1119CSharp10UnitTests : SA1119CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionWithUnnecessaryParenthesesAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int a = 1;
        (a, int c) = ((2, 3));
    }
}";

            var fixedCode = @"public class TestClass
{
    public void TestMethod()
    {
        int a = 1;
        (a, int c) = (2, 3);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic(DiagnosticId).WithSpan(6, 22, 6, 30),
                Diagnostic(ParenthesesDiagnosticId).WithSpan(6, 22, 6, 23),
                Diagnostic(ParenthesesDiagnosticId).WithSpan(6, 29, 6, 30),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
