﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
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
        StyleCop.Analyzers.ReadabilityRules.SA1112ClosingParenthesisMustBeOnLineOfOpeningParenthesis,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

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

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }
    }
}
