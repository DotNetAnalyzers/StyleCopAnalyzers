// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.LayoutRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.LayoutRules.SA1508ClosingBracesMustNotBePrecededByBlankLine, StyleCop.Analyzers.LayoutRules.SA1508CodeFixProvider>;

    public partial class SA1508CSharp14UnitTests : SA1508CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockClosingBracePrecededByBlankLineAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length1 => source.Length;

    [|}|]
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
        public int Length1 => source.Length;
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
