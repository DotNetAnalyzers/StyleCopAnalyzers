// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.SpacingRules;
    using Xunit;

    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1021NegativeSignsMustBeSpacedCorrectly,
        StyleCop.Analyzers.SpacingRules.TokenSpacingCodeFixProvider>;

    public partial class SA1021CSharp10UnitTests : SA1021CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3992, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3992")]
        public async Task TestLineSpanDirectiveAsync()
        {
            // #line span directives are primarily used in generated code, so the spacing of the - sign is not checked
            var testCode = @"public class TestClass
{
    public void TestMethod()
    {
#line (1,1)-(5,60) ""file.cs""
        int x = 0;
#line ( 1 , 1 ) - ( 5 , 60 ) 10 ""file.cs""
        int y = 0;
#line (1, 1) -(5, 60) 10 ""file.cs""
        int z = 0;
#line default
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
