// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.LayoutRules.SA1507CodeMustNotContainMultipleBlankLinesInARow, StyleCop.Analyzers.LayoutRules.SA1507CodeFixProvider>;

    public partial class SA1507CSharp14UnitTests : SA1507CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int TestProperty1 => 0;


        public int TestProperty2 => 0;
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int TestProperty1 => 0;

        public int TestProperty2 => 0;
    }
}
";

            var expected = Diagnostic().WithSpan(7, 1, 9, 1);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
