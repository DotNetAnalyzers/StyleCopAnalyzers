// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1019MemberAccessSymbolsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1019CSharp10UnitTests : SA1019CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3984, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3984")]
        public async Task TestExtendedPropertyPatternAsync()
        {
            var testCode = @"public class Foo
{
    public Foo Inner { get; }

    public int Value { get; }

    public bool TestMethod(Foo value)
    {
        return value is { Inner {|#0:.|}Value: 1 }
            || value is { Inner{|#1:.|} Value: 1 };
    }
}";

            var fixedCode = @"public class Foo
{
    public Foo Inner { get; }

    public int Value { get; }

    public bool TestMethod(Foo value)
    {
        return value is { Inner.Value: 1 }
            || value is { Inner.Value: 1 };
    }
}";

            var expected = new[]
            {
                Diagnostic(DescriptorNotPreceded).WithLocation(0).WithArguments("."),
                Diagnostic(DescriptorNotFollowed).WithLocation(1).WithArguments("."),
            };
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
