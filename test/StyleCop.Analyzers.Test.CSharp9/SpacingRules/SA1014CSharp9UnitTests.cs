// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1014OpeningGenericBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1014CSharp9UnitTests : SA1014CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerOpeningBracketSpacingAsync()
        {
            var testCode = @"public class TestClass
{
    private unsafe delegate* {|#0:<|}int, void> field1;
    private unsafe delegate*{|#1:<|} int, void> field2;
    private unsafe delegate* unmanaged[Cdecl] {|#2:<|}int, void> field3;
}
";

            var fixedCode = @"public class TestClass
{
    private unsafe delegate*<int, void> field1;
    private unsafe delegate*<int, void> field2;
    private unsafe delegate* unmanaged[Cdecl]<int, void> field3;
}
";

            var expected = new[]
            {
                Diagnostic().WithArguments("preceded").WithLocation(0),
                Diagnostic().WithArguments("followed").WithLocation(1),
                Diagnostic().WithArguments("preceded").WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
