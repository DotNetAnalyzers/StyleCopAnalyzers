// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1111CSharp7UnitTests : SA1111UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationWithOneParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s
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
        void Bar(string s)
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
        public async Task TestLocalFunctionDeclarationWithThreeParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s,
                        int i,
                        object o
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
        void Bar(string s,
                        int i,
                        object o)
        {

        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(9, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithNoParameterClosingParenthesisOnTheNextLineAsTheLastParameterAsync()
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithOneParameterClosingParenthesisOnTheSameLineAsTheLastParameterAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(string s)
        {

        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
