// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1141UseTupleSyntax,
        StyleCop.Analyzers.ReadabilityRules.SA1141CodeFixProvider>;

    public partial class SA1141CSharp10UnitTests : SA1141CSharp9UnitTests
    {
        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestExplicitLambdaTupleTypesAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        var projector = {|#0:System.ValueTuple<int, int>|} (int x) => (x, x * x);
        var echo = ({|#1:System.ValueTuple<string, int>|} pair) => pair;
    }
}
";

            var fixedCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        var projector = (int, int) (int x) => (x, x * x);
        var echo = ((string, int) pair) => pair;
    }
}
";

            var expected = new DiagnosticResult[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
