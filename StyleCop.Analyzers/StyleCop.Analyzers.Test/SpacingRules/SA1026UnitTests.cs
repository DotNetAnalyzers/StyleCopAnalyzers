// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1026CodeMustNotContainSpaceAfterNewKeywordInImplicitlyTypedArrayAllocation"/>.
    /// </summary>
    public class SA1026UnitTests
    {
        [Fact]
        public async Task TestValidSpacingOfImplicitlyTypedArrayAsync()
        {
            const string testCode = @"public class Foo
{
    public Foo()
    {
        var ints = new[] { 1, 2, 3 };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \r\n")]
        [InlineData(" \t \r\n\t ")]
        public async Task TestInvalidSpacingOfImplicitlyTypedArrayAsync(string space)
        {
            string testCode = string.Format("public class Foo {{ public Foo() {{ var ints = new{0}[] {{ 1, 2, 3 }}; }} }}", space);
            const string expectedCode = "public class Foo { public Foo() { var ints = new[] { 1, 2, 3 }; } }";
            DiagnosticResult expected = Diagnostic().WithArguments("new").WithLocation(1, 46);

            await VerifyCSharpFixAsync(testCode, expected, expectedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
