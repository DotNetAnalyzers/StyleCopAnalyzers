// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp10.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.NamingRules.SA1305FieldNamesMustNotUseHungarianNotation>;

    public partial class SA1305CSharp11UnitTests : SA1305CSharp10UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestScopedRefLocalWithHungarianNotationAsync()
        {
            var testCode = @"
public class TestClass
{
    public void Bar()
    {
        int value = 5;
        scoped ref int {|#0:baR|} = ref value;
    }
}
";

            var expected = Diagnostic().WithArguments("variable", "baR").WithLocation(0);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
