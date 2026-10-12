// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1024ColonsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1024CSharp13UnitTests : SA1024CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4020, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4020")]
        public async Task TestAllowsRefStructConstraintColonMissingSpaceBeforeAsync()
        {
            var testCode = @"
class Foo<T>
    where T{|#0::|}allows ref struct
{
}";
            var fixedCode = @"
class Foo<T>
    where T : allows ref struct
{
}";

            var expected = new[]
            {
                Diagnostic(DescriptorPreceded).WithLocation(0),
                Diagnostic(DescriptorFollowed).WithLocation(0),
            };

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            };
            test.ExpectedDiagnostics.AddRange(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
