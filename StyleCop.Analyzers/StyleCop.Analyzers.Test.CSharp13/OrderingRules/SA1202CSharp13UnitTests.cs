// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp12.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<
        StyleCop.Analyzers.OrderingRules.SA1202ElementsMustBeOrderedByAccess>;

    public partial class SA1202CSharp13UnitTests : SA1202CSharp12UnitTests
    {
        [Fact]
        [WorkItem(4021, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4021")]
        public async Task TestPartialPublicPropertyAfterInternalPropertyAsync()
        {
            var testCode = @"
public partial class TypeName
{
    internal int OtherProperty { get; set; }

    public partial int {|#0:Test|} { get; set; }
}

public partial class TypeName
{
    public partial int Test
    {
        get => 0;
        set { }
    }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "internal");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4019, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4019")]
        public async Task TestRefStructExplicitInterfaceImplementationAfterInternalMemberAsync()
        {
            var testCode = @"
public interface IInterface
{
    void TestMethod();
}

public ref struct TestRefStruct : IInterface
{
    internal void TestMethod2() { }

    void IInterface.{|#0:TestMethod|}() { }
}";

            var expected = Diagnostic().WithLocation(0).WithArguments("public", "internal");

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
