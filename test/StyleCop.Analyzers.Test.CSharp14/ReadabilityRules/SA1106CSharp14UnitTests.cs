// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.ReadabilityRules.SA1106CodeMustNotContainEmptyStatements, StyleCop.Analyzers.ReadabilityRules.SA1106CodeFixProvider>;

    public partial class SA1106CSharp14UnitTests : SA1106CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockFollowedBySemicolonAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string source)
    {
    }[|;|]
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string source)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
