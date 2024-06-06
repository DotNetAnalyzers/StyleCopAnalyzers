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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SX1116SplitParametersMustStartOnLineAfterDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SX1116CodeFixProvider>;

    public partial class SX1116CSharp7UnitTests : SX1116UnitTests
    {
        [Fact]
        public async Task TestValidLocalFunctionAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        object LocalFunction(int a, string s) => null;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidLocalFunctionsAsync()
        {
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
            DiagnosticResult expected = Diagnostic().WithLocation(6, 30);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
