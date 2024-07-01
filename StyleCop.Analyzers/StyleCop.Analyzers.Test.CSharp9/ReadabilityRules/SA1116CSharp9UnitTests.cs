// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public partial class SA1116CSharp9UnitTests : SA1116CSharp8UnitTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestTargetTypedNewExpressionnAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public Foo(int a, int b)
    {
    }

    public void Method()
    {
        Foo x = new(1,
            2);
    }
}";

            var fixedCode = @"
class Foo
{
    public Foo(int a, int b)
    {
    }

    public void Method()
    {
        Foo x = new(
            1,
            2);
    }
}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(10, 21) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
