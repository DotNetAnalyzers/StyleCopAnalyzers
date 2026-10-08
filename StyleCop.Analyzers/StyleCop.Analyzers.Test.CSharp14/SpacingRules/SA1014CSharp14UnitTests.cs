// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.SpacingRules.SA1014OpeningGenericBracketsMustBeSpacedCorrectly, StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1014CSharp14UnitTests : SA1014CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithSpaceBeforeBracketAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension {|#0:<|}T>(T source)
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension<T>(T source)
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("preceded");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithSpaceAfterBracketAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension{|#0:<|} T>(T source)
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension<T>(T source)
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("followed");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
