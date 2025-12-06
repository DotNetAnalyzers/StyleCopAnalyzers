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
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public partial class SA1121CSharp10UnitTests : SA1121CSharp9UnitTests
    {
        [Fact]
        public async Task TestUsingNameChangeInFileScopedNamespaceAsync()
        {
            string oldSource = @"namespace Foo;

using MyInt = System.UInt32;
class Bar
{
{|#0:MyInt|} value = 3;
}
";
            string newSource = @"namespace Foo;

using MyInt = System.UInt32;
class Bar
{
uint value = 3;
}
";

            await new CSharpTest
            {
                TestCode = oldSource,
                ExpectedDiagnostics = { Diagnostic().WithLocation(0) },
                FixedCode = newSource,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3985, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3985")]
        public async Task TestExplicitLambdaReturnTypeAsync()
        {
            var testCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        var f = {|#0:Int32|} (int x) => x;
        var g = {|#1:System.Int32|} ({|#2:System.Int32|} value) => value;
    }
}
";

            var fixedCode = @"using System;

class TestClass
{
    void TestMethod()
    {
        var f = int (int x) => x;
        var g = int (int value) => value;
    }
}
";

            var expected = new DiagnosticResult[]
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
