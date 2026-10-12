// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1312CSharp8UnitTests : SA1312CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3057, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3057")]
        public async Task TestUnderscoreOnlyNamesInUsingDeclarationAsync()
        {
            var testCode = @"using System.IO;

public class TypeName
{
    public void MethodName1()
    {
        using var _ = new MemoryStream();
        using var __ = new MemoryStream();
    }

    public void MethodName2()
    {
        using (var {|#0:_|} = new MemoryStream())
        {
        }

        var {|#1:__|} = new MemoryStream();
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_").WithLocation(0),
                Diagnostic().WithArguments("__").WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, testCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
