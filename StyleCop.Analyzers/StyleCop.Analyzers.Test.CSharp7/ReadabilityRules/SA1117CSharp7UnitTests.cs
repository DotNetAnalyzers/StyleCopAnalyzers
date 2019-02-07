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
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.ReadabilityRules.SA1117ParametersMustBeOnSameLineOrSeparateLines>;

    public class SA1117CSharp7UnitTests : SA1117UnitTests
    {
        [Fact]
        public async Task TestValidLocalFunctionsAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        void LocalFunction1(
            int a, int b, string s) { }

        void LocalFunction2(
            int a,
            int b,
            string s) { }

        object LocalFunction3(int a, int b, string s) => null;
    }
}";
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestInvalidLocalFunctionsAsync()
        {
            var testCode = @"
class Foo
{
    public void Method()
    {
        object LocalFunction(int a, int b,
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
            int b,
 string s) => null;
    }
}";
            DiagnosticResult expected = Diagnostic().WithLocation(7, 2);
            await VerifyCSharpDiagnosticAsync(testCode, expected).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(fixedCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
            ////await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
