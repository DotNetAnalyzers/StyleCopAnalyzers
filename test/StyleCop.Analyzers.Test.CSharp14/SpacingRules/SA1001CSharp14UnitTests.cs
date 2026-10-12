// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1001CommasMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1001CSharp14UnitTests : SA1001CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4027, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4027")]
        public async Task TestSimpleLambdaParametersWithRefAndOutModifiersAsync()
        {
            var testCode = @"
public delegate void RefOutAction(int a, out int b);

public class TestClass
{
    public void Method()
    {
        RefOutAction action = (a{|#0:,|}out b) => { b = 2; };
    }
}
";

            var fixedCode = @"
public delegate void RefOutAction(int a, out int b);

public class TestClass
{
    public void Method()
    {
        RefOutAction action = (a, out b) => { b = 2; };
    }
}
";

            var expected = Diagnostic().WithArguments(string.Empty, "followed").WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
