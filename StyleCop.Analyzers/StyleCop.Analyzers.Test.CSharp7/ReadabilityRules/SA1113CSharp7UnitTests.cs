// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;

    public class SA1113CSharp7UnitTests : SA1113UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheSecondParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Method()
    {
        void Bar(string s
                 , int i)
        {
        }
    }
}";
            var fixedCode = @"public class Foo
{
    public void Method()
    {
        void Bar(string s,
                 int i)
        {
        }
    }
}";

            DiagnosticResult expected = this.CSharpDiagnostic().WithLocation(6, 18);

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithThreeParametersCommasPlacedAtTheSameLineAsTheNextParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Method()
    {
        void Bar(string s
                 , int i
                 , int i2)
        {
        }
    }
}";
            var fixedCode = @"public class Foo
{
    public void Method()
    {
        void Bar(string s,
                 int i,
                 int i2)
        {
        }
    }
}";

            DiagnosticResult[] expected =
                {
                    this.CSharpDiagnostic().WithLocation(6, 18),
                    this.CSharpDiagnostic().WithLocation(7, 18),
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, cancellationToken: CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationWithTwoParametersCommaPlacedAtTheSameLineAsTheFirstParameterAsync()
        {
            var testCode = @"public class Foo
{
    public void Method()
    {
        void Bar(string s,
                 int i)
        {
        }
    }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
