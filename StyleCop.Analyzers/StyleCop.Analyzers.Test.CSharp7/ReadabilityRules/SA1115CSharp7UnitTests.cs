// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1115CSharp7UnitTests : SA1115UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationEmptyLinesBetweenParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(int i, int z,

string s,

int j,
int k)
        {
        }
    }
}";

            DiagnosticResult expected1 = this.CSharpDiagnostic().WithLocation(8, 1);
            DiagnosticResult expected2 = this.CSharpDiagnostic().WithLocation(10, 1);

            await this.VerifyCSharpDiagnosticAsync(testCode, new[] { expected1, expected2 }, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationSecondParameterOnTheNextLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(int i,
string s)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationParametersAtTheSameLineAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void Bar(int i, string s)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
