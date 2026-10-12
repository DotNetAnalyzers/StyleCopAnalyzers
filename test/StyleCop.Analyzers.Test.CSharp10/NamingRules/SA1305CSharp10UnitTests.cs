// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.NamingRules.SA1305FieldNamesMustNotUseHungarianNotation>;

    public partial class SA1305CSharp10UnitTests : SA1305CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3990, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3990")]
        public async Task TestMixedDeconstructionHungarianNotationAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
        int value = 1;
        (value, int {|#0:iCount|}) = (2, 3);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0).WithArguments("variable", "iCount"),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
