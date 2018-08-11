// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1113CommaMustBeOnSameLineAsPreviousParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

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

            DiagnosticResult expected = Diagnostic().WithLocation(6, 18);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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
                    Diagnostic().WithLocation(6, 18),
                    Diagnostic().WithLocation(7, 18),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
