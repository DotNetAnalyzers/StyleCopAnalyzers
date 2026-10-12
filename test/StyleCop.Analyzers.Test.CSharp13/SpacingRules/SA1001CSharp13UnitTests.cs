// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp13UnitTests : SA1001CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4020, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4020")]
        public async Task TestNoSpaceAfterCommaBeforeAllowsRefStructConstraintAsync()
        {
            var testCode = @"
interface ISomeInterface
{
}

class Foo<T>
    where T : ISomeInterface{|#0:,|}allows ref struct
{
}";
            var fixedCode = @"
interface ISomeInterface
{
}

class Foo<T>
    where T : ISomeInterface, allows ref struct
{
}";

            var expected = Diagnostic().WithLocation(0).WithArguments(string.Empty, "followed");

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            };
            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
