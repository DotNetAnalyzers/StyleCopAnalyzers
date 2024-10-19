// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1113CommaMustBeOnSameLineAsPreviousParameter,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1113CSharp7UnitTests : SA1113UnitTests
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

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
