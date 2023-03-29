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

    public class SA1117CSharp9UnitTests : SA1117CSharp8UnitTests
    {
        [Fact]
        public async Task TestValidTargetTypedNewExpressionAsync()
        {
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidTargetTypedNewExpressionAsync()
        {
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

            DiagnosticResult expected = Diagnostic().WithLocation(11, 16);
            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
