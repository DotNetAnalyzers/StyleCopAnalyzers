// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1509OpeningBracesMustNotBePrecededByBlankLine,
        StyleCop.Analyzers.LayoutRules.SA1509CodeFixProvider>;

    public class SA1509CSharp7UnitTests : SA1509UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()

        {
        }
    }
}";
            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLocalFunctionDeclarationOpeningBraceHasTwoBlankLineAsync()
        {
            var testCode = @"
class Foo
{
    void Method()
    {
        void Bar()


        {
        }
    }
}";

            var fixedCode = @"
class Foo
{
    void Method()
    {
        void Bar()
        {
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);
            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
