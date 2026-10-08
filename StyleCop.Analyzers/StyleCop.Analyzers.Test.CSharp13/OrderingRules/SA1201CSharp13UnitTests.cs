// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.OrderingRules.SA1201ElementsMustAppearInTheCorrectOrder>;

    public partial class SA1201CSharp13UnitTests : SA1201CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4019, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4019")]
        public async Task TestRefStructImplementingInterfaceFieldAfterMethodAsync()
        {
            var testCode = @"
public interface IInterface
{
    void TestMethod();
}

public ref struct TestRefStruct : IInterface
{
    public void TestMethod() { }

    public int {|#0:TestField|};
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("A field", "a method");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
