// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1116CSharp7UnitTests : SA1116UnitTests
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
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 30);
            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }
    }
}
