// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.SpacingRules;
    using StyleCop.Analyzers.Test.CSharp13.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp13UnitTests : SA1000CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4020, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4020")]
        public async Task TestAllowsRefStructConstraintRefKeywordTrailingCommentAsync()
        {
            var testCode = @"
class Foo<T>
    where T : allows {|#0:ref|}/*comment*/struct
{
}";
            var fixedCode = @"
class Foo<T>
    where T : allows ref /*comment*/struct
{
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("ref", string.Empty);

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                ReferenceAssemblies = RuntimeReferenceAssemblies.Net90,
            };
            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
