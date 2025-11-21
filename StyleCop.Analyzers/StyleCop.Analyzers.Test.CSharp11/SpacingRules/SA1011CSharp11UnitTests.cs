// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp11.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp10.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1011ClosingSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1011CSharp11UnitTests : SA1011CSharp10UnitTests
    {
        [Fact]
        [WorkItem(3673, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3673")]
        public async Task TestListPatternInSwitchCaseAsync()
        {
            var testCode = @"public class TestClass
{
    public void TestMethod(object[] arg)
    {
        switch (arg)
        {
            case [string s{|#0:]|} :
                break;
        }
    }}
";

            var fixedCode = @"public class TestClass
{
    public void TestMethod(object[] arg)
    {
        switch (arg)
        {
            case [string s]:
                break;
        }
    }}
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net70,
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(0).WithArguments(" not", "followed"),
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
