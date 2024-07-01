// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public partial class SA1117CSharp7UnitTests : SA1117UnitTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestValidLocalFunctionsAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public void Method()
    {
        void LocalFunction1(
            int a, int b, string s) { }

        void LocalFunction2(
            int a,
            int b,
            string s) { }

        object LocalFunction3(int a, int b, string s) => null;
    }
}";
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInvalidLocalFunctionsAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public void Method()
    {
        object LocalFunction(int a, int b,
 string s) => null;
    }
}";
            var fixedCode = @"
class Foo
{
    public void Method()
    {
        object LocalFunction(
            int a,
            int b,
 string s) => null;
    }
}";
            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(7, 2) };
            await VerifyCSharpDiagnosticAsync(null, testCode, settings, expected, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(null, fixedCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
