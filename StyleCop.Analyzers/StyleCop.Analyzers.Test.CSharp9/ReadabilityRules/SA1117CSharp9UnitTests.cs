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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public partial class SA1117CSharp9UnitTests : SA1117CSharp8UnitTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestValidTargetTypedNewExpressionAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public Foo(int a, int b, int c)
    {
    }

    public void Method()
    {
        Foo x = new(
            1,
            2,
            3);
    }
}";

            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInvalidTargetTypedNewExpressionAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public Foo(int a, int b, int c)
    {
    }

    public void Method()
    {
        Foo x = new(1,
            2, 3);
    }
}";

            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(11, 16) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
