// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.NamingRules.SA1313ParameterNamesMustBeginWithLowerCaseLetter, StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1313CSharp14UnitTests : SA1313CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithParameterNameAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string {|#0:Param|})
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension(string param)
    {
    }
}
";

            var expected = Diagnostic().WithLocation(0).WithArguments("Param");

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationWithoutParameterNameAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension(string)
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
