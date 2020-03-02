// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public class SA1026CSharp7UnitTests : SA1026UnitTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \r\n")]
        [InlineData(" \t \r\n\t ")]
        public async Task TestImplicitStackAllocArrayCreationExpressionAsync(string space)
        {
            string testCode = $"public class Foo {{ public unsafe Foo() {{ int* ints = stackalloc{space}[] {{ 1, 2, 3 }}; }} }}";
            const string expectedCode = "public class Foo { public unsafe Foo() { int* ints = stackalloc[] { 1, 2, 3 }; } }";
            DiagnosticResult[] expected = { Diagnostic().WithArguments("stackalloc").WithLocation(1, 54) };

            await VerifyCSharpFixAsync(LanguageVersion.CSharp7_3, testCode, expected, expectedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
