// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp9.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp8.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1010OpeningSquareBracketsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1010CSharp9UnitTests : SA1010CSharp8UnitTests
    {
        [Fact]
        [WorkItem(3970, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3970")]
        public async Task TestFunctionPointerCallingConventionBracketsAsync()
        {
            var testCode = @"public class TestClass
{
    private unsafe delegate* unmanaged{|#0:[|} Cdecl]<void> field1;
    private unsafe delegate* unmanaged {|#1:[|}Cdecl]<void> field2;
}
";

            var fixedCode = @"public class TestClass
{
    private unsafe delegate* unmanaged[Cdecl]<void> field1;
    private unsafe delegate* unmanaged[Cdecl]<void> field2;
}
";

            var expected = new[]
            {
                Diagnostic(DescriptorNotFollowed).WithLocation(0),
                Diagnostic(DescriptorNotPreceded).WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
