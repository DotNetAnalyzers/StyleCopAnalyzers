﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1116CodeFixProvider>;

    public partial class SA1116CSharp7UnitTests : SA1116UnitTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestValidLocalFunctionAsync(bool treatMultilineParametersAsSplit)
        {
            var settings = GetSettings(treatMultilineParametersAsSplit);

            var testCode = @"
class Foo
{
    public void Method()
    {
        object LocalFunction(int a, string s) => null;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, settings, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
        object LocalFunction(int a,
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
 string s) => null;
    }
}";
            DiagnosticResult[] expected = new[] { Diagnostic().WithLocation(6, 30) };
            await VerifyCSharpFixAsync(testCode, settings, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
