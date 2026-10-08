// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<StyleCop.Analyzers.NamingRules.SA1314TypeParameterNamesMustBeginWithT, StyleCop.Analyzers.NamingRules.SA1314CodeFixProvider>;

    public partial class SA1314CSharp14UnitTests : SA1314CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4023, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4023")]
        public async Task TestExtensionBlockDeclarationAsync()
        {
            var testCode = @"
public static class TestClass
{
    extension<[|Obj|]>(Obj param)
    {
    }
}
";

            var fixedCode = @"
public static class TestClass
{
    extension<TObj>(TObj param)
    {
    }
}
";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
