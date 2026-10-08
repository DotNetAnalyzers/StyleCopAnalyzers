// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1000KeywordsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1000CSharp14UnitTests : SA1000CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4005, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4005")]
        public async Task TestSimpleLambdaParameterWithScopedModifierAsync()
        {
            var testCode = @"
public delegate void ScopedSpanAction(scoped System.Span<int> value);

public class TestClass
{
    public void Method()
    {
        ScopedSpanAction action = ({|#0:scoped|}@x) => { };
    }
}
";

            var fixedCode = @"
public delegate void ScopedSpanAction(scoped System.Span<int> value);

public class TestClass
{
    public void Method()
    {
        ScopedSpanAction action = (scoped @x) => { };
    }
}
";

            var expected = Diagnostic().WithArguments("scoped", string.Empty, "followed").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
