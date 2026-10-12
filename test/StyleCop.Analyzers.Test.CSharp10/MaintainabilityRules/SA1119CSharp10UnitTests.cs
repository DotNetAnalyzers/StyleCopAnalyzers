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
        (a, int c) = {|#0:{|#1:(|}(2, 3){|#2:)|}|};
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
                Diagnostic(DiagnosticId).WithLocation(0),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(1),
                Diagnostic(ParenthesesDiagnosticId).WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
