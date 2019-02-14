// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        Analyzers.ReadabilityRules.SA1130UseLambdaSyntax,
        Analyzers.ReadabilityRules.SA1130CodeFixProvider>;

    public class SA1130CSharp7UnitTests : SA1130UnitTests
    {
        [Fact]
        [WorkItem(2902, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/2902")]
        public async Task VerifyLocalFunctionAsync()
        {
            var testCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        EventHandler LocalTestFunction() => delegate { };
    }
}
";

            var fixedCode = @"using System;
public class TestClass
{
    public void TestMethod()
    {
        EventHandler LocalTestFunction() => (sender, e) => { };
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(6, 45),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
