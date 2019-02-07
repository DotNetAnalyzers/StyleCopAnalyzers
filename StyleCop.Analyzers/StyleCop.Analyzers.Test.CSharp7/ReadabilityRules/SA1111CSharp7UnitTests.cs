// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1111ClosingParenthesisMustBeOnLineOfLastParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
