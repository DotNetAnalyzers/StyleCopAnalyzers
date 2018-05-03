// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1112CSharp7UnitTests : SA1112UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionWithNoParametersClosingParenthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(
)
        {

        }
    }
}";
            var fixedCode = @"
class Foo
{
    public void Method()
    {
        void Bar()
        {

        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(7, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionWithNoParametersClosingParenthesisOnTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar()
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionWithParametersClosingParenthesisOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(
string s)
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
