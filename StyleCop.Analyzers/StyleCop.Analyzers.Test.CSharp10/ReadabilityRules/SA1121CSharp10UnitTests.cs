// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    public class SA1121CSharp10UnitTests : SA1121CSharp9UnitTests
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
        public async Task TestUsingNameChangeInGlobalUsingInAnotherFileAsync()
        {
            var source1 = @"
global using MyDouble = System.Double;";

            var oldSource2 = @"
class TestClass
{
    private [|MyDouble|] x;
}";

            var newSource2 = @"
class TestClass
{
    private double x;
}";

            var test = new CSharpTest();
            test.TestState.Sources.Add(source1);
            test.TestState.Sources.Add(oldSource2);
            test.FixedState.Sources.Add(source1);
            test.FixedState.Sources.Add(newSource2);
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
