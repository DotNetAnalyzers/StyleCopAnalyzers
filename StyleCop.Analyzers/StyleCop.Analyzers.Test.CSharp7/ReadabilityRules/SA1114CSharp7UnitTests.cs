// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1114CSharp7UnitTests : SA1114UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
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

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(8, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
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

        [Fact]
        public async Task TestLocalFunctionDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
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

        [Fact]
        public async Task TestLocalFunctionDeclarationNoParametersAsync()
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
    }
}
